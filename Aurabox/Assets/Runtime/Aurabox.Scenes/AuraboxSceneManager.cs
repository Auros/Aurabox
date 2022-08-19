using Aurabox.Utilities;
using Aurabox.VContainer;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Aurabox.Scenes
{
    public class AuraboxSceneManager : MonoBehaviour
    {
        [Inject]
        private readonly ApplicationContext _applicationContext = null!;

        [SerializeField]
        private string _initialScene = string.Empty;
        
        private void Awake()
        {
            if (!SceneManager.GetSceneByName(_initialScene).isLoaded)
                SceneManager.LoadScene(_initialScene, LoadSceneMode.Additive);
            
            var scene = SceneManager.GetSceneByName(_initialScene);
            var sceneContext = scene.GetComponent<SceneContext>();
            if (sceneContext == null)
                return;
            
            sceneContext.Container = _applicationContext.Container;
            sceneContext.gameObject.SetActive(true);
        }
    }
}