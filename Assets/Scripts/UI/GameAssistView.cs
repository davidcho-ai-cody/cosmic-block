using System.Collections.Generic;
using CosmicBlock.Core;
using CosmicBlock.Board;
using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI {
 public sealed class GameAssistView:MonoBehaviour {
  [SerializeField] GameSession session;[SerializeField] GameFlowController flow;[SerializeField] BoardView board;[SerializeField] Sprite refreshSprite,hintSprite;
  RectTransform slotArea;
  RectTransform root;Button refresh,hint;Text refreshCount,hintCount;readonly List<GameObject> marks=new List<GameObject>();GameObject slotFrame;
  public bool AllowsInteraction=>flow!=null&&flow.Screen==FlowScreen.Game&&!flow.HomeConfirmVisible;
  public Button RefreshButton=>refresh;public Button HintButton=>hint;public RectTransform Root=>root;public int HighlightCount=>marks.Count;
  public void Configure(GameSession s,GameFlowController f,BoardView b,Sprite r,Sprite h){session=s;flow=f;board=b;refreshSprite=r;hintSprite=h;}
  void Awake(){var safe=flow.HomeRoot.transform.parent;slotArea=(RectTransform)safe.Find("BlockArea");root=new GameObject("GameAssistActions",typeof(RectTransform)).GetComponent<RectTransform>();root.SetParent(safe,false);root.SetSiblingIndex(safe.Find("BlockArea").GetSiblingIndex()+1);root.anchorMin=root.anchorMax=Vector2.zero;root.pivot=Vector2.zero;root.anchoredPosition=Vector2.zero;refresh=MakeButton("BlockRefresh",refreshSprite,out refreshCount);hint=MakeButton("Hint",hintSprite,out hintCount);refresh.onClick.AddListener(()=>session.TryRefreshBlocks());hint.onClick.AddListener(()=>session.TryHint());session.ConfigureAssists(this);root.gameObject.SetActive(false);}
  Button MakeButton(string name,Sprite sprite,out Text count){var go=new GameObject(name,typeof(RectTransform),typeof(Image),typeof(Button));go.transform.SetParent(root,false);var image=go.GetComponent<Image>();image.sprite=sprite;image.preserveAspect=true;var button=go.GetComponent<Button>();button.transition=Selectable.Transition.ColorTint;var colors=button.colors;colors.disabledColor=new Color(.35f,.4f,.5f,.75f);button.colors=colors;
   var label=new GameObject("Uses",typeof(RectTransform),typeof(Text));label.transform.SetParent(root,false);count=label.GetComponent<Text>();count.font=flow.HomeRoot.GetComponentInChildren<Text>(true).font;count.fontSize=24;count.alignment=TextAnchor.MiddleCenter;count.color=new Color(.78f,.92f,1);count.raycastTarget=false;return button;}
  public static float ReservedHeight(float w,float q){float width=Mathf.Min(w*.42f,440*q);return width/(1922f/818)+60*q;}
  public void Layout(float w,float q){if(root==null)return;root.sizeDelta=new Vector2(w,ReservedHeight(w,q));float width=Mathf.Min(w*.42f,440*q),height=width/(1922f/818);Place((RectTransform)refresh.transform,w*.25f,48*q+height/2,width,height);Place((RectTransform)hint.transform,w*.75f,48*q+height/2,width,height);Place(refreshCount.rectTransform,w*.25f,25*q,w*.47f,30*q);Place(hintCount.rectTransform,w*.75f,25*q,w*.47f,30*q);refreshCount.fontSize=hintCount.fontSize=Mathf.Max(16,Mathf.RoundToInt(24*q));
   // Follow the slot group's updated rect, without waiting for child layout rebuilds.
   var safe=(RectTransform)root.parent;
   float slotBottom=safe.InverseTransformPoint(slotArea.TransformPoint(new Vector2(0,slotArea.rect.yMin))).y-safe.rect.yMin;
   float y=slotBottom-12*q-(48*q+height);
   root.anchoredPosition=new Vector2(0,Mathf.Clamp(y,8*q,Mathf.Max(8*q,safe.rect.height-root.rect.height)));
  }
  static void Place(RectTransform r,float x,float y,float w,float h){r.anchorMin=r.anchorMax=Vector2.zero;r.pivot=Vector2.one*.5f;r.anchoredPosition=new Vector2(x,y);r.sizeDelta=new Vector2(w,h);}
  void LateUpdate(){if(root==null)return;UpdateHintGeometry();root.gameObject.SetActive(flow.Screen==FlowScreen.Game);refresh.interactable=session.CanUseAssist&&session.RefreshRemaining>0;hint.interactable=session.CanUseAssist&&session.HintRemaining>0&&!session.HintVisible;refreshCount.text=session.RefreshRemaining>0?"무료 "+session.RefreshRemaining+"/1":"무료 0/1 · 광고 보상 준비 중";hintCount.text=session.HintRemaining>0?"무료 "+session.HintRemaining+"/3":"무료 0/3 · 광고 보상 준비 중";}
  public bool ShowHint(PlacementHint recommendation){ClearHint();if(board==null||root==null||!board.Model.CanPlace(recommendation.Shape,recommendation.Anchor.x,recommendation.Anchor.y))return false;
   foreach(var cell in recommendation.Shape.Cells){var go=new GameObject("HintCell",typeof(RectTransform),typeof(Image),typeof(Outline));go.transform.SetParent(board.transform,false);var r=go.GetComponent<RectTransform>();r.anchorMin=r.anchorMax=r.pivot=Vector2.one*.5f;r.position=board.GetCellWorld(recommendation.Anchor+cell);r.sizeDelta=Vector2.one*board.CellSize*.92f;var image=go.GetComponent<Image>();image.raycastTarget=false;image.color=new Color(.52f,.8f,1,.20f);var outline=go.GetComponent<Outline>();outline.effectColor=new Color(.6f,.88f,1,.95f);outline.effectDistance=new Vector2(3,-3);var ignore=go.AddComponent<LayoutElement>();ignore.ignoreLayout=true;marks.Add(go);}
   slotFrame=new GameObject("RecommendedPiece",typeof(RectTransform));slotFrame.transform.SetParent(session.Slots[recommendation.Slot].transform,false);var frame=slotFrame.GetComponent<RectTransform>();frame.anchorMin=Vector2.zero;frame.anchorMax=Vector2.one;frame.offsetMin=new Vector2(-4,-4);frame.offsetMax=new Vector2(4,4);
   for(int i=0;i<4;i++){var line=new GameObject("Border"+i,typeof(RectTransform),typeof(Image));line.transform.SetParent(frame,false);var image=line.GetComponent<Image>();image.color=new Color(.6f,.88f,1,.9f);image.raycastTarget=false;var r=image.rectTransform;r.anchorMin=i<2?new Vector2(0,i):new Vector2(i-2,0);r.anchorMax=i<2?new Vector2(1,i):new Vector2(i-2,1);r.sizeDelta=i<2?new Vector2(0,3):new Vector2(3,0);r.anchoredPosition=Vector2.zero;}return true;}
  void UpdateHintGeometry(){if(!session.HintVisible)return;int i=0;foreach(var cell in session.ActiveHint.Shape.Cells){if(i>=marks.Count)break;var r=marks[i++].GetComponent<RectTransform>();r.position=board.GetCellWorld(session.ActiveHint.Anchor+cell);r.sizeDelta=Vector2.one*board.CellSize*.92f;}}
  public void ClearHint(){foreach(var mark in marks){if(mark!=null){mark.SetActive(false);Destroy(mark);}}marks.Clear();if(slotFrame!=null){slotFrame.SetActive(false);Destroy(slotFrame);}slotFrame=null;}
  void OnDestroy(){ClearHint();if(root!=null)Destroy(root.gameObject);}
 }
}
