using CosmicBlock.Board;
using UnityEngine;
namespace CosmicBlock.Core {
 public enum GameState {Prototype}
 public sealed class GameSession:MonoBehaviour {
  [SerializeField] private BoardView board;

  [SerializeField] private CosmicBlock.Blocks.BlockPiece[] slots;
  [SerializeField] private RectTransform dragLayer;
  [SerializeField, Min(0)] private float dragFingerOffset = 110;
  [SerializeField] private bool useFixedSeed;
  [SerializeField] private int fixedSeed = 1;
  private CosmicBlock.Blocks.BlockDragHandler activeDrag;
  public float DragFingerOffset => dragFingerOffset;
  public void ConfigureBlocks(CosmicBlock.Blocks.BlockPiece[] pieces, RectTransform layer) { slots=pieces; dragLayer=layer; }
  public bool TryBeginDrag(CosmicBlock.Blocks.BlockDragHandler handler) {
   if(activeDrag != null) return false;
   activeDrag=handler; return true;
  }
  public void ReleaseDrag(CosmicBlock.Blocks.BlockDragHandler handler) { if(activeDrag==handler)activeDrag=null; }
  private void Start() {
   if(slots==null || slots.Length==0) return; // Preserve the original board-only scene/probe.
   var generator=new CosmicBlock.Blocks.BlockGenerator(useFixedSeed?(int?)fixedSeed:null);
   foreach(var piece in slots) piece.Initialize(generator.Next(),board,dragLayer,this);
  }
  public GameState State=>GameState.Prototype;
  public BoardModel Model {get;private set;}
  public void Configure(BoardView view)=>board=view;
  private void Awake() { Model=new BoardModel(); board.Bind(Model); }
 }
}
