using BackgroundAudioShared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundAudioShared.Models
{
    public class SingerModel
    {

        public class Result
        {
            public string _imageAvatar;

            public string id { get; set; }
            public string singer_name { get; set; }

            public string imageAvatar
            {
                get { return _imageAvatar; }
                set
                {
                    if (!string.IsNullOrEmpty(value))
                        _imageAvatar = value;
                    else
                        _imageAvatar = Utils.defaultCoverImage;
                }
            }

        }

        public class RootObject
        {
            public List<Result> result { get; set; }
        }
    }
}
