using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace MixMusic.Models
{
    /// <summary>
    /// Defines the app environment
    /// </summary>
    public interface IAppEnvironment
    {
        /// <summary>
        /// Stores the current user that is logged in.
        /// </summary>
        User CurrentUser { get; set; }
    }
}
