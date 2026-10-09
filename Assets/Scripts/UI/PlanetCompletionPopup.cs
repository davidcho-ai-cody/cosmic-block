using System.Collections;
using CosmicBlock.Core;
using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI {
 public sealed class PlanetCompletionPopup:MonoBehaviour {
  [SerializeField] GameFlowController flow;[SerializeField] GameSession session;
  GameObject popup;CanvasGroup group;RectTransform card;Text title,body,unlocked;Image planet;Button next,keep;Coroutine entrance;int planetId;readonly Image[] stars=new Image[4];
  public bool Visible=>popup!=null&&popup.activeSelf;
  public bool ConfirmationVisible=>false;
  public int ShownPlanet=>planetId;public int ShowCount{get;private set;}
  public GameObject PopupRoot=>popup;public GameObject ConfirmationRoot=>null;
  public void Configure(GameFlowController owner,GameSession game){flow=owner;session=game;}
  void Awake(){Build();}
  void Build(){
   var safe=flow.HomeRoot.transform.parent;var template=flow.HomeRoot.transform.Find("NewGameConfirmPanel").gameObject;
   popup=Instantiate(template,safe);popup.name="PlanetCompletionPopup";popup.SetActive(false);
   
   card=(RectTransform)popup.transform.Find("Card");card.anchorMin=new Vector2(.08f,.17f);card.anchorMax=new Vector2(.92f,.83f);card.offsetMin=card.offsetMax=Vector2.zero;
   group=popup.GetComponent<CanvasGroup>();if(group==null)group=popup.AddComponent<CanvasGroup>();
   title=card.Find("Question").GetComponent<Text>();title.name="Title";Place(title.rectTransform,.06f,.90f,.94f,.98f);title.text="★ 행성 복원 완료!";title.fontStyle=FontStyle.Bold;title.color=new Color(1,.8f,.32f);title.resizeTextMaxSize=34;title.resizeTextMinSize=20;
   body=Instantiate(title,card);body.name="Description";Place(body.rectTransform,.07f,.80f,.93f,.90f);body.fontStyle=FontStyle.Normal;body.color=new Color(.75f,.91f,1);body.resizeTextMaxSize=26;
   unlocked=Instantiate(body,card);unlocked.name="Unlock";Place(unlocked.rectTransform,.06f,.33f,.94f,.45f);
   var art=new GameObject("CompletedPlanet",typeof(RectTransform),typeof(Image));art.transform.SetParent(card,false);planet=art.GetComponent<Image>();planet.raycastTarget=false;Place(planet.rectTransform,.27f,.46f,.73f,.79f);
   next=card.Find("Confirm").GetComponent<Button>();keep=card.Find("Cancel").GetComponent<Button>();Place((RectTransform)next.transform,.10f,.20f,.90f,.30f);Place((RectTransform)keep.transform,.10f,.07f,.90f,.17f);
   next.onClick.RemoveAllListeners();keep.onClick.RemoveAllListeners();next.onClick.AddListener(Next);keep.onClick.AddListener(Continue);keep.GetComponentInChildren<Text>().text="계속 플레이";
   foreach(var spark in session.Feedback.GetComponentsInChildren<Image>(true))if(spark.name.StartsWith("Starlight_")){for(int i=0;i<4;i++){var go=new GameObject("PopupStar"+i,typeof(RectTransform),typeof(Image));go.transform.SetParent(card,false);var image=go.GetComponent<Image>();stars[i]=image;image.sprite=spark.sprite;image.preserveAspect=true;image.raycastTarget=false;image.color=new Color(1,.8f,.4f,.45f);var r=image.rectTransform;r.anchorMin=r.anchorMax=new Vector2(i%2==0?.15f:.85f,i<2?.58f:.74f);r.sizeDelta=Vector2.one*22;}break;}
  }
  static void Place(RectTransform r,float x0,float y0,float x1,float y1){r.anchorMin=new Vector2(x0,y0);r.anchorMax=new Vector2(x1,y1);r.offsetMin=r.offsetMax=Vector2.zero;}
  void LateUpdate(){if(Visible)for(int i=0;i<stars.Length;i++){if(stars[i]==null)continue;float pulse=(Mathf.Sin(Time.unscaledTime*1.8f+i*1.4f)+1)*.5f;stars[i].color=new Color(1,.8f,.4f,Mathf.Lerp(.22f,.48f,pulse));stars[i].transform.localScale=Vector3.one*Mathf.Lerp(.92f,1.08f,pulse);}if(Visible&&(flow.Screen!=FlowScreen.Game||session.Restoration.PlanetId!=planetId)){CloseTransient();return;}if(!Visible&&!flow.PlanetSwitchActive&&flow.Screen==FlowScreen.Game&&session.State!=GameState.Resolving&&session.Restoration.IsRestored&&PlanetCompletionNotice.Pending(session.Restoration.PlanetId)&&!session.ClearSequenceActive&&!session.Feedback.IsPlaying&&!Object.FindAnyObjectByType<GameHud>().PlanetView.FeedbackActive)Show(session.Restoration.PlanetId);}
  public void Show(int id){planetId=id;ShowCount++;popup.SetActive(true);popup.transform.SetAsLastSibling();session.ModalInputBlocked=true;title.text="★ 행성 복원 완료!";body.text=PlanetDefinitions.Get(id).Name+"의 빛을 모두 되찾았습니다.";unlocked.text=id<5?"새로운 행성 해금!\n행성 "+(id+1).ToString("00")+" · "+PlanetDefinitions.Get(id+1).Name:"모든 행성의 빛을 되찾았습니다!";next.GetComponentInChildren<Text>().text=id<5?"다음 행성으로 이동":"행성 도감";PlanetArtCatalog.Current.Apply(planet,id,5);float bound=PlanetArtCatalog.Current.Bound(id);planet.transform.localScale/=bound;planet.rectTransform.anchoredPosition/=bound;entrance=StartCoroutine(Appear());}
  IEnumerator Appear(){group.alpha=0;for(float t=0;t<.22f;t+=Time.unscaledDeltaTime){float p=Mathf.Clamp01(t/.22f);group.alpha=p;card.localScale=Vector3.one*Mathf.Lerp(.94f,1,p);yield return null;}group.alpha=1;card.localScale=Vector3.one;entrance=null;}
  public void Continue(){if(!Visible)return;PlanetCompletionNotice.Acknowledge(planetId);session.SaveCurrentRun();CloseTransient();}
  public void Next(){if(!Visible||flow.PlanetSwitchActive)return;if(planetId==5){Continue();flow.ShowHome();flow.ShowCollection();return;}if(!flow.TrySwitchRunPlanet(planetId+1,planetId)){unlocked.text="행성 이동을 저장할 수 없습니다.\n현재 게임을 유지합니다. 다시 시도해 주세요.";}}
  public void Back(){}
  public void CloseTransient(){if(entrance!=null)StopCoroutine(entrance);entrance=null;if(popup!=null)popup.SetActive(false);if(session!=null)session.ModalInputBlocked=false;}
  void OnDestroy(){CloseTransient();if(popup!=null)Destroy(popup);}
 }
}
