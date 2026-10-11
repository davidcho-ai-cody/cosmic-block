using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI {
 public enum HomeMenuIcon { Play, Orbit, Chevron, Trophy, Settings, Power }
 [RequireComponent(typeof(CanvasRenderer))] public sealed class HomeMenuIconGraphic:MaskableGraphic {
  [SerializeField] HomeMenuIcon icon;
  public void Configure(HomeMenuIcon value){icon=value;raycastTarget=false;SetVerticesDirty();}
  protected override void OnPopulateMesh(VertexHelper vh){vh.Clear();
   switch(icon){case HomeMenuIcon.Play:Path(vh,new[]{new Vector2(-6,9),new Vector2(9,0),new Vector2(-6,-9),new Vector2(-6,9)});break;
    case HomeMenuIcon.Orbit:Ring(vh,7,7,0,0,360);Ring(vh,12,4,-30,0,360);break;
    case HomeMenuIcon.Chevron:Path(vh,new[]{new Vector2(-3,7),new Vector2(4,0),new Vector2(-3,-7)});break;
    case HomeMenuIcon.Power:Ring(vh,8,8,0,125,415);Line(vh,new Vector2(0,10),new Vector2(0,0));break;
    case HomeMenuIcon.Settings:Ring(vh,4,4,0,0,360);for(int i=0;i<24;i++){float a=i*Mathf.PI*2/24,b=(i+1)*Mathf.PI*2/24;float r=i%3==0?10:8.3f,rr=(i+1)%3==0?10:8.3f;Line(vh,new Vector2(Mathf.Cos(a),Mathf.Sin(a))*r,new Vector2(Mathf.Cos(b),Mathf.Sin(b))*rr);}break;
    case HomeMenuIcon.Trophy:Path(vh,new[]{new Vector2(-6,9),new Vector2(-5,1),new Vector2(-2,-3),new Vector2(2,-3),new Vector2(5,1),new Vector2(6,9),new Vector2(-6,9)});Path(vh,new[]{new Vector2(-6,6),new Vector2(-10,6),new Vector2(-9,0),new Vector2(-4,-1)});Path(vh,new[]{new Vector2(6,6),new Vector2(10,6),new Vector2(9,0),new Vector2(4,-1)});Line(vh,new Vector2(0,-3),new Vector2(0,-8));Line(vh,new Vector2(-5,-9),new Vector2(5,-9));break;
   }
  }
  void Path(VertexHelper vh,Vector2[] points){for(int i=1;i<points.Length;i++)Line(vh,points[i-1],points[i]);}
  void Ring(VertexHelper vh,float x,float y,float tilt,float from,float to){Vector2 previous=Vector2.zero;for(int i=0;i<=48;i++){float a=Mathf.Lerp(from,to,i/48f)*Mathf.Deg2Rad,t=tilt*Mathf.Deg2Rad;var p=new Vector2(x*Mathf.Cos(a),y*Mathf.Sin(a));p=new Vector2(p.x*Mathf.Cos(t)-p.y*Mathf.Sin(t),p.x*Mathf.Sin(t)+p.y*Mathf.Cos(t));if(i>0)Line(vh,previous,p);previous=p;}}
  void Line(VertexHelper vh,Vector2 a,Vector2 b){float u=Mathf.Min(rectTransform.rect.width,rectTransform.rect.height)/24f;a=a*u+rectTransform.rect.center;b=b*u+rectTransform.rect.center;var n=new Vector2(-(b-a).y,(b-a).x).normalized*.75f*u;int k=vh.currentVertCount;vh.AddVert(a+n,color,Vector2.zero);vh.AddVert(a-n,color,Vector2.zero);vh.AddVert(b-n,color,Vector2.zero);vh.AddVert(b+n,color,Vector2.zero);vh.AddTriangle(k,k+1,k+2);vh.AddTriangle(k,k+2,k+3);}
 }
}
