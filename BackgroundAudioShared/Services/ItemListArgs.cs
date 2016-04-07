using BackgroundAudioShared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundAudioShared.Services
{
    public enum MusicDataLoaded
    {
        PopularMusic,
        NewSong,
        Singer,
        Production,
        GetMusicBySingerId
    }
    public class ItemListArgs:EventArgs
    {
        public ObservableCollection<MusicModel.Result> ListItemPopularMusic = new ObservableCollection<MusicModel.Result>();
        public ObservableCollection<SingerModel.Result> ListItemSinger = new ObservableCollection<SingerModel.Result>();
        public ObservableCollection<MusicModel.Result> ListItemNewSong = new ObservableCollection<MusicModel.Result>();
        public ObservableCollection<MusicModel.Result> ListItemSearchMusic = new ObservableCollection<MusicModel.Result>();
        public ObservableCollection<MusicModel.Result> ListItemGetMusicBySingerId = new ObservableCollection<MusicModel.Result>();
        public ObservableCollection<ProductionModel.Result> ListItemProduction = new ObservableCollection<ProductionModel.Result>();
    }
}
