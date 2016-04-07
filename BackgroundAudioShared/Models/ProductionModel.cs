using BackgroundAudioShared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundAudioShared.Models
{
    public class ProductionModel
    {
        public class Result
        {
            private string _production_image;


            public string id { get; set; }
            public string production_name { get; set; }

            public string production_image
            {
                get { return _production_image; }
                set
                {
                    if (!string.IsNullOrEmpty(value))
                        _production_image = value;
                    else
                        _production_image = Utils.defaultCoverImage;
                }
            }

        }

        public class RootObject
        {
            public List<Result> result { get; set; }
        }
    }
}
