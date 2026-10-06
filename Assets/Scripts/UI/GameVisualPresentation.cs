using System.Globalization;
using CosmicBlock.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace CosmicBlock.UI {
 public sealed class GameVisualPresentation:MonoBehaviour {
  [SerializeField] GameSession session;[SerializeField] TMP_Text score,best;[SerializeField] Text restorationPercent;[SerializeField] PlanetRestorationView planet;
  [SerializeField] RectTransform board,boardFrame,planetPanel;[SerializeField] Image frameImage,planetFrameImage;
  public TMP_Text ScoreText=>score;public TMP_Text BestText=>best;
  public void Configure(GameSession owner,TMP_Text current,TMP_Text record,Text percent,PlanetRestorationView view,RectTransform content,Image frame,RectTransform panel,Image panelArt){session=owner;score=current;best=record;restorationPercent=percent;planet=view;board=content;boardFrame=frame.rectTransform;frameImage=frame;planetPanel=panel;planetFrameImage=panelArt;}
  public void RenderValues(int current,int record){score.text=current.ToString("N0",CultureInfo.InvariantCulture);best.text=record.ToString("N0",CultureInfo.InvariantCulture);}
  public void RefreshVisual(){if(session!=null&&session.Restoration!=null){RenderValues(session.Score,session.BestScore);restorationPercent.text="복원도 "+PlanetRestoration.PercentForTotal(planet.PresentedEnergy)+"%";}Layout();}
  public void Layout(){if(board==null)return;board.GetComponent<SquareBoardLayout>().SendMessage("LateUpdate");float side=board.rect.width;boardFrame.sizeDelta=new Vector2(side/.74f*frameImage.sprite.rect.width/frameImage.sprite.rect.height,side/.74f);var area=(RectTransform)board.parent;float width=area.rect.width*.94f,height=width*planetFrameImage.sprite.rect.height/planetFrameImage.sprite.rect.width;height=Mathf.Min(height,area.rect.height*.21f);width=height*planetFrameImage.sprite.rect.width/planetFrameImage.sprite.rect.height;planetPanel.sizeDelta=new Vector2(width,height);}
  void LateUpdate()=>RefreshVisual();
 }
}