using System;
using UnityEngine;

namespace Aurabox.VContainer
{
    [DisallowMultipleComponent]
    public class AutoInject : MonoBehaviour
    {
        [field: SerializeField]
        [field: InspectorName("Register")]
        public Component[] SingleComponents { get; set; } = Array.Empty<Component>();
    }
}
