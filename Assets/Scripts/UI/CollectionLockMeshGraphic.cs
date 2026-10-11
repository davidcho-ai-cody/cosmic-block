using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI {
 [RequireComponent(typeof(CanvasRenderer))] public sealed class CollectionLockMeshGraphic:MaskableGraphic {
  protected override void OnPopulateMesh(VertexHelper vh){vh.Clear();float u=Mathf.Min(rectTransform.rect.width,rectTransform.rect.height)/24f;var center=rectTransform.rect.center;var bottom=new Rect(center.x-7*u,center.y-9*u,14*u,12*u);Quad(vh,bottom,color);
   for(int i=0;i<24;i++){float a=i*Mathf.PI/24,b=(i+1)*Mathf.PI/24;Vector2 p=new Vector2(Mathf.Cos(a),Mathf.Sin(a)),q=new Vector2(Mathf.Cos(b),Mathf.Sin(b));int n=vh.currentVertCount;Vector2 c=center+Vector2.up*3*u;vh.AddVert(c+p*6*u,color,Vector2.zero);vh.AddVert(c+p*4.5f*u,color,Vector2.zero);vh.AddVert(c+q*4.5f*u,color,Vector2.zero);vh.AddVert(c+q*6*u,color,Vector2.zero);vh.AddTriangle(n,n+1,n+2);vh.AddTriangle(n,n+2,n+3);}
   Quad(vh,new Rect(center.x-u,center.y-5*u,2*u,5*u),new Color(.03f,.06f,.12f,color.a));
  }
  static void Quad(VertexHelper vh,Rect r,Color c){int n=vh.currentVertCount;vh.AddVert(new Vector2(r.xMin,r.yMin),c,Vector2.zero);vh.AddVert(new Vector2(r.xMax,r.yMin),c,Vector2.zero);vh.AddVert(new Vector2(r.xMax,r.yMax),c,Vector2.zero);vh.AddVert(new Vector2(r.xMin,r.yMax),c,Vector2.zero);vh.AddTriangle(n,n+1,n+2);vh.AddTriangle(n,n+2,n+3);}
 }
}
