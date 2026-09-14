using CosmicBlock.Board;
using CosmicBlock.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CosmicBlock.Blocks
{
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class BlockDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, ICancelHandler
    {
        private BoardView board;
        private RectTransform layer;
        private GameSession session;
        private BlockPiece piece;
        private Transform slot;
        private int sibling, pointerId;
        private bool hasAnchor, pendingReturn;
        private Vector2Int anchor;
        public bool IsDragging { get; private set; }

        public void Configure(BoardView view, RectTransform dragLayer, GameSession owner)
        { board = view; layer = dragLayer; session = owner; piece = GetComponent<BlockPiece>(); }
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (IsDragging || piece == null || piece.IsConsumed || eventData.button != PointerEventData.InputButton.Left ||
                session == null) return;
            if (pendingReturn) RestoreToSlot();
            if (!session.TryBeginDrag(this)) return;
            IsDragging = true; pointerId = eventData.pointerId;
            slot = transform.parent; sibling = transform.GetSiblingIndex();
            transform.SetParent(layer, false);
            piece.Rect.anchorMin = piece.Rect.anchorMax = new Vector2(.5f, .5f);
            transform.SetAsLastSibling();
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            EventSystem.current?.SetSelectedGameObject(gameObject);
            Move(eventData);
        }
        public void OnDrag(PointerEventData eventData)
        { if (IsDragging && eventData.pointerId == pointerId) Move(eventData); }
        private void Move(PointerEventData eventData)
        {
            Canvas.ForceUpdateCanvases();
            var canvas = layer.GetComponentInParent<Canvas>();
            Camera camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(layer, eventData.position, camera, out var local))
            { hasAnchor = false; board.ClearPreview(); return; }
            piece.SetGeometry(board.CellSize, board.CellSpacing);
            piece.Rect.anchoredPosition = local + new Vector2(0, session.DragFingerOffset);
            Vector2 originScreen = RectTransformUtility.WorldToScreenPoint(camera, piece.OriginWorld);
            hasAnchor = board.TryScreenToCell(originScreen, camera, out anchor);
            if (!hasAnchor) { board.ClearPreview(); return; }
            // Snap the visual origin and preview to the exact same nearest cell.
            piece.Rect.position += board.GetCellWorld(anchor) - piece.OriginWorld;
            board.ShowPreview(piece.Shape, anchor, board.Model.CanPlace(piece.Shape, anchor.x, anchor.y));
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            if (!IsDragging || eventData.pointerId != pointerId) return;
            Move(eventData);
            board.ClearPreview();
            bool placed = hasAnchor && board.Model.TryPlace(piece.Shape, anchor.x, anchor.y);
            Finish(placed);
        }
        public void OnCancel(BaseEventData eventData) => CancelDrag();
        public void CancelDrag()
        { if (IsDragging) { if (board != null) board.ClearPreview(); Finish(false); } }
        private void Finish(bool placed, bool deferReturn = false)
        {
            IsDragging = false; hasAnchor = false;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            if (session != null) session.ReleaseDrag(this);
            if (deferReturn)
            {
                pendingReturn = true;
                if (session != null && session.isActiveAndEnabled) session.StartCoroutine(ReturnNextFrame());
            }
            else RestoreToSlot();
            if (placed) piece.Consume();

        }
        private System.Collections.IEnumerator ReturnNextFrame()
        {
            yield return null;
            if (this != null && pendingReturn) RestoreToSlot();
        }
        private void RestoreToSlot()
        {
            pendingReturn = false;
            if (slot != null)
            {
                transform.SetParent(slot, false); transform.SetSiblingIndex(sibling);
                piece.Rect.anchorMin = piece.Rect.anchorMax = new Vector2(.5f, .5f);
                piece.Rect.anchoredPosition = Vector2.zero;
                piece.FitSlot();
            }

        }
        private void OnApplicationFocus(bool focused) { if (!focused) CancelDrag(); }
        private void OnApplicationPause(bool paused) { if (paused) CancelDrag(); }
        private void OnDisable()
        {
            if (!IsDragging) return;
            if (board != null) board.ClearPreview();
            // Unity forbids SetParent inside an activation/deactivation callback.
            Finish(false, true);
        }
    }
}
