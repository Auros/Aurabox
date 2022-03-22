using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using VContainer;
using VContainer.Diagnostics;
using VContainer.Unity;

namespace Aurabox.VContainer
{
    // We want this to be the first thing that executes in the scene.
    [DefaultExecutionOrder(-9999)]
    public class SceneContext : MonoBehaviour
    {
        private bool _initialized;
        private bool _initializing;
        private readonly List<AutoInject> _autoInjects = new();
        private readonly List<(GameObject, bool)> _rootObjectStates = new();

        [SerializeField]
        protected bool _standalone;

        [SerializeField]
        protected MonoInstaller[] _monoInstallers = null!;

        [PublicAPI]
        public event Action<SceneContext>? PostResolved;
        public bool Standalone => _standalone;

        private IObjectResolver? _container;
        public IObjectResolver Container
        {
            get
            {
                if (_container is null)
                    throw new InvalidOperationException("The container has not been initialized.");
                return _container;
            }
        }

        private void Awake()
        {
            // The first thing we want to do is ensure that nothing is enabled in the scene.
            foreach (var obj in gameObject.scene.GetRootGameObjects())
            {
                _autoInjects.AddRange(obj.GetComponentsInChildren<AutoInject>());
                if (_standalone) continue;
                
                // We want to store the activity state of every root game object, then disable it while the container resolves (for multi-scene loading).
                _rootObjectStates.Add((obj, obj.activeSelf));
                obj.SetActive(false);
            }

            if (!_standalone) return;
            
            // As this is a standalone context, it builds its own container.
            ContainerBuilder builder = new()
            {
                ApplicationOrigin = this,
                Diagnostics = VContainerSettings.DiagnosticsEnabled ? DiagnositcsContext.GetCollector("Root Context: " + gameObject.scene.name) : null!
            };
            builder.RegisterComponent(this);
            Install(builder);
            builder.Build();
        }

        public void Install(IContainerBuilder containerBuilder)
        {
            // Bore starting the initialization process, lets make sure this context hasn't been initialized.
            if (_initializing || _initialized) return;
            
            _initializing = true;

            // For every mono installer we setup their configuration step.
            foreach (var installer in _monoInstallers)
                installer.Build(containerBuilder);

            // Now for all the auto injectors we've collected, we register 
            foreach (var autoInject in _autoInjects)
            foreach (var registers in autoInject.SingleComponents)
                containerBuilder.RegisterInstance(registers).As(registers.GetType());

            containerBuilder.RegisterBuildCallback(container =>
            {
                // If this scene context has already been initialized for some reason, do nothing on callback.
                if (_initialized)
                    return;

                // Mark the container as complete and
                // supply the container.
                _initialized = true;
                _initializing = false;
                _container = container;

                // Inject every gameobject so it has its dependencies.
                foreach (var autoInject in _autoInjects)
                    container.InjectGameObject(autoInject.gameObject);

                if (!_standalone)
                {
                    // Reenable the objects that were disabled before.
                    foreach (var root in _rootObjectStates)
                        root.Item1.SetActive(root.Item2);
                }

                // I don't wanna hold onto these references.
                _rootObjectStates.Clear();

                PostResolved?.Invoke(this);
            });
        }
    }
}