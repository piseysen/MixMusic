using MixMusic.NavigationBar;
using ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixMusic.ViewModels
{
    public class ShellViewModel:ViewModelBase
    {
        public ShellViewModel()
        {
            //NavigationBarMenuItems = ServiceLocator.Current
            //    .GetAllInstances<INavigationBarMenuItem>()
            //    .Where(i => i.Position == NavigationBarItemPosition.Top)
            //    .ToList();

        }

        /// <summary>
        /// The navigation bar items at the bottom.
        /// </summary>
        public List<INavigationBarMenuItem> BottomNavigationBarMenuItems { get; }

        /// <summary>
        /// The navigation bar items at the top.
        /// </summary>
        public List<INavigationBarMenuItem> NavigationBarMenuItems { get; private set; }
    }
}
