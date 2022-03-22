using JetBrains.Annotations;
using UnityEngine;
using VContainer;

namespace Aurabox.VContainer
{
    public abstract class MonoInstaller : MonoBehaviour
    {
        public virtual void Build([PublicAPI] IContainerBuilder builder) { }
    }
}