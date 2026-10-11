using CosmicBlock.Board;
using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI {
 // Visual-only replacement for the empty-cell art; original Image state remains for preview.
 public sealed class GameEmptyCellMesh:BaseMeshEffect {
  [SerializeField] HomeQuitMeshGraphic face;Image image;Color lastFill,lastEdge;float lastWidth=-1;
  public void Configure(HomeQuitMeshGraphic value){face=value;}
  void LateUpdate(){if(image==null)image=GetComponent<Image>();if(face==null)return;bool empty=image.sprite!=null&&image.sprite.name=="game_empty_cell";face.gameObject.SetActive(empty);if(!empty)return;
   bool valid=image.color==BoardView.ValidFill,invalid=image.color==BoardView.InvalidFill;
   var fill=valid?BoardView.ValidFill:invalid?BoardView.InvalidFill:new Color(.025f,.065f,.14f,.98f);
   var edge=valid?BoardView.ValidBorder:invalid?BoardView.InvalidBorder:new Color(.20f,.40f,.58f,.7f);
   float width=face.rectTransform.rect.width;if(fill!=lastFill||edge!=lastEdge||!Mathf.Approximately(width,lastWidth)){face.Style(fill,edge,width*.11f,1);lastFill=fill;lastEdge=edge;lastWidth=width;}face.raycastTarget=false;
  }
  public override void ModifyMesh(VertexHelper mesh){if(!IsActive())return;var i=GetComponent<Image>();if(i.sprite!=null&&i.sprite.name=="game_empty_cell")mesh.Clear();}
 }
}
