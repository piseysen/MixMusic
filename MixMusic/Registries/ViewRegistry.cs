using MixMusic.Facades;
using MixMusic.ViewModels;
using MixMusic.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixMusic.Registries
{
    public class ViewRegistry : IRegistry
    {
        /// <summary>
        /// Configures dependencies.
        /// </summary>
        public void Configure()
        {
            NavigationFacade.AddType(typeof(MainPage), typeof(MainViewModel));
            NavigationFacade.AddType(typeof(ProductionPage), typeof(ProductionViewModel));
            
        }
    }
}
