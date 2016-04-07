using BackgroundAudioShared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundAudioShared.Models
{
    public class MusicModel
    {
        public class Result
        {
            private string _image_thumb;
            private string _image_album;

            public string id { get; set; }
            public string music_title { get; set; }
            public string music_path { get; set; }

            public string image_album
            {
                get { return _image_album; }
                set
                {
                    if (!string.IsNullOrEmpty(value))
                        _image_album = value;
                    else
                        _image_album = Utils.defaultCoverImage;
                }
            }

            public string image_thumb
            {
                get { return _image_thumb; }
                set
                {
                    if (!string.IsNullOrEmpty(value))
                        _image_thumb = value;
                    else
                        _image_thumb = Utils.defaultCoverImage;
                }
            }
            public string music_view { get; set; }
            public string music_size { get; set; }
            public string music_duration { get; set; }
            public string albums { get; set; }
        }

        public class RootObject
        {
            public List<Result> result { get; set; }
        }
    }
}
