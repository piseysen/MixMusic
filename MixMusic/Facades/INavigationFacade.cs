using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixMusic.Facades
{
    /// <summary>
    /// Encapsulates page navigation.
    /// </summary>
    public interface INavigationFacade
    {
        /// <summary>
        /// Goes back to the previews view(s).
        /// </summary>
        /// <param name="steps">Number of views to go back.</param>
        void GoBack(int steps = 1);

        /// <summary>
        /// Navigates to the Setting view.
        /// </summary>
        void NavigateToSettingView();

        void NavigateToSingerView();

        void NavigateToProductionView();

        void NavigateToMainView();

        /// <summary>
        /// Removes the specified number of frames from the back stack.
        /// </summary>
        /// <param name="numberOfFrames">The number of frames.</param>
        void RemoveBackStackFrames(int numberOfFrames);



    }
}
