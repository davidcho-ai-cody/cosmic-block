using CosmicBlock.Core;
using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI {
 public sealed class HomeRunControls:MonoBehaviour {
  [SerializeField] GameFlowController flow;
  [SerializeField] Image primary;
  [SerializeField] Sprite playSprite,continueSprite;
  [SerializeField] Button newGame,cancel,confirm;
  [SerializeField] GameObject panel;
  public bool NewGameVisible=>panel!=null&&panel.activeSelf;
  public void Configure(GameFlowController owner,Image image,Sprite play,Sprite resume,Button auxiliary,GameObject modal,Button no,Button yes){flow=owner;primary=image;playSprite=play;continueSprite=resume;newGame=auxiliary;panel=modal;cancel=no;confirm=yes;}
  void Awake(){newGame.onClick.AddListener(RequestNewGame);cancel.onClick.AddListener(CancelNewGame);confirm.onClick.AddListener(ConfirmNewGame);}
  void OnEnable(){Refresh();}
  void LateUpdate(){Layout();}
  public void Layout(){var bottom=transform.Find("BottomActions")as RectTransform;if(bottom==null||newGame==null)return;var r=(RectTransform)newGame.transform;r.anchorMin=r.anchorMax=r.pivot=Vector2.one*.5f;r.sizeDelta=new Vector2(bottom.rect.width*.14f,bottom.rect.height*.70f);r.anchoredPosition=bottom.anchoredPosition+new Vector2(0,bottom.rect.height*.10f);}
  public void Refresh(){if(primary==null||flow==null)return;bool saved=flow.HasSavedRun;primary.sprite=saved?continueSprite:playSprite;primary.preserveAspect=true;newGame.gameObject.SetActive(saved);}
  public void RequestNewGame(){if(!flow.HasSavedRun){flow.StartNewRun();return;}panel.SetActive(true);panel.transform.SetAsLastSibling();}
  public void CancelNewGame(){if(panel!=null)panel.SetActive(false);}
  public void ConfirmNewGame(){if(!NewGameVisible)return;CancelNewGame();flow.StartNewRun();}
  void OnDisable(){CancelNewGame();}
  void OnDestroy(){newGame.onClick.RemoveListener(RequestNewGame);cancel.onClick.RemoveListener(CancelNewGame);confirm.onClick.RemoveListener(ConfirmNewGame);}
 }
}
