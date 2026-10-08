using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI {
 public sealed class PlanetArtCatalog:ScriptableObject {
  [System.Serializable] public sealed class Art {public int planetId;public Sprite[] stages;public float[] scales;public Vector2[] bodyCenters;}
  public Art[] planets;public Sprite lockedPlanet,smallLock,homeBrand;
  static PlanetArtCatalog cache;
  public static PlanetArtCatalog Current=>cache!=null?cache:cache=Resources.Load<PlanetArtCatalog>("PlanetArtCatalog");
  public Sprite Sprite(int id,int stage){foreach(var a in planets)if(a.planetId==id)return a.stages[Mathf.Clamp(stage,1,5)-1];return lockedPlanet;}
  public float Scale(int id,int stage){foreach(var a in planets)if(a.planetId==id)return a.scales[Mathf.Clamp(stage,1,5)-1];return 1;}
  public float Bound(int id){float bound=1;foreach(var a in planets)if(a.planetId==id)for(int i=0;i<5;i++)bound=Mathf.Max(bound,a.scales[i]*(1+2*Mathf.Max(Mathf.Abs(a.bodyCenters[i].x-.5f),Mathf.Abs(a.bodyCenters[i].y-.5f))));return bound;}
  public void Apply(Image image,int id,int stage){image.sprite=Sprite(id,stage);image.type=Image.Type.Simple;image.preserveAspect=true;float scale=Scale(id,stage);image.transform.localScale=Vector3.one*scale;Vector2 center=new Vector2(.5f,.5f);foreach(var a in planets)if(a.planetId==id&&a.bodyCenters!=null&&a.bodyCenters.Length>=stage)center=a.bodyCenters[stage-1];float side=Mathf.Min(image.rectTransform.rect.width,image.rectTransform.rect.height)*scale;image.rectTransform.anchoredPosition=new Vector2(.5f-center.x,center.y-.5f)*side;}
 }
}
