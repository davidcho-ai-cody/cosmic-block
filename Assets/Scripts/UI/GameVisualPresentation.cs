using System.Globalization;
using CosmicBlock.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace CosmicBlock.UI {
 public sealed class GameVisualPresentation:MonoBehaviour {
  [SerializeField] GameSession session;[SerializeField] TMP_Text score,best;[SerializeField] Text restorationPercent;[SerializeField] PlanetRestorationView planet;
  [SerializeField] bool layoutMatch;
  [SerializeField] RectTransform board,boardFrame,planetPanel;[SerializeField] Image frameImage,planetFrameImage;
  public TMP_Text ScoreText=>score;public TMP_Text BestText=>best;
  public void Configure(GameSession owner,TMP_Text current,TMP_Text record,Text percent,PlanetRestorationView view,RectTransform content,Image frame,RectTransform panel,Image panelArt){session=owner;score=current;best=record;restorationPercent=percent;planet=view;board=content;boardFrame=frame.rectTransform;frameImage=frame;planetPanel=panel;planetFrameImage=panelArt;}
  public void RenderValues(int current,int record){score.text=current.ToString("N0",CultureInfo.InvariantCulture);best.text=record.ToString("N0",CultureInfo.InvariantCulture);}
  public void RefreshVisual(){if(session!=null&&session.Restoration!=null){RenderValues(session.Score,session.BestScore);restorationPercent.text="복원도 "+PlanetRestoration.PercentForTotal(planet.PresentedEnergy,planet.PresentedPlanetId)+"%";}Layout();}
  public void ConfigureLayoutMatch()=>layoutMatch=true;
  public void Layout(){if(board==null)return;if(layoutMatch){LayoutMatched();return;}board.GetComponent<SquareBoardLayout>().SendMessage("LateUpdate");float side=board.rect.width;boardFrame.sizeDelta=new Vector2(side/.74f*frameImage.sprite.rect.width/frameImage.sprite.rect.height,side/.74f);var area=(RectTransform)board.parent;float width=area.rect.width*.94f,height=width*planetFrameImage.sprite.rect.height/planetFrameImage.sprite.rect.width;height=Mathf.Min(height,area.rect.height*.21f);width=height*planetFrameImage.sprite.rect.width/planetFrameImage.sprite.rect.height;planetPanel.sizeDelta=new Vector2(width,height);}
  // All measurements are Canvas units within the existing Safe Area.
  void LayoutMatched(){
   var area=(RectTransform)board.parent;float w=area.rect.width,h=area.rect.height,q=Mathf.Min(w/1080f,h/1920f);q=Mathf.Max(.1f,q);
   // Keep the SCORE rect, font size and total vertical inset; move its baseline 8 units below the enlarged crown.
   score.fontSizeMin=best.fontSizeMin=Mathf.Min(15,15*q);score.margin=new Vector4(6,14,6,-2)*q;best.margin=new Vector4(1,1,1,1)*q;
   float statusW=w*.96f,statusH=260*q;
   float slotH=324*q,headerH=220*q,gapHud=12*q,gapBoard=16*q,gapSlot=16*q,bottomMargin=36*q;
   var assists=GetComponentInParent<Canvas>().GetComponent<GameAssistView>();if(assists!=null){bottomMargin=GameAssistView.ReservedHeight(w,q)+8*q;assists.Layout(w,q);}
   float budget=h-headerH-statusH-slotH-bottomMargin-gapHud-gapBoard-gapSlot;
   float side=Mathf.Max(0,Mathf.Min(w*.86f,budget-108*q,w*.96f-108*q));
   float frameH=side+108*q,statusTop=h-headerH-gapHud,frameTop=statusTop-statusH-gapBoard;
   Place(planetPanel,w/2,statusTop-statusH/2,statusW,statusH);var mesh=planetFrameImage.GetComponent<GameFrameMesh>();if(mesh!=null){var anchors=mesh.PlanetTrackHorizontalAnchors();var track=(RectTransform)planetPanel.Find("ProgressBar");track.anchorMin=new Vector2(anchors.x,track.anchorMin.y);track.anchorMax=new Vector2(anchors.y,track.anchorMax.y);track.offsetMin=track.offsetMax=Vector2.zero;}Place((RectTransform)planetPanel.Find("PlanetVisual"),statusH*.40f,statusH*.51f,statusH*.78f,statusH*.78f);
   Place(board,w/2,frameTop-frameH/2,side,side);Place(boardFrame,w/2,frameTop-frameH/2,frameH,frameH);boardFrame.GetComponent<GameFrameMesh>()?.SetDecorationScale(.22f*q);
   Place((RectTransform)area.Find("BlockArea"),w/2,frameTop-frameH-gapSlot-slotH/2,w*.98f,slotH);
   Place((RectTransform)area.Find("PlayingHomeButton"),100*q,h-90*q,160*q,160*q);
   Place((RectTransform)transform.Find("BestFrameArea"),w-206.25f*q,h-68.75f*q,412.5f*q,137.5f*q);
   Place((RectTransform)transform.Find("ScoreLabel"),w/2,h-55*q,w*.30f,40*q);
   Place(score.rectTransform,w/2,h-145*q,w*.46f,132*q);
   var grid=board.GetComponent<GridLayoutGroup>();float cell=Mathf.Max(0,(side-grid.padding.horizontal-grid.spacing.x*7)/8);grid.cellSize=new Vector2(cell,cell);
  }
  static void Place(RectTransform r,float x,float y,float width,float height){var area=(RectTransform)r.parent;r.anchorMin=r.anchorMax=new Vector2(.5f,.5f);r.pivot=new Vector2(.5f,.5f);r.sizeDelta=new Vector2(width,height);r.anchoredPosition=new Vector2(x-area.rect.width/2,y-area.rect.height/2);}
  void LateUpdate()=>RefreshVisual();
 }
}