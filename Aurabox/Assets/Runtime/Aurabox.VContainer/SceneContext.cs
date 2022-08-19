using System;
using System.Collections.Generic;
using Aurabox.Utilities;
using UnityEngine.Pool;
using VContainer;
using VContainer.Diagnostics;
using VContainer.Unity;

namespace Aurabox.VContainer
{
    public class SceneContext : Context
    {
        private void Start()
        {
            // If the container has not been assigned, create one.
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (Container == null)
            {
                ContainerBuilder builder = new();
                BuildAndSetupContainer(builder);
                Container = builder.Build();
            }
            else // If the container was assigned at this point, that means it's its a parent container assigned by some secondary system
            {
                var parentContainer = Container;
                Container = parentContainer.CreateScope(BuildAndSetupContainer);
            }
        }

        private void BuildAndSetupContainer(IContainerBuilder builder)
        {
            if (VContainerSettings.DiagnosticsEnabled)
                builder.Diagnostics = DiagnositcsContext.GetCollector($"Scene Context - [{gameObject.scene.name}]");

            //builder.ApplicationOrigin = this;
            var list = ListPool<AutoInject>.Get();
            GetInjectables(list);
                    
            foreach (var injectable in list)
                injectable.Build(builder);

            builder.RegisterComponent(this);
            builder.RegisterBuildCallback(container =>
            {
                foreach (var injectable in list)
                    injectable.Inject(container);
                ListPool<AutoInject>.Release(list);
            });
        }

        internal override void GetInjectables(List<AutoInject> injectables)
        {
            gameObject.scene.GetComponents(injectables);
        }

        private void OnDestroy()
        {
            // Dispose the container when this context destroyed.
            Container.Dispose();
        }
    }
}