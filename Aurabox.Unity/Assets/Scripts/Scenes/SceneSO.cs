using UnityEngine;

namespace Aurabox
{
    [CreateAssetMenu(fileName = "SceneName", menuName = "ScriptableObjects/Aurabox/SceneSO")]
    public class SceneSO : ScriptableObject
    {
        [field: SerializeField]
        public string Name { get; set; } = string.Empty;
    }
}