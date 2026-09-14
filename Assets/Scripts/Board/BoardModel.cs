using System;
namespace CosmicBlock.Board {
 public sealed class BoardModel {
  public const int Size=8;
  private readonly bool[,] cells=new bool[Size,Size];
  public event Action<int,int,bool> CellChanged;
  public bool IsOccupied(int x,int y) { Check(x,y); return cells[x,y]; }
  public void SetOccupied(int x,int y,bool value) {
   Check(x,y); if(cells[x,y]==value)return;
   cells[x,y]=value; CellChanged?.Invoke(x,y,value);
  }

  // Validate every offset before any board mutation. Failed placements are atomic.
  public bool CanPlace(CosmicBlock.Blocks.BlockShape shape, int x, int y) {
   if(shape == null) return false;
   foreach(var offset in shape.Cells) {
    long cx = (long)x + offset.x, cy = (long)y + offset.y;
    if(cx < 0 || cy < 0 || cx >= Size || cy >= Size || cells[(int)cx,(int)cy]) return false;
   }
   return true;
  }
  public bool TryPlace(CosmicBlock.Blocks.BlockShape shape, int x, int y) {
   if(!CanPlace(shape,x,y)) return false;
   foreach(var offset in shape.Cells) cells[x+offset.x,y+offset.y] = true;
   foreach(var offset in shape.Cells) CellChanged?.Invoke(x+offset.x,y+offset.y,true);
   return true;
  }

  public bool CanPlaceAnywhere(CosmicBlock.Blocks.BlockShape shape) {
   for(int y=0;y<Size;y++) for(int x=0;x<Size;x++) if(CanPlace(shape,x,y))return true;
   return false;
  }
  public LineClearResult ClearCompletedLines() {
   var rows=new System.Collections.Generic.List<int>();
   var columns=new System.Collections.Generic.List<int>();
   var unique=new System.Collections.Generic.List<UnityEngine.Vector2Int>();
   var fullRows=new bool[Size]; var fullColumns=new bool[Size];
   // Collect both dimensions before clearing anything.
   for(int i=0;i<Size;i++) {
    bool row=true, column=true;
    for(int j=0;j<Size;j++) { row &= cells[j,i]; column &= cells[i,j]; }
    if(row) { fullRows[i]=true; rows.Add(i); }
    if(column) { fullColumns[i]=true; columns.Add(i); }
   }
   for(int y=0;y<Size;y++) for(int x=0;x<Size;x++) {
    if(!fullRows[y] && !fullColumns[x])continue;
    cells[x,y]=false; unique.Add(new UnityEngine.Vector2Int(x,y));
   }
   foreach(var cell in unique) CellChanged?.Invoke(cell.x,cell.y,false);
   return new LineClearResult(rows,columns,unique);
  }
  public void Clear() { for(int y=0;y<8;y++)for(int x=0;x<8;x++)SetOccupied(x,y,false); }
  private static void Check(int x,int y) { if(x<0||y<0||x>=8||y>=8)throw new ArgumentOutOfRangeException(nameof(x)); }
 }
}
