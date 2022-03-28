using UnityEngine;

namespace Aurabox
{
    public class CanvasFader : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _canvasGroup = null!;

        public float Alpha
        {
            get => _canvasGroup.alpha;
            set => _canvasGroup.alpha = value;
        }
    }
}