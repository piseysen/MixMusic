using Microsoft.Practices.Unity;
using MixMusic.Facades;
using MixMusic.Models;
using MixMusic.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixMusic.Registries
{
    /// <summary>
    /// Registry for services.
    /// </summary>
    public class ServicesRegistry : RegistryBase, IRegistry
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="container">The Unity container.</param>
        public ServicesRegistry(IUnityContainer container) : base(container)
        {
        }

        /// <summary>
        /// Configures dependencies.
        /// </summary>
        public void Configure()
        {
            Container.RegisterInstance(typeof(IAppEnvironment), new AppEnvironment());

            Container.RegisterType<IDialogService, MessageDialogService>();
            Container.RegisterType<INavigationFacade, NavigationFacade>();
        }
    }
}
