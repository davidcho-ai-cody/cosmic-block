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
        private static int paletteIndex;
        private static readonly Color[] Palette = { new Color(.30f,.58f,.96f), new Color(.60f,.42f,.91f), new Color(.94f,.69f,.30f) };
        private Image[] visuals;
        private Outline[] cellOutlines;
        private float cellSize;
        public static readonly Color DragOutlineColor = new Color(1f, .76f, .28f, .98f);
        private static readonly Color RestOutlineColor = new Color(1f, 1f, 1f, .20f);
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
            cellOutlines = new Outline[shape.Cells.Count];
            for (int i = 0; i < visuals.Length; i++)
            {
                var rect = new GameObject("BlockCell_" + i, typeof(RectTransform)).GetComponent<RectTransform>();
                rect.SetParent(transform, false);
                rect.anchorMin = rect.anchorMax = new Vector2(0, 1);
                visuals[i] = rect.gameObject.AddComponent<Image>();
                visuals[i].color = Palette[paletteIndex % Palette.Length];
                visuals[i].raycastTarget = false;
                var highlight = rect.gameObject.AddComponent<Outline>();
                highlight.effectColor = RestOutlineColor;
                highlight.effectDistance = new Vector2(-1.5f, 1.5f);
                highlight.useGraphicAlpha = true;
                var shade = rect.gameObject.AddComponent<Shadow>();
                shade.effectColor = new Color(.02f, .03f, .10f, .28f);
                shade.effectDistance = new Vector2(2, -2);
                shade.useGraphicAlpha = true;
                cellOutlines[i] = highlight;
            }
            GetComponent<BlockDragHandler>().Configure(board, dragLayer, session);
            paletteIndex++;
            gameObject.SetActive(true);
            FitSlot();
        }
        public void SetDraggingVisual(bool dragging)
        {
            if (cellOutlines == null) return;
            foreach (var outline in cellOutlines)
            {
                if (outline == null) continue;
                outline.effectColor = dragging ? DragOutlineColor : RestOutlineColor;
                outline.effectDistance = dragging ? new Vector2(2.5f, -2.5f) : new Vector2(-1.5f, 1.5f);
                outline.useGraphicAlpha = !dragging;
            }
        }
        public bool DragHighlightActive
        {
            get
            {
                if (cellOutlines == null || cellOutlines.Length == 0) return false;
                foreach (var outline in cellOutlines)
                    if (outline == null || outline.effectColor != DragOutlineColor) return false;
                return true;
            }
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
            SetDraggingVisual(false);
            IsConsumed = true;
            gameObject.SetActive(false);
        }
    }
}
