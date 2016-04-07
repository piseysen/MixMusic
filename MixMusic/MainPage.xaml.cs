using BackgroundAudioShared.Models;
using BackgroundAudioShared.Services;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MixMusic.Views;
using MixMusic.Models;
using MixMusic.Common;

namespace MixMusic
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<MusicModel.Result> PopularSongDataItems { get; private set; }
        public ObservableCollection<MusicModel.Result> NewSongDataItems { get; private set; }
        public ObservableCollection<MusicModel.Result> FullSongDataItems { get; private set; }

        public MainPage()
        {
            this.InitializeComponent();
            PopularSongDataItems = new ObservableCollection<MusicModel.Result>();
            NewSongDataItems = new ObservableCollection<MusicModel.Result>();
            FullSongDataItems = new ObservableCollection<MusicModel.Result>();
            this.Loaded += MainPage_Loaded;
            NavigationCacheMode = NavigationCacheMode.Required;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            IMusicEvents imixmusic = new MixMusicData();
            imixmusic.OnNewSongLoaded += Imixmusic_OnNewSongLoaded;
            imixmusic.OnPopularMusicLoaded += Imixmusic_OnPopularMusicLoaded;
            imixmusic.ConnectToNewSong();
        }

        private void Imixmusic_OnPopularMusicLoaded(object sender, ItemListArgs e)
        {
            App.iMixMusicData.ListItemPopularMusic.Clear();
            foreach (var item in e.ListItemPopularMusic)
            {
                PopularSongDataItems.Add(item);
            }
        }

        private void Imixmusic_OnNewSongLoaded(object sender, ItemListArgs e)
        {
            App.iMixMusicData.ListItemNewSong.Clear();
            foreach (var item in e.ListItemNewSong)
            {
                NewSongDataItems.Add(item);
            }
        }

        #region Private Event

        private void ListNewSong_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as MusicModel.Result;
            if (item != null)
            {
                FullSongDataItems.Clear();
                FullSongDataItems.AddRang(NewSongDataItems);
                FullSongDataItems.AddRang(PopularSongDataItems);

                var data = new SongNavigationModel()
                {
                   SongCollection= FullSongDataItems,
                   TrackId= item.music_path.ToString(),
                   Position = new TimeSpan().ToString()

                };
                Frame?.Navigate(typeof(PlayingNowPage), data);
            }
        }

        private void ListPopularSong_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as MusicModel.Result;
            if (item != null)
            {
                FullSongDataItems.Clear();
                FullSongDataItems.AddRang(NewSongDataItems);
                FullSongDataItems.AddRang(PopularSongDataItems);
                var data = new SongNavigationModel()
                {
                    SongCollection = FullSongDataItems,
                    TrackId = item.music_path.ToString(),
                    Position = new TimeSpan().ToString()

                };
                Frame?.Navigate(typeof(PlayingNowPage), data);
            }
        }

        #endregion


    }
}
