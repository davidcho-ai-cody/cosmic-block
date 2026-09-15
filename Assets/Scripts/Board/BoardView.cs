using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.Board {
 [RequireComponent(typeof(RectTransform),typeof(GridLayoutGroup))]
 public sealed class BoardView:MonoBehaviour {
  [SerializeField] private Image[] cells;
  private readonly System.Collections.Generic.HashSet<int> preview = new System.Collections.Generic.HashSet<int>();
  private bool previewValid;
  public BoardModel Model {get;private set;}
  public int CellCount=>cells==null?0:cells.Length;
  public void Configure(Image[] images)=>cells=images;
  public void Bind(BoardModel model) {
   if(Model!=null)Model.CellChanged-=Refresh;
   Model=model; Model.CellChanged+=Refresh;
   for(int y=0;y<8;y++)for(int x=0;x<8;x++)Refresh(x,y,Model.IsOccupied(x,y));
  }
  private void Refresh(int x,int y,bool occupied) { cells[y*8+x].color=preview.Contains(y*8+x) ? (previewValid ? new Color(.96f,.73f,.3f,.92f) : new Color(.67f,.25f,.32f,.9f)) : occupied?new Color(.43f,.56f,.94f):new Color(.08f,.13f,.29f,.78f); }

  public float CellSize => GetComponent<GridLayoutGroup>().cellSize.x;
  public float CellSpacing => GetComponent<GridLayoutGroup>().spacing.x;
  public bool TryScreenToCell(Vector2 screen, Camera camera, out Vector2Int coordinate) {
   coordinate = default;
   var rect = (RectTransform)transform;
   if(cells == null || cells.Length != 64 ||
      !RectTransformUtility.ScreenPointToLocalPointInRectangle(rect,screen,camera,out var local) ||
      !rect.rect.Contains(local)) return false;
   Vector3 first = cells[0].transform.localPosition;
   var grid = GetComponent<GridLayoutGroup>();
   float pitchX = grid.cellSize.x + grid.spacing.x, pitchY = grid.cellSize.y + grid.spacing.y;
   if(pitchX <= 0 || pitchY <= 0) return false;
   coordinate = new Vector2Int(Mathf.RoundToInt((local.x-first.x)/pitchX),
                             Mathf.RoundToInt((first.y-local.y)/pitchY));
   return true;
  }
  public Vector3 GetCellWorld(Vector2Int coordinate) {
   var grid = GetComponent<GridLayoutGroup>();
   return transform.TransformPoint(cells[0].transform.localPosition +
      new Vector3(coordinate.x*(grid.cellSize.x+grid.spacing.x),
                  -coordinate.y*(grid.cellSize.y+grid.spacing.y),0));
  }
  public void ShowPreview(CosmicBlock.Blocks.BlockShape shape, Vector2Int anchor, bool valid) {
   ClearPreview(); previewValid = valid;
   foreach(var offset in shape.Cells) {
    int x=anchor.x+offset.x, y=anchor.y+offset.y;
    if(x<0 || y<0 || x>=8 || y>=8) continue;
    preview.Add(y*8+x); Refresh(x,y,Model.IsOccupied(x,y));
   }
  }
  public void ClearPreview() {
   preview.Clear();
   if(Model == null) return;
   for(int y=0;y<8;y++) for(int x=0;x<8;x++) Refresh(x,y,Model.IsOccupied(x,y));
  }
  private void LateUpdate() {
   var grid=GetComponent<GridLayoutGroup>();
   float size=Mathf.Max(0,(((RectTransform)transform).rect.width-grid.padding.horizontal-grid.spacing.x*7)/8);
   grid.cellSize=new Vector2(size,size);
  }
  private void OnDestroy() { if(Model!=null)Model.CellChanged-=Refresh; }
 }
}
