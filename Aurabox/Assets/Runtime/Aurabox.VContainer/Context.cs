using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Aurabox.VContainer
{
    public abstract class Context : MonoBehaviour
    {
        public IObjectResolver Container { get; internal set; } = null!;

        /// <summary>
        /// Gets the injectables associated with this context.
        /// </summary>
        /// <returns></returns>
        internal abstract void GetInjectables(List<AutoInject> injectables);
    }
}