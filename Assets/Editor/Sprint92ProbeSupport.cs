using CosmicBlock.Core;
using CosmicBlock.Blocks;
public static class Sprint92ProbeSupport {
 public static bool PlaceImmediate(GameSession session,BlockPiece piece,int x,int y){bool result=session.TryPlacePiece(piece,x,y);if(result)session.DebugCompleteTurnForProbe();return result;}
}
