﻿using BackgroundAudioShared.Models;
using BackgroundAudioShared.Services;
using MixMusic.ViewModels;
using ServiceLocation;
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
    public sealed partial class ProductionPage : Page
    {
        public ObservableCollection<ProductionModel.Result> ProductionDataItems { get; private set; }
        private readonly ProductionViewModel _viewModel;

        public ProductionPage()
        {
            this.InitializeComponent();
            ProductionDataItems = new ObservableCollection<ProductionModel.Result>();

            this.Loaded += ProductionPage_Loaded;
            NavigationCacheMode = NavigationCacheMode.Required;

            //_viewModel = ServiceLocator.Current.GetInstance<ProductionViewModel>();
            //DataContext = _viewModel;
        }

        private void ProductionPage_Loaded(object sender, RoutedEventArgs e)
        {
            IMusicEvents imixmusic = new MixMusicData();
            imixmusic.OnProductionLoaded += Imixmusic_OnProductionLoaded;
            imixmusic.ConnectToProduction();
        }

        private void Imixmusic_OnProductionLoaded(object sender, ItemListArgs e)
        {
            App.iMixMusicData.ListItemProduction.Clear();
            foreach (var item in e.ListItemProduction)
            {
                ProductionDataItems.Add(item);
            }
        }
    }
}
