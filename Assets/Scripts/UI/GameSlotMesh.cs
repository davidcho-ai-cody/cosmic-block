using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI {
 // Image remains the original Slot hit target; only its PNG geometry is suppressed.
 public sealed class GameSlotMesh:BaseMeshEffect {
  [SerializeField] HomeQuitMeshGraphic face;SlotVisual state;bool initialized,wasSelected;
  public void Configure(HomeQuitMeshGraphic value){face=value;}
  void LateUpdate(){if(face==null)return;if(state==null)state=GetComponent<SlotVisual>();bool selected=state!=null&&state.IsSelected;if(initialized&&wasSelected==selected)return;initialized=true;wasSelected=selected;face.Style(new Color(.025f,.055f,.13f,.88f),selected?SlotVisual.SelectedBorder:new Color(.24f,.63f,.85f,.75f),24,1.5f);}
  public override void ModifyMesh(VertexHelper mesh){if(IsActive())mesh.Clear();}
 }
}
