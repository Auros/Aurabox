using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Aurabox.VContainer
{
    public class AutoInject : MonoBehaviour
    {
        internal virtual void Build(IContainerBuilder builder) { }

        internal virtual void Inject(IObjectResolver container)
        {
            InjectGameObjectUnlessTypeOrBehaviours(container, gameObject, Array.Empty<MonoBehaviour>());
        }

        protected static void InjectGameObjectUnlessTypeOrBehaviours(IObjectResolver container, GameObject gameObject, MonoBehaviour[] behaviours)
        {
            var buffer = ListPool<MonoBehaviour>.Get();

            void InjectRecursive(GameObject current)
            {
                if (current == null)
                    return;
                
                buffer!.Clear();
                current.GetComponents(buffer);

                bool hasAutoInjectOrInstaller = false;
                for (var i = 0; i < buffer.Count; i++)
                {
                    var behaviour = buffer[i];
                    var isAutoInjectOrInstaller = behaviour is AutoInject or IInstaller;
                    
                    // ReSharper disable once ConvertIfStatementToSwitchStatement
                    if (isAutoInjectOrInstaller)
                        hasAutoInjectOrInstaller = true;
                    
                    if (!isAutoInjectOrInstaller && !Contains(behaviour, behaviours))
                        container.Inject(behaviour);
                }

                // If there was an AutoInject component *somewhere* on the current object, we don't inject its children.
                if (hasAutoInjectOrInstaller)
                    return;

                // Inject the gameobject's children.
                var transform = current.transform;
                for (int i = 0; i < transform.childCount; i++)
                {
                    var child = transform.GetChild(i);
                    InjectRecursive(child.gameObject);
                }
            }
            InjectRecursive(gameObject);
            
            ListPool<MonoBehaviour>.Release(buffer);
        }

        // Shorthand way to not allocate and to keep code tidier
        private static bool Contains(Object behaviour, IReadOnlyList<MonoBehaviour> behaviours)
        {
            for (int i = 0; i < behaviours.Count; i++)
                if (behaviours[i] == behaviour)
                    return true;
            return false;
        }
    }
}