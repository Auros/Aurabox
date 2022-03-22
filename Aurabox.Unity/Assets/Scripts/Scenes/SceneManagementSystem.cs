using System.Collections.Generic;
using Aurabox.VContainer;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Diagnostics;
using VContainer.Unity;

namespace Aurabox
{
    public class SceneManagementSystem : MonoBehaviour
    {
        [SerializeField]
        [InspectorName("Root Context")]
        protected SceneContext _sceneContext = null!;

        [SerializeField]
        protected SceneSO[] _decoratorScenes = null!;

        [SerializeField]
        protected SceneSO _defaultActiveScene = null!;

        private SceneLoadData? _activeScene;
        private IObjectResolver? _decoratorContainer;

        // Uplon loading into the scene, we start the loading of the decorator scenes.
        protected async UniTaskVoid Start()
        {
            await LoadDecoratorScenes();
            await SetActiveSceneAsync(_defaultActiveScene);
        }

        public void SetActiveScene(SceneSO sceneSO)
        {
            UniTask.Create(() => SetActiveSceneAsync(sceneSO));
        }

        public void Restart()
        {
            UniTask.Create(() => RestartAsync());
        }

        private async UniTask RestartAsync()
        {
            await UnloadActiveSceneAsync();
            await UnloadDecoratorScenes();
            await LoadDecoratorScenes();
            await SetActiveSceneAsync(_defaultActiveScene);
        }

        private async UniTask SetActiveSceneAsync(SceneSO sceneSO)
        {
            await UnloadActiveSceneAsync();
            var parentContainer = _decoratorContainer ?? _sceneContext.Container;

            var scene = SceneManager.GetSceneByName(sceneSO.Name);
            if (!scene.isLoaded)
            {
                await SceneManager.LoadSceneAsync(sceneSO.Name, LoadSceneMode.Additive);
                scene = SceneManager.GetSceneByName(sceneSO.Name);
            }

            var sceneContext = GetSceneContext(scene);
            if (sceneContext != null)
            {
                parentContainer.CreateScope(installation =>
                {
                    installation.ApplicationOrigin = this;
                    installation.Diagnostics = VContainerSettings.DiagnosticsEnabled ? DiagnositcsContext.GetCollector("Main Context: " + sceneSO.Name) : null!;
                    sceneContext.Install(installation);
                });
            }

            _activeScene = new SceneLoadData(scene, sceneContext);
        }

        private async UniTask UnloadActiveSceneAsync()
        {
            if (_activeScene is { Scene: { isLoaded: true }})
            {
                if (_activeScene.SceneContext != null)
                    _activeScene.SceneContext.Container.Dispose();
                await SceneManager.UnloadSceneAsync(_activeScene.Scene);
            }
            _activeScene = null;
        }

        private async UniTask LoadDecoratorScenes()
        {
            List<Scene> decoratorScenes = new();
            foreach (var sceneSO in _decoratorScenes)
            {
                var dScene = SceneManager.GetSceneByName(sceneSO.Name);
                if (!dScene.isLoaded)
                {
                    await SceneManager.LoadSceneAsync(sceneSO.Name, LoadSceneMode.Additive);
                    dScene = SceneManager.GetSceneByName(sceneSO.Name);
                }
                decoratorScenes.Add(dScene);
            }

            List<SceneContext> sceneContexts = new();
            foreach (var scene in decoratorScenes)
            {
                var sceneContext = GetSceneContext(scene);
                if (sceneContext != null && !sceneContext.Standalone)
                    sceneContexts.Add(sceneContext);
            }

            if (sceneContexts.Count != 0)
            {
                _decoratorContainer?.Dispose();
                _decoratorContainer = _sceneContext.Container.CreateScope(installation =>
                {
                    installation.ApplicationOrigin = this;
                    installation.Diagnostics = VContainerSettings.DiagnosticsEnabled ? DiagnositcsContext.GetCollector("Decorator Container") : null!;
                    foreach (var context in sceneContexts)
                        context.Install(installation);

                });
            }
        }

        private async UniTask UnloadDecoratorScenes()
        {
            List<Scene> loaded = new();
            foreach (var sceneSO in _decoratorScenes)
            {
                var dScene = SceneManager.GetSceneByName(sceneSO.Name);
                if (dScene.isLoaded)
                    loaded.Add(dScene);
            }

            _decoratorContainer?.Dispose();
            _decoratorContainer = null;

            foreach (var loadedScene in loaded)
                await SceneManager.UnloadSceneAsync(loadedScene);
        }

        private static SceneContext? GetSceneContext(Scene scene)
        {
            foreach (var obj in scene.GetRootGameObjects())
            {
                var context = obj.GetComponent<SceneContext>();
                if (context != null)
                    return context;
            }
            return null;
        }

        private class SceneLoadData
        {
            public Scene Scene { get; }
            public SceneContext? SceneContext { get; }

            public SceneLoadData(Scene scene, SceneContext? sceneContext = null)
            {
                Scene = scene;
                SceneContext = sceneContext;
            }
        }
    }
}
