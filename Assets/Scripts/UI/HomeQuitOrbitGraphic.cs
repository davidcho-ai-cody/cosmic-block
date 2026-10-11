using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI {
 [RequireComponent(typeof(CanvasRenderer))] public sealed class HomeQuitOrbitGraphic : MaskableGraphic {
  protected override void OnPopulateMesh(VertexHelper vh){vh.Clear();float unit=Mathf.Min(rectTransform.rect.width,rectTransform.rect.height)/24f;Ring(vh,7*unit,7*unit,0,unit);Ring(vh,11*unit,4*unit,-35*Mathf.Deg2Rad,.8f*unit);}
  void Ring(VertexHelper vh,float x,float y,float angle,float line){int start=vh.currentVertCount;for(int i=0;i<48;i++){float a=i*Mathf.PI*2/48;foreach(float offset in new[]{-line*.5f,line*.5f}){Vector2 p=new Vector2((x+offset)*Mathf.Cos(a),(y+offset)*Mathf.Sin(a));p=new Vector2(p.x*Mathf.Cos(angle)-p.y*Mathf.Sin(angle),p.x*Mathf.Sin(angle)+p.y*Mathf.Cos(angle));vh.AddVert(p+rectTransform.rect.center,color,Vector2.zero);}}for(int i=0;i<48;i++){int next=(i+1)%48;vh.AddTriangle(start+i*2,start+next*2,start+next*2+1);vh.AddTriangle(start+i*2,start+next*2+1,start+i*2+1);}}
 }
}
