using BackgroundAudioShared;
using BackgroundAudioShared.Messages;
using BackgroundAudioShared.Models;
using MixMusic.Helpers;
using MixMusic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Windows.Foundation;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace MixMusic.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlayingNowPage : Page
    {
        private SongNavigationModel currentData;
        public ObservableCollection<MusicModel.Result> ListAllTrackItems { get; private set; }
        private DispatcherTimer MediaTimer;
        private bool isDragging = false;
        private bool isPlayingInCurrentState = false;

        public PlayingNowPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            ListAllTrackItems = new ObservableCollection<MusicModel.Result>();

            MediaTimer = new DispatcherTimer();
            MediaTimer.Interval = TimeSpan.FromMilliseconds(200);
            MediaTimer.Tick += MediaTimer_Tick;
            backgroundAudioTaskStarted = new AutoResetEvent(false);
        }

        #region Navigation

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Application.Current.Suspending += ForegroundApp_Suspending;
            Application.Current.Resuming += ForegroundApp_Resuming;
            ApplicationSettingsHelper.SaveSettingsValue(ApplicationSettingsConstants.AppState, AppState.Active.ToString());

            if (e.Parameter != null)
            {
                var data = e.Parameter as SongNavigationModel;
                if (data != null)
                {
                    Utils.IsPlayingNowActive = true;
                    currentData = new SongNavigationModel();
                    currentData = data;
                    ListAllTrackItems.Clear();
                    ListAllTracking.ItemsSource = null;
                    ListAllTracking.DataContext = null;
                    ListAllTracking.ItemsSource= currentData.SongCollection;
                    isPlayingInCurrentState = true;
                    FirstInitializeMusic(currentData);
                }
            }
            else
            {
                isPlayingInCurrentState = false;
            }
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (_isMyBackgroundTaskRunning)
            {
                //RemoveMediaPlayerEventHandlers();
                //ApplicationSettingsHelper.SaveSettingsValue(ApplicationSettingsConstants.BackgroundTaskState, BackgroundTaskState.Running.ToString());
            }

            base.OnNavigatedFrom(e);
        }

        #endregion

        #region Method

        private void FirstInitializeMusic(SongNavigationModel data)
        {
            Debug.WriteLine("Clicked item from App: " + data.TrackId);

            // Start the background task if it wasn't running
            if (!IsMyBackgroundTaskRunning || MediaPlayerState.Closed == CurrentPlayer.CurrentState)
            {
                // First update the persisted start track
                ApplicationSettingsHelper.SaveSettingsValue(ApplicationSettingsConstants.TrackId, data.TrackId);
                ApplicationSettingsHelper.SaveSettingsValue(ApplicationSettingsConstants.Position, data.Position);

                // Start task
                StartBackgroundAudioTask();
            }
            else
            {
                // Switch to the selected track
                MessageService.SendMessageToBackground(new TrackChangedMessage(new Uri(data.TrackId)));               
            }

            if (MediaPlayerState.Paused == CurrentPlayer.CurrentState)
            {
                CurrentPlayer.Play();
            }
        }

        #endregion

        #region PlayerBackground

        #region Private Fields and Properties

        private AutoResetEvent backgroundAudioTaskStarted;
        private bool _isMyBackgroundTaskRunning = false;
        //private Dictionary<string, BitmapImage> albumArtCache = new Dictionary<string, BitmapImage>();
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
            BtnPreviousSongs.IsEnabled = true;
            BtnNextSongs.IsEnabled = true;
            ApplicationSettingsHelper.SaveSettingsValue(ApplicationSettingsConstants.BackgroundTaskState, BackgroundTaskState.Unknown.ToString());
            playIcon.Symbol = Symbol.Pause;

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
                txtCurrentTrack.Text = trackId == null ? string.Empty : currentData.SongCollection.Single(x=>x.music_path==trackId.ToString()).music_title;
                //txtCurrentState.Text = CurrentPlayer.CurrentState.ToString();
            }
            else
            {
                playIcon.Symbol = Symbol.Play;  // Change to play button
                txtCurrentTrack.Text = string.Empty;
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
                        ListAllTracking.SelectedIndex = -1;
                        imgThumbailSongs.Source = null;
                        txtCurrentTrack.Text = string.Empty;
                        txtAlbum.Text = string.Empty;
                        BtnPreviousSongs.IsEnabled = false;
                        BtnNextSongs.IsEnabled = false;
                        return;
                    }

                    var songIndex = currentData.SongCollection.ToList().FindIndex(x=>x.music_path== trackChangedMessage.TrackId.AbsoluteUri);
                    if (songIndex != -1)
                    {
                        var song = currentData.SongCollection[songIndex];

                        // Update list UI
                        ListAllTracking.SelectedIndex = songIndex;

                        // Update the album art
                        imgThumbailSongs.Source = new BitmapImage(new Uri(song.image_album));

                        // Update song title
                        txtCurrentTrack.Text = song.music_title;
                        txtAlbum.Text = song.albums;
                    }

                    // Ensure track buttons are re-enabled since they are disabled when pressed
                    BtnPreviousSongs.IsEnabled =true;
                    BtnNextSongs.IsEnabled = true;
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
      
        

        /// <summary>
        /// Sends message to the background task to skip to the previous track.
        /// </summary>
        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if(ListAllTracking.SelectedIndex!=-1 && ListAllTracking.SelectedIndex != 0)
                MessageService.SendMessageToBackground(new SkipPreviousMessage());

            // Prevent the user from repeatedly pressing the button and causing 
            // a backlong of button presses to be handled. This button is re-eneabled 
            // in the TrackReady Playstate handler.
            //AppBarPrevButton.IsEnabled = false;
        }

        /// <summary>
        /// If the task is already running, it will just play/pause MediaPlayer Instance
        /// Otherwise, initializes MediaPlayer Handlers and starts playback
        /// track or to pause if we're already playing.
        /// </summary>
        private void PlayButton_Click(object sender, RoutedEventArgs e)
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
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            MessageService.SendMessageToBackground(new SkipNextMessage());

            // Prevent the user from repeatedly pressing the button and causing 
            // a backlong of button presses to be handled. This button is re-eneabled 
            // in the TrackReady Playstate handler.
            //AppBarNextButton.IsEnabled = false;
        }

        
        #endregion Button Click Event Handlers

        #region Media Playback Helper methods
        private void UpdateTransportControls(MediaPlayerState state)
        {
            if (state == MediaPlayerState.Playing)
            {
                playIcon.Symbol =Symbol.Pause; // Change to pause button
            }
            else
            {
                playIcon.Symbol = Symbol.Play;  // Change to play button
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
                MediaTimer.Stop();
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
            CurrentPlayer.MediaOpened += CurrentPlayer_MediaOpened;
            MediaTimer.Start();
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

        private void CurrentPlayer_MediaOpened(MediaPlayer sender, object args)
        {
            CommonHelper.CallOnUiThreadAsync(() =>
            {
                TimeSpan ts = CurrentPlayer.NaturalDuration;
                slideSeeker.Maximum = ts.TotalSeconds;
                slideSeeker.SmallChange = 1;
                slideSeeker.LargeChange = Math.Min(10, ts.TotalSeconds / 10);
            });

        }

        private void MediaTimer_Tick(object sender, object e)
        {
            if (!isDragging)
            {
                slideSeeker.Value = CurrentPlayer.Position.TotalSeconds;
                //currentTime = slideSeeker.Value;
                TimeSpan ts = CurrentPlayer.Position;
                TotalDuraton.Text = String.Format("{0}", CurrentPlayer.NaturalDuration.ToString(@"hh\:mm\:ss"));
                StartDuraton.Text = string.Format("{1:D2}:{2:D2}", ts.Hours, ts.Minutes, ts.Seconds);
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
                    MessageService.SendMessageToBackground(new UpdatePlaylistMessage(currentData.SongCollection.ToList()));
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

        private void ListAllTracking_ItemClick(object sender, ItemClickEventArgs e)
        {
            var data = e.ClickedItem as MusicModel.Result;
            if (data != null)
            {
                Debug.WriteLine("Clicked item from App: " + data.music_path);

                // Start the background task if it wasn't running
                if (!IsMyBackgroundTaskRunning || MediaPlayerState.Closed == CurrentPlayer.CurrentState)
                {
                    // First update the persisted start track
                    ApplicationSettingsHelper.SaveSettingsValue(ApplicationSettingsConstants.TrackId, data.music_path);
                    ApplicationSettingsHelper.SaveSettingsValue(ApplicationSettingsConstants.Position,new TimeSpan().ToString());

                    // Start task
                    StartBackgroundAudioTask();
                }
                else
                {
                    // Switch to the selected track
                    MessageService.SendMessageToBackground(new TrackChangedMessage(new Uri(data.music_path)));
                }

                if (MediaPlayerState.Paused == CurrentPlayer.CurrentState)
                {
                    CurrentPlayer.Play();
                }
            }
        }
    }
}
