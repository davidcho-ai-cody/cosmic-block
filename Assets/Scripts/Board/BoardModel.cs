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
  public void Clear() { for(int y=0;y<8;y++)for(int x=0;x<8;x++)SetOccupied(x,y,false); }
  private static void Check(int x,int y) { if(x<0||y<0||x>=8||y>=8)throw new ArgumentOutOfRangeException(nameof(x)); }
 }
}
