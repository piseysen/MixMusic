using Microsoft.Practices.Unity;
using MixMusic.NavigationBar;
using MixMusic.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixMusic.Registries
{
    /// <summary>
    /// Dependency registry for navigation bar items.
    /// </summary>
    public class NavigationBarRegistry : RegistryBase, IRegistry
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="container">The Unity container.</param>
        public NavigationBarRegistry(IUnityContainer container) : base(container)
        {
        }

        /// <summary>
        /// Configures dependencies.
        /// </summary>
        public void Configure()
        {
            // Top items
            Container.RegisterTypeWithName<INavigationBarMenuItem, MainNavigationBarMenuItem>();
            Container.RegisterTypeWithName<INavigationBarMenuItem, ProductionNavigationBarMenuItem>();
        }
    }
}
