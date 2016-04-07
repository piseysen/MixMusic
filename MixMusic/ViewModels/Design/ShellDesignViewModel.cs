using MixMusic.NavigationBar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixMusic.ViewModels.Design
{
    public class ShellDesignViewModel
    {
        public ShellDesignViewModel()
        {
            NavigationBarMenuItems = new List<INavigationBarMenuItem>();

            BottomNavigationBarMenuItems = new List<INavigationBarMenuItem>
            {
                //new SettingsNavigationBarMenuItem(),
                //new DebugNavigationBarMenuItem()
            };
        }
        // public List<INavigationBarMenuItem> BottomNavigationBarMenuItems { get; }

        public List<INavigationBarMenuItem> BottomNavigationBarMenuItems { get; }

        public List<INavigationBarMenuItem> NavigationBarMenuItems { get; private set; }
    }

   
}
