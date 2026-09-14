using CosmicBlock.Board;
using CosmicBlock.Core;
using UnityEngine;
using UnityEngine.UI;

namespace CosmicBlock.Blocks
{
    [RequireComponent(typeof(RectTransform), typeof(Image), typeof(CanvasGroup))]
    [RequireComponent(typeof(BlockDragHandler))]
    public sealed class BlockPiece : MonoBehaviour
    {
        private Image[] visuals;
        private float cellSize;
        public BlockShape Shape { get; private set; }
        public bool IsConsumed { get; private set; }
        public RectTransform Rect => (RectTransform)transform;
        public Vector3 OriginWorld => Rect.TransformPoint(new Vector3(Rect.rect.xMin + cellSize / 2,
                                                                      Rect.rect.yMax - cellSize / 2));
        public void Initialize(BlockShape shape, BoardView board, RectTransform dragLayer, GameSession session)
        {
            GetComponent<BlockDragHandler>().ResetForReuse();
            if (visuals != null) foreach (var visual in visuals)
            {
                if (visual == null) continue;
                visual.gameObject.SetActive(false);
                Destroy(visual.gameObject);
            }
            Shape = shape; IsConsumed = false;
            GetComponent<Image>().color = new Color(0, 0, 0, .001f);
            GetComponent<Image>().raycastTarget = true;
            visuals = new Image[shape.Cells.Count];
            for (int i = 0; i < visuals.Length; i++)
            {
                var rect = new GameObject("BlockCell_" + i, typeof(RectTransform)).GetComponent<RectTransform>();
                rect.SetParent(transform, false);
                rect.anchorMin = rect.anchorMax = new Vector2(0, 1);
                visuals[i] = rect.gameObject.AddComponent<Image>();
                visuals[i].color = new Color(.43f, .48f, .85f);
                visuals[i].raycastTarget = false;
            }
            GetComponent<BlockDragHandler>().Configure(board, dragLayer, session);
            gameObject.SetActive(true);
            FitSlot();
        }
        public void SetGeometry(float size, float spacing)
        {
            cellSize = size;
            float pitch = size + spacing;
            Rect.sizeDelta = new Vector2(Shape.Width * pitch - spacing, Shape.Height * pitch - spacing);
            for (int i = 0; i < visuals.Length; i++)
            {
                var rect = (RectTransform)visuals[i].transform;
                rect.sizeDelta = new Vector2(size, size);
                rect.anchoredPosition = new Vector2(Shape.Cells[i].x * pitch + size / 2,
                                                   -Shape.Cells[i].y * pitch - size / 2);
            }
        }
        public void FitSlot()
        {
            if (Shape == null || !(transform.parent is RectTransform slot)) return;
            float size = Mathf.Max(1, Mathf.Min(64, slot.rect.width * .24f, slot.rect.height * .24f));
            SetGeometry(size, 6);
        }
        private void LateUpdate()
        {
            if (Shape != null && !GetComponent<BlockDragHandler>().IsDragging) FitSlot();
        }
        public void Consume()
        {
            IsConsumed = true;
            gameObject.SetActive(false);
        }
    }
}
