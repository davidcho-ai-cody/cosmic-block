using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI {
 public enum GameMeshIcon { Home,Refresh,Hint }
 public sealed class GameMeshIconGraphic:MaskableGraphic {
  [SerializeField] GameMeshIcon icon;
  public void Configure(GameMeshIcon value){icon=value;raycastTarget=false;color=new Color(.6f,.88f,1);SetVerticesDirty();}
  protected override void OnPopulateMesh(VertexHelper v){v.Clear();if(icon==GameMeshIcon.Home){Path(v,new[]{new Vector2(-10,0),new Vector2(0,9),new Vector2(10,0)});Path(v,new[]{new Vector2(-7,2),new Vector2(-7,-9),new Vector2(-2,-9),new Vector2(-2,-3),new Vector2(2,-3),new Vector2(2,-9),new Vector2(7,-9),new Vector2(7,2)});}else if(icon==GameMeshIcon.Refresh){Arc(v,8,8,30,195);Arc(v,8,8,210,375);Path(v,new[]{new Vector2(3,9),new Vector2(8,4),new Vector2(8,10)});Path(v,new[]{new Vector2(-3,-9),new Vector2(-8,-4),new Vector2(-8,-10)});}else{Arc(v,7,8,-30,210);Path(v,new[]{new Vector2(-6,-4),new Vector2(-3,-7),new Vector2(3,-7),new Vector2(6,-4)});Line(v,new Vector2(-3,-10),new Vector2(3,-10));Line(v,new Vector2(-2,-12),new Vector2(2,-12));}}
  void Arc(VertexHelper v,float x,float y,float a,float b){var p=Vector2.zero;for(int i=0;i<=36;i++){float t=Mathf.Lerp(a,b,i/36f)*Mathf.Deg2Rad;var n=new Vector2(Mathf.Cos(t)*x,Mathf.Sin(t)*y);if(i>0)Line(v,p,n);p=n;}}
  void Path(VertexHelper v,Vector2[] p){for(int i=1;i<p.Length;i++)Line(v,p[i-1],p[i]);}
  void Line(VertexHelper v,Vector2 a,Vector2 b){float q=Mathf.Min(rectTransform.rect.width,rectTransform.rect.height)/26;a=a*q+rectTransform.rect.center;b=b*q+rectTransform.rect.center;var n=new Vector2(-(b-a).y,(b-a).x).normalized*q*.8f;int k=v.currentVertCount;v.AddVert(a+n,color,Vector2.zero);v.AddVert(a-n,color,Vector2.zero);v.AddVert(b-n,color,Vector2.zero);v.AddVert(b+n,color,Vector2.zero);v.AddTriangle(k,k+1,k+2);v.AddTriangle(k,k+2,k+3);}
 }
}
