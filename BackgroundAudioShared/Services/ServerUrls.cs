using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundAudioShared.Services
{
    public class ServerUrls:IServerUrls
    {
        private const string domainMain = @"http://128.199.206.27/website/web_server/";
        private readonly string domain;
        public ServerUrls()
        {
            this.domain = domainMain;
        }

        private Uri absoluteUri(string value)
        {
            return new Uri(value, UriKind.Absolute);
        }


        public Uri GetNewMusic { get { return this.absoluteUri(this.domain + @"getkhmernewmusic"); } }
        public Uri GetSinger { get { return this.absoluteUri(this.domain + @"singer"); } }
        public Uri GetPopularMusic { get { return this.absoluteUri(this.domain + @"getkhmermusicpopular"); } }
        public Uri GetProduction { get { return this.absoluteUri(this.domain + @"production"); } }
        public Uri GetMusicBySingerId { get { return this.absoluteUri(this.domain + @"getsingerbyid"); } }



    }
}
