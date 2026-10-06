using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI {
 // Expand artwork around its existing logical cell center; no Rect or input geometry changes.
 public sealed class CellSpriteDensity : BaseMeshEffect {
  [SerializeField] float visualScale=1;
  public void Configure(float scale){visualScale=scale;if(graphic!=null)graphic.SetVerticesDirty();}
  public override void ModifyMesh(VertexHelper mesh){if(!IsActive())return;var center=graphic.GetPixelAdjustedRect().center;var vertex=new UIVertex();for(int i=0;i<mesh.currentVertCount;i++){mesh.PopulateUIVertex(ref vertex,i);var p=vertex.position;p.x=center.x+(p.x-center.x)*visualScale;p.y=center.y+(p.y-center.y)*visualScale;vertex.position=p;mesh.SetUIVertex(vertex,i);}}
 }
}
