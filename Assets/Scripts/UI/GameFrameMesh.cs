using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI {
 // A five-by-five frame layout stretches straight strips while keeping corner
 // and middle ornaments at the same scale. The original Sprite and alpha remain.
 [RequireComponent(typeof(Image))]
 public sealed class GameFrameMesh:BaseMeshEffect {
  [SerializeField] float decorationScale=.22f;
  [SerializeField] bool planetStatus;
  public void ConfigurePlanetStatus(){planetStatus=true;graphic.SetVerticesDirty();}
  static readonly float[] SourceX={0,250,470,830,1049,1299};
  static readonly float[] SourceY={0,230,465,745,981,1211};
  public void SetDecorationScale(float value){if(Mathf.Approximately(value,decorationScale))return;decorationScale=value;graphic.SetVerticesDirty();}
  public Vector2 PlanetTrackHorizontalAnchors(){
   var r=((Image)graphic).rectTransform.rect;float scale=r.height/779;
   float[] sx={0,620,1060,1320,1720,2018};float[] x={0,620*scale,r.width*.59f-130*scale,r.width*.59f+130*scale,r.width-298*scale,r.width};
   // Inner track pixels 982..1858 of the original 2048-wide HUD art; borders stay outside.
   return new Vector2(Map(982f/2048*2018,sx,x)/r.width,Map(1858f/2048*2018,sx,x)/r.width);
  }
  static float Map(float value,float[] source,float[] target){for(int i=0;i<source.Length-1;i++)if(value<=source[i+1])return Mathf.Lerp(target[i],target[i+1],(value-source[i])/(source[i+1]-source[i]));return target[target.Length-1];}
  public override void ModifyMesh(VertexHelper mesh){
   if(!IsActive())return;var image=(Image)graphic;var sprite=image.sprite;if(sprite==null)return;
   Rect r=image.rectTransform.rect,uv=sprite.textureRect;float scale=decorationScale;
   float[] x={r.xMin,r.xMin+250*scale,r.center.x-180*scale,r.center.x+180*scale,r.xMax-250*scale,r.xMax};
   float[] y={r.yMin,r.yMin+230*scale,r.center.y-140*scale,r.center.y+140*scale,r.yMax-230*scale,r.yMax};
   float[] sx=SourceX,sy=SourceY;float sourceW=1299,sourceH=1211;
   if(planetStatus){scale=r.height/779;sx=new float[]{0,620,1060,1320,1720,2018};sy=new float[]{0,779};sourceW=2018;sourceH=779;x=new float[]{r.xMin,r.xMin+620*scale,r.xMin+r.width*.59f-130*scale,r.xMin+r.width*.59f+130*scale,r.xMax-298*scale,r.xMax};y=new float[]{r.yMin,r.yMax};}
   mesh.Clear();var quad=new UIVertex[4];
   for(int row=0;row<sy.Length-1;row++)for(int col=0;col<5;col++){
    for(int i=0;i<4;i++){int cx=col+(i==2||i==3?1:0),cy=row+(i==1||i==2?1:0);var v=UIVertex.simpleVert;v.position=new Vector3(x[cx],y[cy]);v.color=image.color;v.uv0=new Vector2((uv.x+uv.width*sx[cx]/sourceW)/sprite.texture.width,(uv.y+uv.height*sy[cy]/sourceH)/sprite.texture.height);quad[i]=v;}
    mesh.AddUIVertexQuad(quad);
   }
  }
 }
}