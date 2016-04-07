using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundAudioShared.Services
{
    public class IServerUrls
    {
        Uri GetNewMusic { get; }
        Uri GetSinger { get; }
        Uri GetPopularMusic { get; }
        Uri GetProduction { get; }

    }
}
