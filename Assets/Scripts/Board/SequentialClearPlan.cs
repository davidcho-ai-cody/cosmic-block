using System.Collections.Generic;
using UnityEngine;
namespace CosmicBlock.Board {
 public sealed class ClearLineStep {
  public bool IsRow {get;} public int Coordinate {get;} public IReadOnlyList<Vector2Int> Cells {get;}
  public Vector2Int Representative => IsRow ? new Vector2Int(0,Coordinate) : new Vector2Int(Coordinate,0);
  public ClearLineStep(bool row,int coordinate,List<Vector2Int> cells){IsRow=row;Coordinate=coordinate;Cells=cells.ToArray();}
  public LineClearResult Result => new LineClearResult(IsRow?new List<int>{Coordinate}:new List<int>(),IsRow?new List<int>():new List<int>{Coordinate},new List<Vector2Int>(Cells));
 }
 public sealed class SequentialClearPlan {
  public IReadOnlyList<ClearLineStep> Steps {get;}
  public SequentialClearPlan(LineClearResult snapshot){var lines=new List<ClearLineStep>();foreach(int y in snapshot.ClearedRows)lines.Add(new ClearLineStep(true,y,new List<Vector2Int>()));foreach(int x in snapshot.ClearedColumns)lines.Add(new ClearLineStep(false,x,new List<Vector2Int>()));
   // Board X points right, Y points down. Sort top-left representatives by Y, X,
   // then stable Row-before-Column for the (0,0) tie. Never enumerate a HashSet.
   lines.Sort((a,b)=>{int n=a.Representative.y.CompareTo(b.Representative.y);if(n==0)n=a.Representative.x.CompareTo(b.Representative.x);return n!=0?n:(a.IsRow?0:1).CompareTo(b.IsRow?0:1);});var seen=new HashSet<Vector2Int>();var result=new List<ClearLineStep>();foreach(var line in lines){var cells=new List<Vector2Int>();for(int i=0;i<8;i++){var cell=line.IsRow?new Vector2Int(i,line.Coordinate):new Vector2Int(line.Coordinate,i);if(seen.Add(cell))cells.Add(cell);}result.Add(new ClearLineStep(line.IsRow,line.Coordinate,cells));}Steps=result.ToArray();
  }
 }
}
