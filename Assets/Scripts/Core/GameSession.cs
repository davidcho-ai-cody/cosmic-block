using CosmicBlock.Board;
using UnityEngine;
namespace CosmicBlock.Core {
 public enum GameState {Prototype}
 public sealed class GameSession:MonoBehaviour {
  [SerializeField] private BoardView board;
  public GameState State=>GameState.Prototype;
  public BoardModel Model {get;private set;}
  public void Configure(BoardView view)=>board=view;
  private void Awake() { Model=new BoardModel(); board.Bind(Model); }
 }
}
