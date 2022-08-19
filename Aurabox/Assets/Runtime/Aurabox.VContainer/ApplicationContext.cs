using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Aurabox.Utilities;
using UnityEngine.Pool;
using VContainer;
using VContainer.Diagnostics;
using VContainer.Unity;

[assembly: InternalsVisibleTo("Aurabox.Scenes")]
namespace Aurabox.VContainer
{
    /// <summary>
    /// The application context controls dependency management for the entire project.
    /// There should only be one active at once, and it *should* exist only within the
    /// "bootstrapper"/first scene.
    ///
    /// There should as little as possible dependencies in the same scope as the application context,
    /// as it is the only one that is guaranteed to never be unloaded or destroyed.
    /// </summary>
    public class ApplicationContext : Context
    {
        /// <summary>
        /// The project settings have the ApplicationContext run before anything else.
        /// We setup the initial DI container here.
        /// </summary>
        private void Awake()
        {
            ContainerBuilder builder = new();

            // Setup diagnostics if necessary.
            if (VContainerSettings.DiagnosticsEnabled)
                builder.Diagnostics = DiagnositcsContext.GetCollector("Application Context");

            // Register this component.
            builder.RegisterComponent(this);
            builder.ApplicationOrigin = this;
            
            var list = ListPool<AutoInject>.Get();
            GetInjectables(list);
            foreach (var injectable in list)
                injectable.Build(builder);
            Container = builder.Build();
            foreach (var injectable in list)
                injectable.Inject(Container);
            ListPool<AutoInject>.Release(list);
        }

        private void OnDestroy()
        {
            // Dispose the container when this context destroyed.
            Container.Dispose();
        }

        internal override void GetInjectables(List<AutoInject> injectables)
        {
            gameObject.scene.GetComponents(injectables);
        }
    }
}