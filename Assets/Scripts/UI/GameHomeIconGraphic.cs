using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI {
 [RequireComponent(typeof(CanvasRenderer))]
 public sealed class GameHomeIconGraphic : MaskableGraphic {
  protected override void OnPopulateMesh(VertexHelper vh){vh.Clear();var r=rectTransform.rect;Vector2 P(float x,float y)=>new Vector2(r.xMin+x*r.width,r.yMin+y*r.height);Line(vh,P(.10f,.53f),P(.50f,.89f));Line(vh,P(.50f,.89f),P(.90f,.53f));Line(vh,P(.22f,.58f),P(.22f,.12f));Line(vh,P(.22f,.12f),P(.78f,.12f));Line(vh,P(.78f,.12f),P(.78f,.58f));Line(vh,P(.43f,.12f),P(.43f,.41f));Line(vh,P(.43f,.41f),P(.57f,.41f));Line(vh,P(.57f,.41f),P(.57f,.12f));}
  void Line(VertexHelper vh,Vector2 a,Vector2 b){var n=new Vector2(-(b-a).y,(b-a).x).normalized*Mathf.Max(1,rectTransform.rect.width*.035f);int i=vh.currentVertCount;vh.AddVert(a-n,color,Vector2.zero);vh.AddVert(a+n,color,Vector2.zero);vh.AddVert(b+n,color,Vector2.zero);vh.AddVert(b-n,color,Vector2.zero);vh.AddTriangle(i,i+1,i+2);vh.AddTriangle(i,i+2,i+3);}
 }
}
