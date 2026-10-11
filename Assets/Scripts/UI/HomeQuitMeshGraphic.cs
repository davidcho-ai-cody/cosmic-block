using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI {
 // Rounded confirmation panels and buttons use only generated vertex colors.
 [RequireComponent(typeof(CanvasRenderer))]
 public sealed class HomeQuitMeshGraphic : MaskableGraphic {
  [SerializeField] Color fill=new Color(.063f,.102f,.18f,1),edge=new Color(.81f,.70f,.43f,.5f);
  [SerializeField] float radius=18,stroke=1;
  public void Style(Color background,Color border,float corner,float line){fill=background;edge=border;radius=corner;stroke=line;SetVerticesDirty();}
  public void Geometry(float corner,float line){if(Mathf.Approximately(radius,corner)&&Mathf.Approximately(stroke,line))return;radius=corner;stroke=line;SetVerticesDirty();}
  protected override void OnPopulateMesh(VertexHelper vh){vh.Clear();Rect r=rectTransform.rect;if(r.width<=0||r.height<=0)return;float rad=Mathf.Clamp(radius,0,Mathf.Min(r.width,r.height)*.5f),line=Mathf.Clamp(stroke,0,Mathf.Min(r.width,r.height)*.5f);Rect inside=new Rect(r.xMin+line,r.yMin+line,r.width-2*line,r.height-2*line);const int n=36;
   // Soft outer light and a lowered shadow, drawn behind the surface.
   Halo(vh,r,rad,Mathf.Max(2,stroke*5),edge*new Color(1,1,1,.14f),Vector2.zero);
   Halo(vh,r,rad,Mathf.Max(2,stroke*4),new Color(0,0,0,.20f),new Vector2(0,-stroke*2));int start=vh.currentVertCount;
   vh.AddVert(r.center,Surface(r.center,r),Vector2.zero);
   for(int i=0;i<n;i++)vh.AddVert(Point(inside,Mathf.Max(0,rad-line),i),Surface(Point(inside,Mathf.Max(0,rad-line),i),r),Vector2.zero);
   for(int i=0;i<n;i++)vh.AddTriangle(start,start+1+i,start+1+(i+1)%n);
   for(int i=0;i<n;i++)vh.AddVert(Point(r,rad,i),Border(Point(r,rad,i),r),Vector2.zero);
   for(int i=0;i<n;i++){int j=(i+1)%n;vh.AddTriangle(start+1+i,start+1+n+i,start+1+n+j);vh.AddTriangle(start+1+i,start+1+n+j,start+1+j);}
   // Transparent outer edge softens the polygon at different Canvas scales.
   float aa=.65f;Rect outer=new Rect(r.xMin-aa,r.yMin-aa,r.width+2*aa,r.height+2*aa);Color transparent=edge*color;transparent.a=0;
   for(int i=0;i<n;i++)vh.AddVert(Point(outer,rad+aa,i),transparent,Vector2.zero);
   for(int i=0;i<n;i++){int j=(i+1)%n;vh.AddTriangle(start+1+n+i,start+1+2*n+i,start+1+2*n+j);vh.AddTriangle(start+1+n+i,start+1+2*n+j,start+1+n+j);}
  }
  Color Surface(Vector2 p,Rect r){float t=Mathf.InverseLerp(r.yMin,r.yMax,p.y);Color c=Color.Lerp(fill*new Color(.78f,.82f,.90f,1),fill*new Color(1.12f,1.12f,1.10f,1),t);return c*color;}
  Color Border(Vector2 p,Rect r){float t=Mathf.InverseLerp(r.yMin,r.yMax,p.y);return Color.Lerp(edge*new Color(.60f,.65f,.75f,.65f),edge*new Color(1.20f,1.20f,1.20f,1),t)*color;}
  void Halo(VertexHelper vh,Rect r,float rad,float spread,Color tint,Vector2 offset){int start=vh.currentVertCount;var inner=new Rect(r.position+offset,r.size);var outer=new Rect(inner.xMin-spread,inner.yMin-spread,inner.width+spread*2,inner.height+spread*2);var transparent=tint;transparent.a=0;for(int i=0;i<36;i++)vh.AddVert(Point(inner,rad,i),tint*color,Vector2.zero);for(int i=0;i<36;i++)vh.AddVert(Point(outer,rad+spread,i),transparent,Vector2.zero);for(int i=0;i<36;i++){int j=(i+1)%36;vh.AddTriangle(start+i,start+36+i,start+36+j);vh.AddTriangle(start+i,start+36+j,start+j);}}
  static Vector2 Point(Rect r,float radius,int index){int corner=index/9;float angle=(corner*90+index%9*90f/8)*Mathf.Deg2Rad;var center=new Vector2(corner==0||corner==3?r.xMax-radius:r.xMin+radius,corner<2?r.yMax-radius:r.yMin+radius);return center+new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*radius;}
 }
}
