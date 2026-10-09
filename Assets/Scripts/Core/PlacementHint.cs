using System.Collections.Generic;
using CosmicBlock.Blocks;
using CosmicBlock.Board;
using UnityEngine;
namespace CosmicBlock.Core {
 public sealed class PlacementHint {
  public int Slot,Lines,ClearedCells,EmptyCells,FuturePlacements;public Vector2Int Anchor;public BlockShape Shape;
  // Lexicographic, deterministic ranking; slot/y/x order breaks exact ties.
  public static PlacementHint Find(BoardModel board,IReadOnlyList<BlockPiece> slots){PlacementHint best=null;
   for(int slot=0;slot<slots.Count;slot++){var piece=slots[slot];if(piece==null||piece.IsConsumed)continue;
    for(int y=0;y<8;y++)for(int x=0;x<8;x++){if(!board.CanPlace(piece.Shape,x,y))continue;var copy=new BoardModel();for(int cy=0;cy<8;cy++)for(int cx=0;cx<8;cx++)if(board.IsOccupied(cx,cy))copy.SetOccupied(cx,cy,true);copy.TryPlace(piece.Shape,x,y);var clear=copy.ClearCompletedLines();int empty=0,future=0;for(int cy=0;cy<8;cy++)for(int cx=0;cx<8;cx++)if(!copy.IsOccupied(cx,cy))empty++;
     if(best!=null&&(clear.LineCount<best.Lines||(clear.LineCount==best.Lines&&(clear.UniqueClearedCells.Count<best.ClearedCells||(clear.UniqueClearedCells.Count==best.ClearedCells&&empty<best.EmptyCells)))))continue;
     foreach(var shape in BlockCatalog.Shapes)for(int py=0;py<8;py++)for(int px=0;px<8;px++)if(copy.CanPlace(shape,px,py))future++;
     var candidate=new PlacementHint{Slot=slot,Anchor=new Vector2Int(x,y),Shape=piece.Shape,Lines=clear.LineCount,ClearedCells=clear.UniqueClearedCells.Count,EmptyCells=empty,FuturePlacements=future};if(Better(candidate,best))best=candidate;
    }
   }return best;
  }
  static bool Better(PlacementHint a,PlacementHint b)=>b==null||a.Lines>b.Lines||(a.Lines==b.Lines&&(a.ClearedCells>b.ClearedCells||(a.ClearedCells==b.ClearedCells&&(a.EmptyCells>b.EmptyCells||(a.EmptyCells==b.EmptyCells&&a.FuturePlacements>b.FuturePlacements)))));
 }
}
