using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixMusic.Models
{
    /// <summary>
    /// Specifies the configuration data model
    /// </summary>
    /// 
    public class Config
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Config" /> class.
        /// </summary>
        /// <param name="buildVersion">The build version.</param>
        /// <param name="categoryThumbnailsSmallFormFactor">The number of category thumbnails for small devices.</param>
        /// <param name="categoryThumbnailsLargeFormFactor">The number of category thumbnails for large devices.</param>
        public Config(string buildVersion, int categoryThumbnailsSmallFormFactor,
            int categoryThumbnailsLargeFormFactor)
        {
            BuildVersion = buildVersion;
            CategoryThumbnailsSmallFormFactor = categoryThumbnailsSmallFormFactor;
            CategoryThumbnailsLargeFormFactor = categoryThumbnailsLargeFormFactor;
        }

        /// <summary>
        /// Gets or sets the service dll build version.
        /// </summary>
        public string BuildVersion { get; set; }

        /// <summary>
        /// Gets or sets the number of category thumbnails for large devices.
        /// </summary>
        /// <value>
        /// The number of category thumbnails.
        /// </value>
        public int CategoryThumbnailsLargeFormFactor { get; set; }

        /// <summary>
        /// Gets or sets the number of category thumbnails for small devices.
        /// </summary>
        /// <value>
        /// The number of category thumbnails.
        /// </value>
        public int CategoryThumbnailsSmallFormFactor { get; set; }

    }

 

}
