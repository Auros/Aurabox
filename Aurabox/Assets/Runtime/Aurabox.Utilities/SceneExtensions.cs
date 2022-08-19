using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace Aurabox.Utilities
{
    /// <summary>
    /// Some useful extension methods related to scenes.
    /// </summary>
    public static class SceneExtensions
    {
        public static void GetComponents<T>(this Scene scene, List<T> components) where T : Component
        {
            if (!scene.IsValid())
                return;

            var gameObjects = ListPool<GameObject>.Get();
            scene.GetRootGameObjects(gameObjects);
            
            void AddComponents(Transform transform)
            {
                transform.GetComponents(components);
                for (int i = 0; i < transform.childCount; i++)
                    AddComponents(transform.GetChild(i));
            }
            
            foreach (var gameObject in gameObjects)
                AddComponents(gameObject.transform);
            
            ListPool<GameObject>.Release(gameObjects);
        }

        public static T? GetComponent<T>(this Scene scene) where T : Component
        {
            var gameObjects = ListPool<GameObject>.Get();
            scene.GetRootGameObjects(gameObjects);

            T? FindComponent(Transform transform)
            {
                if (transform.TryGetComponent<T>(out var component))
                    return component;

                for (int i = 0; i < transform.childCount; i++)
                {
                    component = FindComponent(transform.GetChild(i));
                    if (component != null)
                        return component;
                }

                return null;
            }

            foreach (var gameObject in gameObjects)
            {
                var component = FindComponent(gameObject.transform);
                if (component == null)
                    continue;
                
                ListPool<GameObject>.Release(gameObjects);
                return component;
            }
            
            ListPool<GameObject>.Release(gameObjects);
            return null;
        }
    }
}