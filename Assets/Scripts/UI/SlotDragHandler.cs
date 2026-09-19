using CosmicBlock.Blocks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CosmicBlock.UI
{
    [RequireComponent(typeof(Image))]
    public sealed class SlotDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, ICancelHandler
    {
        BlockPiece piece;
        BlockDragHandler drag;
        Image hitArea;
        bool forwarding;

        public bool IsInteractable => hitArea != null && hitArea.raycastTarget && piece != null && piece.gameObject.activeInHierarchy && !piece.IsConsumed;

        void Awake()
        {
            hitArea = GetComponent<Image>();
            Configure(GetComponentInChildren<BlockPiece>(true));
        }

        public void Configure(BlockPiece slotPiece)
        {
            piece = slotPiece;
            drag = piece == null ? null : piece.GetComponent<BlockDragHandler>();
            SetInteractable(piece != null && piece.gameObject.activeSelf && !piece.IsConsumed);
        }

        public void SetInteractable(bool value)
        {
            if (hitArea == null) hitArea = GetComponent<Image>();
            hitArea.raycastTarget = value;
            if (!value) forwarding = false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!IsInteractable || drag == null) return;
            drag.OnBeginDrag(eventData);
            forwarding = drag.IsDragging;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (forwarding && drag != null) drag.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!forwarding || drag == null) return;
            drag.OnEndDrag(eventData);
            forwarding = false;
        }

        public void OnCancel(BaseEventData eventData)
        {
            if (forwarding && drag != null) drag.OnCancel(eventData);
            forwarding = false;
        }
    }
}
