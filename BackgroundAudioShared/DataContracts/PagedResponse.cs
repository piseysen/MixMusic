using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundAudioShared.DataContracts
{
    /// <summary>
    /// Specifies a paged list result that allows clients to request
    /// item collections page wise.
    /// </summary>
    /// <typeparam name="TContract">The item's type.</typeparam>
    public class PagedResponse<TContract>
    {
        /// <summary>
        /// The continuation token.
        /// </summary>
        public string ContinuationToken { get; set; }

        /// <summary>
        /// The items.
        /// </summary>
        public IList<TContract> Items { get; set; }
    }
}
