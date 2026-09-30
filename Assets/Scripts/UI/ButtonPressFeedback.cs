using UnityEngine;
using UnityEngine.EventSystems;

namespace CosmicBlock.UI
{
    public sealed class ButtonPressFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField, Range(.9f, 1f)] float pressedScale = .975f;
        RectTransform rect;
        void Awake() => rect = transform as RectTransform;
        public void OnPointerDown(PointerEventData eventData) { if (rect != null) rect.localScale = Vector3.one * pressedScale; }
        public void OnPointerUp(PointerEventData eventData) => ResetScale();
        public void OnPointerExit(PointerEventData eventData) => ResetScale();
        void OnDisable() => ResetScale();
        void ResetScale() { if (rect == null) rect = transform as RectTransform; if (rect != null) rect.localScale = Vector3.one; }
    }
}
