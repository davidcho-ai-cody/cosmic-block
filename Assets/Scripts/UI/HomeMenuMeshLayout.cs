using TMPro;
using UnityEngine;
namespace CosmicBlock.UI {
 // HOME presentation only. Existing buttons and event listeners remain in place.
 public sealed class HomeMenuMeshLayout : MonoBehaviour {
  [SerializeField] TextMeshProUGUI bestLabel;
  HomeViewController view;
  public void Configure(TextMeshProUGUI best){bestLabel=best;Apply();}
  void Awake(){view=GetComponent<HomeViewController>();}
  void LateUpdate(){Apply();if(bestLabel!=null&&view!=null)bestLabel.text=view.BestText;}
  public void Apply(){var home=(RectTransform)transform;if(home.rect.width<=0)return;
   float w=home.rect.width*.80f,top=home.rect.height*.485f-24*home.rect.width/1080f;
   float q=Mathf.Min(w/864f,(top-home.rect.height*.105f)/590f);q=Mathf.Max(.1f,q);
   var actions=(RectTransform)transform.Find("MainActions");Stretch(actions);var bottom=(RectTransform)transform.Find("BottomActions");Stretch(bottom);
   float y=-home.rect.height*.5f+top;
   Row((RectTransform)actions.Find("GameStartButton"),w,100*q,ref y);y-=28*q;
   Row((RectTransform)actions.Find("PlanetCollectionButton"),w,94*q,ref y);y-=28*q;
   Row((RectTransform)transform.Find("BestScoreArea"),w,62*q,ref y);y-=42*q;
   float utilityW=(w-38*q)*.5f;var settings=(RectTransform)bottom.Find("SettingsButton");var quit=(RectTransform)bottom.Find("QuitButton");Place(settings,utilityW,62*q,new Vector2(-(utilityW+38*q)*.5f,y-31*q));Place(quit,utilityW,62*q,new Vector2((utilityW+38*q)*.5f,y-31*q));y-=62*q+26*q;
   Row((RectTransform)transform.Find("NewGameButton"),w*.64f,44*q,ref y);
   foreach(var text in GetComponentsInChildren<TextMeshProUGUI>(true)){if(text.transform.IsChildOf(actions)||text.transform.IsChildOf(bottom)||text.transform.IsChildOf(transform.Find("BestScoreArea"))||text.transform.IsChildOf(transform.Find("NewGameButton"))){float font=text.name=="MeshPrimaryLabel"?40:text.name=="MeshCollectionLabel"?34:text.name=="MeshBestValue"?34:text.name=="MeshNewLabel"?23:25;text.fontSize=font*q;if(text.name=="MeshBestValue"){text.fontSizeMax=font*q;text.fontSizeMin=16*q;}}}
   foreach(var b in new[]{actions.Find("GameStartButton"),actions.Find("PlanetCollectionButton"),settings,quit})b.Find("MeshSurface").GetComponent<HomeQuitMeshGraphic>()?.Geometry(24*q,1.8f*q);
   transform.Find("BestScoreArea/Frame/MeshSurface").GetComponent<HomeQuitMeshGraphic>()?.Geometry(18*q,q);
  }
  static void Stretch(RectTransform r){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;}
  static void Row(RectTransform r,float w,float h,ref float y){Place(r,w,h,new Vector2(0,y-h*.5f));y-=h;}
  static void Place(RectTransform r,float w,float h,Vector2 p){r.anchorMin=r.anchorMax=r.pivot=Vector2.one*.5f;r.sizeDelta=new Vector2(w,h);r.anchoredPosition=p;}
 }
}
