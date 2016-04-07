using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixMusic.Registries
{
    /// <summary>
    /// Dependency registry for ViewModels.
    /// </summary>
    public class ViewModelRegistry : RegistryBase, IRegistry
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="container">The Unity container.</param>
        public ViewModelRegistry(IUnityContainer container) : base(container)
        {
        }

        /// <summary>
        /// Configures dependencies.
        /// </summary>
        public void Configure()
        {
            // We use the container controlled lifetime manager here, as we want to keep instances (for backward navigation)
            // unless we request a new one explicitly (forward navigation).
           // Container.RegisterType<CategoriesViewModel>(new ContainerControlledLifetimeManager());
            //Container.RegisterType<StreamViewModel>(new ExternallyControlledLifetimeManager());
            //Container.RegisterType<IUploadFinishedHandler, DefaultUploadFinishedHandler>();
        }
    }
}
