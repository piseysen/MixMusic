﻿using BackgroundAudioShared.Models;
using BackgroundAudioShared.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MixMusic.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ArtistPage : Page
    {
        public ObservableCollection<SingerModel.Result> SingerDataItems { get; private set; }

        public ArtistPage()
        {
            this.InitializeComponent();
            SingerDataItems = new ObservableCollection<SingerModel.Result>();
            this.Loaded += ArtistPage_Loaded;
            NavigationCacheMode = NavigationCacheMode.Required;
        }

        private void ArtistPage_Loaded(object sender, RoutedEventArgs e)
        {
            IMusicEvents imixmusic = new MixMusicData();
            imixmusic.OnSingerLoaded += Imixmusic_OnSingerLoaded;
            imixmusic.ConnectToSinger();
        }

        private void Imixmusic_OnSingerLoaded(object sender, ItemListArgs e)
        {
            App.iMixMusicData.ListItemSinger.Clear();
            foreach (var item in e.ListItemSinger)
            {
                SingerDataItems.Add(item);
            }
        }
    }
}
