using Noear.UWP.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundAudioShared.Services
{
    public class MixMusicWebservice
    {
        /// <summary>
        /// RequestAsync Method Post
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="values"></param>
        /// <param name="callback"></param>
        public async void RequestPostAsync(Uri uri, Dictionary<string, string> values,Action<string> callback)
        {
            try
            {
                var rsp = await new AsyncHttpClient().Url(uri.AbsoluteUri)                   
                    .Post(values);

                if (rsp.StatusCode == HttpStatusCode.OK)
                {
                    string data = rsp.GetString();
                    callback(data);
                }
                else
                {
                    string data = rsp.GetString();
                    callback(data);

                }
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                callback(ex.Message);

            }
        }

        /// <summary>
        /// RequestGetAsync
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="values"></param>
        /// <param name="callback"></param>
        public async void RequestGetAsync(Uri uri,Action<string> callback)
        {
            try
            {
                var rsp = await new AsyncHttpClient().Url(uri.AbsoluteUri)
                    .Get();

                if (rsp.StatusCode == HttpStatusCode.OK)
                {
                    string data = rsp.GetString();
                    callback(data);
                }
                else
                {
                    string data = rsp.GetString();
                    callback(data);

                }
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                callback(ex.Message);

            }
        }


    }
}
