using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI {
 [DefaultExecutionOrder(150)] public sealed class GameMeshPresentation:MonoBehaviour {
  [System.Serializable] public struct Caption {public Text source;public TMP_Text target;public float size;}
  [SerializeField] Caption[] captions;[SerializeField] TMP_FontAsset medium,semi;
  public void Configure(Caption[] values,TMP_FontAsset body,TMP_FontAsset emphasis){captions=values;medium=body;semi=emphasis;}
  void LateUpdate()=>Apply();
  public void Apply(){
   var safe=(RectTransform)transform.parent;float w=safe.rect.width,h=safe.rect.height,q=Mathf.Max(.1f,Mathf.Min(w/1080,h/1920));var visual=GetComponent<GameVisualPresentation>();
   Position((RectTransform)transform.Find("BestFrameArea"),w-170*q,h-84*q,300*q,130*q);
   var best=visual.BestText;Anchor(best.rectTransform,.29f,.13f,.94f,.55f);best.margin=Vector4.zero;best.fontSizeMax=40*q;best.fontSizeMin=16*q;
   var bestTitle=transform.Find("BestFrameArea/BestFrame/MeshBestTitle").GetComponent<TMP_Text>();bestTitle.fontSize=bestTitle.fontSizeMax=25*q;bestTitle.fontSizeMin=18*q;
   Position(visual.ScoreText.rectTransform,w/2,h-169*q,w*.38f,132*q);visual.ScoreText.margin=Vector4.zero;visual.ScoreText.fontSizeMax=106*q;visual.ScoreText.fontSizeMin=30*q;
   var scoreLabel=transform.Find("ScoreLabel").GetComponent<TMP_Text>();scoreLabel.fontSize=scoreLabel.fontSizeMax=32*q;
   var home=(RectTransform)safe.Find("PlayingHomeButton");Position(home,98*q,h-91*q,130*q,130*q);home.Find("MeshSurface").GetComponent<HomeQuitMeshGraphic>().Geometry(65*q,2*q);
   // Preserve Board dimensions, Cell geometry and Slot drag hit areas.
   var board=(RectTransform)safe.Find("Board");var face=(RectTransform)safe.Find("BoardFrame/MeshSurface");face.anchorMin=face.anchorMax=Vector2.one*.5f;face.anchoredPosition=Vector2.zero;face.sizeDelta=board.rect.size+Vector2.one*30*q;face.GetComponent<HomeQuitMeshGraphic>().Geometry(25*q,2.5f*q);
   var panel=safe.Find("PlanetRestoration");Anchor((RectTransform)panel.Find("Title"),.245f,.61f,.94f,.85f);Anchor((RectTransform)panel.Find("RestorationPercent"),.245f,.34f,.46f,.57f);Anchor((RectTransform)panel.Find("ProgressBar"),.49f,.40f,.94f,.51f);Anchor((RectTransform)panel.Find("Stage"),.245f,.12f,.55f,.30f);Anchor((RectTransform)panel.Find("Energy"),.57f,.12f,.94f,.30f);
   panel.Find("StatusFrame/MeshSurface").GetComponent<HomeQuitMeshGraphic>().Geometry(28*q,1.8f*q);
   foreach(var c in captions){c.target.text=c.source.text;c.target.fontSizeMax=c.size*q;c.target.fontSizeMin=16*q;}
   var assists=GetComponentInParent<Canvas>().GetComponent<GameAssistView>();
   if(assists!=null&&assists.Root!=null){Assist(assists.RefreshButton,"블록 새로고침",GameMeshIcon.Refresh,q);Assist(assists.HintButton,"힌트",GameMeshIcon.Hint,q);
    foreach(var c in assists.Root.GetComponentsInChildren<Text>(true)){c.enabled=false;var tmp=c.transform.Find("MeshLabel")?.GetComponent<TMP_Text>();if(tmp==null){tmp=NewText(c.transform,"MeshLabel",medium);Anchor(tmp.rectTransform,0,0,1,1);}tmp.text=c.text;tmp.fontSize=tmp.fontSizeMax=24*q;tmp.fontSizeMin=15*q;}}
  }
  void Assist(Button button,string title,GameMeshIcon icon,float q){
   var image=button.GetComponent<Image>();image.color=Color.clear;image.raycastTarget=false;
   var face=button.transform.Find("MeshSurface")?.GetComponent<HomeQuitMeshGraphic>();
   if(face==null){face=Surface(button.transform,true);button.targetGraphic=face;var t=NewText(button.transform,"MeshTitle",semi);t.text=title;Anchor(t.rectTransform,.25f,.08f,.94f,.92f);var r=Node(button.transform,"MeshIcon");Anchor(r,.065f,.24f,.23f,.76f);r.gameObject.AddComponent<GameMeshIconGraphic>().Configure(icon);}
   face.Geometry(45*q,1.8f*q);var text=button.transform.Find("MeshTitle").GetComponent<TMP_Text>();text.fontSizeMax=32*q;text.fontSizeMin=18*q;
  }
  public static HomeQuitMeshGraphic Surface(Transform p,bool hit){var r=Node(p,"MeshSurface");Anchor(r,0,0,1,1);r.SetAsFirstSibling();var g=r.GetComponent<HomeQuitMeshGraphic>()??r.gameObject.AddComponent<HomeQuitMeshGraphic>();g.Style(new Color(.025f,.055f,.13f,.92f),new Color(.28f,.75f,.95f,.9f),24,1.8f);g.raycastTarget=hit;return g;}
  public static TextMeshProUGUI NewText(Transform p,string name,TMP_FontAsset font){var r=Node(p,name);var t=r.GetComponent<TextMeshProUGUI>()??r.gameObject.AddComponent<TextMeshProUGUI>();t.font=font;t.color=new Color(.84f,.94f,1);t.fontSize=30;t.enableAutoSizing=true;t.fontSizeMax=30;t.fontSizeMin=16;t.alignment=TextAlignmentOptions.Center;t.raycastTarget=false;return t;}
  public static RectTransform Node(Transform p,string n){var old=p.Find(n);if(old!=null)return(RectTransform)old;var go=new GameObject(n,typeof(RectTransform),typeof(CanvasRenderer));go.transform.SetParent(p,false);return(RectTransform)go.transform;}
  public static void Anchor(RectTransform r,float a,float b,float c,float d){r.anchorMin=new Vector2(a,b);r.anchorMax=new Vector2(c,d);r.offsetMin=r.offsetMax=Vector2.zero;}
  static void Position(RectTransform r,float x,float y,float w,float h){var p=(RectTransform)r.parent;r.anchorMin=r.anchorMax=r.pivot=Vector2.one*.5f;r.sizeDelta=new Vector2(w,h);r.anchoredPosition=new Vector2(x-p.rect.width/2,y-p.rect.height/2);}
 }
}
