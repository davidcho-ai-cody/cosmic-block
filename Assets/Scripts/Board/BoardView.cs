using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.Board {
 [RequireComponent(typeof(RectTransform),typeof(GridLayoutGroup))]
 public sealed class BoardView:MonoBehaviour {
  [SerializeField] private Image[] cells;
  public BoardModel Model {get;private set;}
  public int CellCount=>cells==null?0:cells.Length;
  public void Configure(Image[] images)=>cells=images;
  public void Bind(BoardModel model) {
   if(Model!=null)Model.CellChanged-=Refresh;
   Model=model; Model.CellChanged+=Refresh;
   for(int y=0;y<8;y++)for(int x=0;x<8;x++)Refresh(x,y,Model.IsOccupied(x,y));
  }
  private void Refresh(int x,int y,bool occupied) { cells[y*8+x].color=occupied?new Color(.43f,.48f,.85f):new Color(.22f,.27f,.47f,.72f); }
  private void LateUpdate() {
   var grid=GetComponent<GridLayoutGroup>();
   float size=Mathf.Max(0,(((RectTransform)transform).rect.width-grid.padding.horizontal-grid.spacing.x*7)/8);
   grid.cellSize=new Vector2(size,size);
  }
  private void OnDestroy() { if(Model!=null)Model.CellChanged-=Refresh; }
 }
}
