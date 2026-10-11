using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI {
 // Textured sprites are intentionally unnecessary for this HOME-only graphic.
 [RequireComponent(typeof(CanvasRenderer))]
 public sealed class HomeQuitMeshGraphic : MaskableGraphic {
  [SerializeField] Color fill=new Color(.063f,.102f,.18f,1),edge=new Color(.81f,.70f,.43f,.5f);
  [SerializeField] float radius=18,stroke=1;
  public void Style(Color background,Color border,float corner,float line){fill=background;edge=border;radius=corner;stroke=line;SetVerticesDirty();}
  public void Geometry(float corner,float line){if(Mathf.Approximately(radius,corner)&&Mathf.Approximately(stroke,line))return;radius=corner;stroke=line;SetVerticesDirty();}
  protected override void OnPopulateMesh(VertexHelper vh){vh.Clear();Rect r=rectTransform.rect;if(r.width<=0||r.height<=0)return;float rad=Mathf.Clamp(radius,0,Mathf.Min(r.width,r.height)*.5f),line=Mathf.Clamp(stroke,0,Mathf.Min(r.width,r.height)*.5f);Rect inside=new Rect(r.xMin+line,r.yMin+line,r.width-2*line,r.height-2*line);const int n=36;
   vh.AddVert(r.center,(Color32)(fill*color),Vector2.zero);
   for(int i=0;i<n;i++)vh.AddVert(Point(inside,Mathf.Max(0,rad-line),i),(Color32)(fill*color),Vector2.zero);
   for(int i=0;i<n;i++)vh.AddTriangle(0,1+i,1+(i+1)%n);
   for(int i=0;i<n;i++)vh.AddVert(Point(r,rad,i),(Color32)(edge*color),Vector2.zero);
   for(int i=0;i<n;i++){int j=(i+1)%n;vh.AddTriangle(1+i,1+n+i,1+n+j);vh.AddTriangle(1+i,1+n+j,1+j);}
   // Transparent outer edge softens the polygon at different Canvas scales.
   float aa=.65f;Rect outer=new Rect(r.xMin-aa,r.yMin-aa,r.width+2*aa,r.height+2*aa);Color transparent=edge*color;transparent.a=0;
   for(int i=0;i<n;i++)vh.AddVert(Point(outer,rad+aa,i),transparent,Vector2.zero);
   for(int i=0;i<n;i++){int j=(i+1)%n;vh.AddTriangle(1+n+i,1+2*n+i,1+2*n+j);vh.AddTriangle(1+n+i,1+2*n+j,1+n+j);}
  }
  static Vector2 Point(Rect r,float radius,int index){int corner=index/9;float angle=(corner*90+index%9*90f/8)*Mathf.Deg2Rad;var center=new Vector2(corner==0||corner==3?r.xMax-radius:r.xMin+radius,corner<2?r.yMax-radius:r.yMin+radius);return center+new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*radius;}
 }
}
