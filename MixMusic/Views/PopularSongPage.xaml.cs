﻿using BackgroundAudioShared;
using BackgroundAudioShared.Messages;
using BackgroundAudioShared.Models;
using BackgroundAudioShared.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace MixMusic.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PopularSongPage : Page
    {
        public ObservableCollection<MusicModel.Result> PopularSongDataItems { get; private set; }
        private Shell appShell;
        public PopularSongPage()
        {
            this.InitializeComponent();
            appShell = Shell.Instance;
            PopularSongDataItems = new ObservableCollection<MusicModel.Result>();
            this.Loaded += PopularSongPage_Loaded;
            // Setup the initialization lock
            backgroundAudioTaskStarted = new AutoResetEvent(false);
        }

        private void PopularSongPage_Loaded(object sender, RoutedEventArgs e)
        {
            IMusicEvents imixmusic = new MixMusicData();
            imixmusic.OnPopularMusicLoaded += Imixmusic_OnPopularMusicLoaded;
            imixmusic.ConnectToPopulrMusic();
        }

        private void Imixmusic_OnPopularMusicLoaded(object sender, ItemListArgs e)
        {
            App.iMixMusicData.ListItemPopularMusic.Clear();
            foreach (var item in e.ListItemPopularMusic)
            {
                PopularSongDataItems.Add(item);
            }
        }

        private void ListPopularSong_ItemClick(object sender, ItemClickEventArgs e)
        {
            var song = e.ClickedItem as MusicModel.Result;
            Debug.WriteLine("Clicked item from App: " + song.music_path.ToString());

            // Start the background task if it wasn't running
            if (!IsMyBackgroundTaskRunning || MediaPlayerState.Closed == CurrentPlayer.CurrentState)
            {
                // First update the persisted start track
                ApplicationSettingsHelper.SaveSettingsValue(ApplicationSettingsConstants.TrackId, song.music_path.ToString());
                ApplicationSettingsHelper.SaveSettingsValue(ApplicationSettingsConstants.Position, new TimeSpan().ToString());

                // Start task
                StartBackgroundAudioTask();
            }
            else
            {
                // Switch to the selected track
                MessageService.SendMessageToBackground(new TrackChangedMessage(new Uri(song.music_path)));
            }

            if (MediaPlayerState.Paused == CurrentPlayer.CurrentState)
            {
                CurrentPlayer.Play();
            }
        }

        #region PlayerBackground

        #region Private Fields and Properties
        private MainPage rootPage;
        private AutoResetEvent backgroundAudioTaskStarted;
        private bool _isMyBackgroundTaskRunning = false;
        private Dictionary<string, BitmapImage> albumArtCache = new Dictionary<string, BitmapImage>();
        const int RPC_S_SERVER_UNAVAILABLE = -2147023174; // 0x800706BA


        /// <summary>
        /// Gets the information about background task is running or not by reading the setting saved by background task.
        /// This is used to determine when to start the task and also when to avoid sending messages.
        /// </summary>
        private bool IsMyBackgroundTaskRunning
        {
            get
            {
                if (_isMyBackgroundTaskRunning)
                    return true;

                string value = ApplicationSettingsHelper.ReadResetSettingsValue(ApplicationSettingsConstants.BackgroundTaskState) as string;
                if (value == null)
                {
                    return false;
                }
                else
                {
                    try
                    {
                        _isMyBackgroundTaskRunning = EnumHelper.Parse<BackgroundTaskState>(value) == BackgroundTaskState.Running;
                    }
                    catch (ArgumentException)
                    {
                        _isMyBackgroundTaskRunning = false;
                    }
                    return _isMyBackgroundTaskRunning;
                }
            }
        }
        #endregion

        /// <summary>
        /// You should never cache the MediaPlayer and always call Current. It is possible
        /// for the background task to go away for several different reasons. When it does
        /// an RPC_S_SERVER_UNAVAILABLE error is thrown. We need to reset the foreground state
        /// and restart the background task.
        /// </summary>
        private MediaPlayer CurrentPlayer
        {
            get
            {
                MediaPlayer mp = null;
                int retryCount = 2;

                while (mp == null && --retryCount >= 0)
                {
                    try
                    {
                        mp = BackgroundMediaPlayer.Current;
                    }
                    catch (Exception ex)
                    {
                        if (ex.HResult == RPC_S_SERVER_UNAVAILABLE)
                        {
                            // The foreground app uses RPC to communicate with the background process.
                            // If the background process crashes or is killed for any reason RPC_S_SERVER_UNAVAILABLE
                            // is returned when calling Current. We must restart the task, the while loop will retry to set mp.
                            ResetAfterLostBackground();
                            StartBackgroundAudioTask();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                if (mp == null)
                {
                    throw new Exception("Failed to get a MediaPlayer instance.");
                }

                return mp;
            }
        }

        /// <summary>
        /// The background task did exist, but it has disappeared. Put the foreground back into an initial state. Unfortunately,
        /// any attempts to unregister things on BackgroundMediaPlayer.Current will fail with the RPC error once the background task has been lost.
        /// </summary>
        private void ResetAfterLostBackground()
        {
            BackgroundMediaPlayer.Shutdown();
            _isMyBackgroundTaskRunning = false;
            backgroundAudioTaskStarted.Reset();
            prevButton.IsEnabled = true;
            nextButton.IsEnabled = true;
            ApplicationSettingsHelper.SaveSettingsValue(ApplicationSettingsConstants.BackgroundTaskState, BackgroundTaskState.Unknown.ToString());
            playButton.Content = "| |";

            try
            {
                BackgroundMediaPlayer.MessageReceivedFromBackground += BackgroundMediaPlayer_MessageReceivedFromBackground;
            }
            catch (Exception ex)
            {
                if (ex.HResult == RPC_S_SERVER_UNAVAILABLE)
                {
                    throw new Exception("Failed to get a MediaPlayer instance.");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            // Subscribe to list UI changes
            //ListPopularSong.ItemClick += PlaylistView_ItemClick;

            // Adding App suspension handlers here so that we can unsubscribe handlers 
            // that access BackgroundMediaPlayer events
            Application.Current.Suspending += ForegroundApp_Suspending;
            Application.Current.Resuming += ForegroundApp_Resuming;
            ApplicationSettingsHelper.SaveSettingsValue(ApplicationSettingsConstants.AppState, AppState.Active.ToString());
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (_isMyBackgroundTaskRunning)
            {
                RemoveMediaPlayerEventHandlers();
                ApplicationSettingsHelper.SaveSettingsValue(ApplicationSettingsConstants.BackgroundTaskState, BackgroundTaskState.Running.ToString());
            }

            base.OnNavigatedFrom(e);
        }

        #region Foreground App Lifecycle Handlers
        /// <summary>
        /// Read persisted current track information from application settings
        /// </summary>
        private Uri GetCurrentTrackIdAfterAppResume()
        {
            object value = ApplicationSettingsHelper.ReadResetSettingsValue(ApplicationSettingsConstants.TrackId);
            if (value != null)
                return new Uri((String)value);
            else
                return null;
        }

        /// <summary>
        /// Sends message to background informing app has resumed
        /// Subscribe to MediaPlayer events
        /// </summary>
        void ForegroundApp_Resuming(object sender, object e)
        {
            ApplicationSettingsHelper.SaveSettingsValue(ApplicationSettingsConstants.AppState, AppState.Active.ToString());

            // Verify the task is running
            if (IsMyBackgroundTaskRunning)
            {
                // If yes, it's safe to reconnect to media play handlers
                AddMediaPlayerEventHandlers();

                // Send message to background task that app is resumed so it can start sending notifications again
                MessageService.SendMessageToBackground(new AppResumedMessage());

                UpdateTransportControls(CurrentPlayer.CurrentState);

                var trackId = GetCurrentTrackIdAfterAppResume();
                //txtCurrentTrack.Text = trackId == null ? string.Empty : playlistView.GetSongById(trackId).Title;
                //txtCurrentState.Text = CurrentPlayer.CurrentState.ToString();
            }
            else
            {
                playButton.Content = ">";     // Change to play button
                //txtCurrentTrack.Text = string.Empty;
                //txtCurrentState.Text = "Background Task Not Running";
            }
        }

        /// <summary>
        /// Send message to Background process that app is to be suspended
        /// Stop clock and slider when suspending
        /// Unsubscribe handlers for MediaPlayer events
        /// </summary>
        void ForegroundApp_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // Only if the background task is already running would we do these, otherwise
            // it would trigger starting up the background task when trying to suspend.
            if (IsMyBackgroundTaskRunning)
            {
                // Stop handling player events immediately
                RemoveMediaPlayerEventHandlers();

                // Tell the background task the foreground is suspended
                MessageService.SendMessageToBackground(new AppSuspendedMessage());
            }

            // Persist that the foreground app is suspended
            ApplicationSettingsHelper.SaveSettingsValue(ApplicationSettingsConstants.AppState, AppState.Suspended.ToString());

            deferral.Complete();
        }
        #endregion

        #region Background MediaPlayer Event handlers
        /// <summary>
        /// MediaPlayer state changed event handlers. 
        /// Note that we can subscribe to events even if Media Player is playing media in background
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        async void MediaPlayer_CurrentStateChanged(MediaPlayer sender, object args)
        {
            var currentState = sender.CurrentState; // cache outside of completion or you might get a different value
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Update state label
               // txtCurrentState.Text = currentState.ToString();

                // Update controls
                UpdateTransportControls(currentState);
            });
        }

        /// <summary>
        /// This event is raised when a message is recieved from BackgroundAudioTask
        /// </summary>
        async void BackgroundMediaPlayer_MessageReceivedFromBackground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            TrackChangedMessage trackChangedMessage;
            if (MessageService.TryParseMessage(e.Data, out trackChangedMessage))
            {
                // When foreground app is active change track based on background message
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    // If playback stopped then clear the UI
                    if (trackChangedMessage.TrackId == null)
                    {
                        //playlistView.SelectedIndex = -1;
                        //albumArt.Source = null;
                        //txtCurrentTrack.Text = string.Empty;
                        prevButton.IsEnabled = false;
                        nextButton.IsEnabled = false;
                        return;
                    }

                    //var songIndex = playlistView.GetSongIndexById(trackChangedMessage.TrackId);
                    //var song = playlistView.Songs[songIndex];

                    // Update list UI
                    //playlistView.SelectedIndex = songIndex;

                    // Update the album art
                    //albumArt.Source = albumArtCache[song.AlbumArtUri.ToString()];

                    // Update song title
                   // txtCurrentTrack.Text = song.Title;

                    // Ensure track buttons are re-enabled since they are disabled when pressed
                    prevButton.IsEnabled = true;
                    nextButton.IsEnabled = true;
                });
                return;
            }

            BackgroundAudioTaskStartedMessage backgroundAudioTaskStartedMessage;
            if (MessageService.TryParseMessage(e.Data, out backgroundAudioTaskStartedMessage))
            {
                // StartBackgroundAudioTask is waiting for this signal to know when the task is up and running
                // and ready to receive messages
                Debug.WriteLine("BackgroundAudioTask started");
                backgroundAudioTaskStarted.Set();
                return;
            }
        }

        #endregion

        #region Button and Control Click Event Handlers
        private void PlaylistView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var song = e.ClickedItem as MusicModel.Result;
            Debug.WriteLine("Clicked item from App: " + song.music_path.ToString());

            // Start the background task if it wasn't running
            if (!IsMyBackgroundTaskRunning || MediaPlayerState.Closed == CurrentPlayer.CurrentState)
            {
                // First update the persisted start track
                ApplicationSettingsHelper.SaveSettingsValue(ApplicationSettingsConstants.TrackId, song.music_path.ToString());
                ApplicationSettingsHelper.SaveSettingsValue(ApplicationSettingsConstants.Position, new TimeSpan().ToString());

                // Start task
                StartBackgroundAudioTask();
            }
            else
            {
                // Switch to the selected track
                MessageService.SendMessageToBackground(new TrackChangedMessage(new Uri(song.music_path)));
            }

            if (MediaPlayerState.Paused == CurrentPlayer.CurrentState)
            {
                CurrentPlayer.Play();
            }
        }

        /// <summary>
        /// Sends message to the background task to skip to the previous track.
        /// </summary>
        private void prevButton_Click(object sender, RoutedEventArgs e)
        {
            MessageService.SendMessageToBackground(new SkipPreviousMessage());

            // Prevent the user from repeatedly pressing the button and causing 
            // a backlong of button presses to be handled. This button is re-eneabled 
            // in the TrackReady Playstate handler.
            prevButton.IsEnabled = false;
        }

        /// <summary>
        /// If the task is already running, it will just play/pause MediaPlayer Instance
        /// Otherwise, initializes MediaPlayer Handlers and starts playback
        /// track or to pause if we're already playing.
        /// </summary>
        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Play button pressed from App");
            if (IsMyBackgroundTaskRunning)
            {
                if (MediaPlayerState.Playing == CurrentPlayer.CurrentState)
                {
                    CurrentPlayer.Pause();
                }
                else if (MediaPlayerState.Paused == CurrentPlayer.CurrentState)
                {
                    CurrentPlayer.Play();
                }
                else if (MediaPlayerState.Closed == CurrentPlayer.CurrentState)
                {
                    StartBackgroundAudioTask();
                }
            }
            else
            {
                StartBackgroundAudioTask();
            }
        }

        /// <summary>
        /// Tells the background audio agent to skip to the next track.
        /// </summary>
        /// <param name="sender">The button</param>
        /// <param name="e">Click event args</param>
        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            MessageService.SendMessageToBackground(new SkipNextMessage());

            // Prevent the user from repeatedly pressing the button and causing 
            // a backlong of button presses to be handled. This button is re-eneabled 
            // in the TrackReady Playstate handler.
            nextButton.IsEnabled = false;
        }

        private void speedButton_Click(object sender, RoutedEventArgs e)
        {
            // Create menu and add commands
            var popupMenu = new PopupMenu();

            popupMenu.Commands.Add(new UICommand("4.0x", command => CurrentPlayer.PlaybackRate = 4.0));
            popupMenu.Commands.Add(new UICommand("2.0x", command => CurrentPlayer.PlaybackRate = 2.0));
            popupMenu.Commands.Add(new UICommand("1.5x", command => CurrentPlayer.PlaybackRate = 1.5));
            popupMenu.Commands.Add(new UICommand("1.0x", command => CurrentPlayer.PlaybackRate = 1.0));
            popupMenu.Commands.Add(new UICommand("0.5x", command => CurrentPlayer.PlaybackRate = 0.5));

            // Get button transform and then offset it by half the button
            // width to center. This will show the popup just above the button.
            var button = (Button)sender;
            var transform = button.TransformToVisual(null);
            var point = transform.TransformPoint(new Point(button.Width / 2, 0));

            // Show popup
            var ignoreAsyncResult = popupMenu.ShowAsync(point);

        }
        #endregion Button Click Event Handlers

        #region Media Playback Helper methods
        private void UpdateTransportControls(MediaPlayerState state)
        {
            if (state == MediaPlayerState.Playing)
            {
                playButton.Content = "| |";     // Change to pause button
            }
            else
            {
                playButton.Content = ">";     // Change to play button
            }
        }

        /// <summary>
        /// Unsubscribes to MediaPlayer events. Should run only on suspend
        /// </summary>
        private void RemoveMediaPlayerEventHandlers()
        {
            try
            {
                BackgroundMediaPlayer.Current.CurrentStateChanged -= this.MediaPlayer_CurrentStateChanged;
                BackgroundMediaPlayer.MessageReceivedFromBackground -= BackgroundMediaPlayer_MessageReceivedFromBackground;
            }
            catch (Exception ex)
            {
                if (ex.HResult == RPC_S_SERVER_UNAVAILABLE)
                {
                    // do nothing
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Subscribes to MediaPlayer events
        /// </summary>
        private void AddMediaPlayerEventHandlers()
        {
            CurrentPlayer.CurrentStateChanged += this.MediaPlayer_CurrentStateChanged;

            try
            {
                BackgroundMediaPlayer.MessageReceivedFromBackground += BackgroundMediaPlayer_MessageReceivedFromBackground;
            }
            catch (Exception ex)
            {
                if (ex.HResult == RPC_S_SERVER_UNAVAILABLE)
                {
                    // Internally MessageReceivedFromBackground calls Current which can throw RPC_S_SERVER_UNAVAILABLE
                    ResetAfterLostBackground();
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Initialize Background Media Player Handlers and starts playback
        /// </summary>
        private void StartBackgroundAudioTask()
        {
            AddMediaPlayerEventHandlers();

            var startResult = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                bool result = backgroundAudioTaskStarted.WaitOne(10000);
                //Send message to initiate playback
                if (result == true)
                {
                    MessageService.SendMessageToBackground(new UpdatePlaylistMessage(PopularSongDataItems.ToList()));
                    MessageService.SendMessageToBackground(new StartPlaybackMessage());
                }
                else
                {
                    throw new Exception("Background Audio Task didn't start in expected time");
                }
            });
            startResult.Completed = new AsyncActionCompletedHandler(BackgroundTaskInitializationCompleted);
        }

        private void BackgroundTaskInitializationCompleted(IAsyncAction action, AsyncStatus status)
        {
            if (status == AsyncStatus.Completed)
            {
                Debug.WriteLine("Background Audio Task initialized");
            }
            else if (status == AsyncStatus.Error)
            {
                Debug.WriteLine("Background Audio Task could not initialized due to an error ::" + action.ErrorCode.ToString());
            }
        }
        #endregion

        #endregion

    }
}
