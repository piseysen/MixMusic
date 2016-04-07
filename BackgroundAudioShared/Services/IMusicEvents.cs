using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundAudioShared.Services
{
    public delegate void PopularMusic(object sender, ItemListArgs e);
    public delegate void Singer(object sender, ItemListArgs e);
    public delegate void NewSong(object sender, ItemListArgs e);
    public delegate void Production(object sender, ItemListArgs e);

    public interface IMusicEvents
    {
      event PopularMusic OnPopularMusicLoaded;
      event Singer OnSingerLoaded;
      event NewSong OnNewSongLoaded;
      event Production OnProductionLoaded;
      void ConnectToSinger();
      void ConnectToProduction();
      void ConnectToNewSong();
      void ConnectToPopulrMusic();
    }
}
