using BackgroundAudioShared.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace BackgroundAudioShared.Services
{
    public class MixMusicData : IMusicEvents
    {
        public event NewSong OnNewSongLoaded=null;
        public event PopularMusic OnPopularMusicLoaded=null;
        public event Production OnProductionLoaded=null;
        public event Singer OnSingerLoaded=null;

        private ItemListArgs _musicListItems = new ItemListArgs();
        private MixMusicWebservice _mixMusicWebservice = new MixMusicWebservice();
        private ServerUrls _serverUrls = new ServerUrls();

        public async void ConnectToSinger()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, delegate () { LoadSinger(); });
        }

        public async void ConnectToProduction()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, delegate () { LoadProduction(); });
        }

        public async void ConnectToNewSong()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, delegate () { LoadNewMusic(); });
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, delegate () { LoadPopularMusic(); });
        }

        public async void ConnectToPopulrMusic()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, delegate () { LoadPopularMusic(); });
        }

        private void LoadNewMusic()
        {
            _musicListItems.ListItemNewSong.Clear();
            _mixMusicWebservice.RequestGetAsync(_serverUrls.GetNewMusic, (result) =>
            {
                var resultData = JsonConvert.DeserializeObject<MusicModel.RootObject>(result);
                if (resultData != null)
                {
                    if (resultData.result != null)
                    {
                        foreach (var items in resultData.result)
                        {
                            _musicListItems.ListItemNewSong.Add(items);
                        }
                    }
                    OnNewSongLoaded(this, _musicListItems);
                }
            });

        }

        private void LoadPopularMusic()
        {
            _musicListItems.ListItemPopularMusic.Clear();
            _mixMusicWebservice.RequestGetAsync(_serverUrls.GetPopularMusic, (result) =>
            {
                var resultData = JsonConvert.DeserializeObject<MusicModel.RootObject>(result);
                if (resultData != null)
                {
                    if (resultData.result!=null)
                    {
                        foreach(var items in resultData.result)
                        {
                            _musicListItems.ListItemPopularMusic.Add(items);
                        }
                    }
                    OnPopularMusicLoaded(this, _musicListItems);
                }
            });

        }

        private void LoadSinger()
        {
            _musicListItems.ListItemSinger.Clear();
            Dictionary<string, string> values = new Dictionary<string, string>();
            _mixMusicWebservice.RequestPostAsync(_serverUrls.GetSinger, values,(result) =>
            {
                var resultData = JsonConvert.DeserializeObject<SingerModel.RootObject>(result);
                if (resultData != null)
                {
                    if (resultData.result != null)
                    {
                        foreach (var items in resultData.result)
                        {
                            _musicListItems.ListItemSinger.Add(items);
                        }
                    }
                    OnSingerLoaded(this, _musicListItems);
                }
            });

        }

        private void LoadProduction()
        {
            _musicListItems.ListItemProduction.Clear();
            Dictionary<string, string> values = new Dictionary<string, string>();
            _mixMusicWebservice.RequestPostAsync(_serverUrls.GetProduction, values, (result) =>
            {
                var resultData = JsonConvert.DeserializeObject<ProductionModel.RootObject>(result);
                if (resultData != null)
                {
                    if (resultData.result != null)
                    {
                        foreach (var items in resultData.result)
                        {
                            _musicListItems.ListItemProduction.Add(items);
                        }
                    }
                    OnProductionLoaded(this, _musicListItems);
                }
            });

        }

    }
}
