using BackgroundAudioShared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixMusic.Models
{
    public class SongNavigationModel
    {
        public ObservableCollection<MusicModel.Result> SongCollection { get; set; }
        public string TrackId { get; set; }
        public string Position { get; set; }
        public int TypeId { get; set; }
    }
}
