using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using CosmicBlock.Core;
using CosmicBlock.Board;
using System;
using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI {
 public sealed class GameHud:MonoBehaviour {
  [SerializeField] Text scoreText,comboText,finalScoreText,journeyText,reachedText;
  [SerializeField] GameObject gameOverPanel;
  [SerializeField] Button retryButton,adPlaceholderButton;
  [SerializeField] Image journeyFill;
  [SerializeField] CanvasGroup reachedFeedback;
  [SerializeField] PlanetRestorationView planetView;
  public int ReachedCount=>reachedThisRun.Count;
  public bool IsJourneyFeedbackVisible=>reachedFeedback!=null&&reachedFeedback.gameObject.activeSelf&&reachedFeedback.alpha>0;
  public string JourneyFeedbackText=>reachedText==null?string.Empty:reachedText.text;
  public GameObject JourneyFeedbackRoot=>reachedFeedback==null?null:reachedFeedback.gameObject;
  public GameObject GameOverRoot=>gameOverPanel;
  public PlanetRestorationView PlanetView=>planetView;
  GameSession session; readonly HashSet<int> reachedThisRun=new HashSet<int>(); Coroutine feedbackRoutine;
  public void Configure(Text score,Text combo,GameObject panel,Text finalScore,Button retry,Button ad){scoreText=score;comboText=combo;gameOverPanel=panel;finalScoreText=finalScore;retryButton=retry;adPlaceholderButton=ad;}
  public void ConfigureJourney(Text journey,Image fill,CanvasGroup feedback,Text feedbackText){journeyText=journey;journeyFill=fill;reachedFeedback=feedback;reachedText=feedbackText;}
  public void ConfigurePlanet(PlanetRestorationView view){planetView=view;}
  public void Connect(GameSession owner){if(session!=null)retryButton.onClick.RemoveListener(session.Retry);session=owner;retryButton.onClick.AddListener(session.Retry);adPlaceholderButton.interactable=false;reachedThisRun.Clear();}
  public void Render(GameSession owner){
   string score=owner.Score.ToString("N0",CultureInfo.InvariantCulture),best=owner.BestScore.ToString("N0",CultureInfo.InvariantCulture);
   scoreText.text="최고  "+best+"     점수  "+score;
   if(comboText!=null){comboText.text=string.Empty;comboText.gameObject.SetActive(false);}
   var j=owner.Journey;
   journeyText.text=j.Next.HasValue?j.Current.Name+"  >  "+j.Next.Value.Name+"\n"+score+" / "+j.Next.Value.Score.ToString("N0",CultureInfo.InvariantCulture):j.Current.Name+"  -  JOURNEY COMPLETE\n"+score;
   journeyFill.fillAmount=j.Progress;
   string route=j.Next.HasValue?j.Current.Name+"  >  "+j.Next.Value.Name:j.Current.Name;
   finalScoreText.text="점수  "+score+"\n\n"+(owner.Restoration.IsRestored?"행성 01 · "+PlanetRestoration.Planet01DisplayName+" · 복원 완료 ✓":"행성 01 · "+PlanetRestoration.Planet01DisplayName+"\n복원 단계 "+owner.Restoration.Stage+" / 5 · 전체 "+owner.Restoration.Percent+"%")+"\n\n최고  "+best;
   gameOverPanel.SetActive(owner.State==GameState.GameOver);
   if(planetView!=null)planetView.Render(owner.Restoration,true);
  }
  public void NotifyJourneyCrossings(int before,int after){if(journeyText==null||!journeyText.gameObject.activeInHierarchy||after<=before)return;foreach(var m in JourneyProgress.Milestones)if(m.Score>0&&before<m.Score&&after>=m.Score&&reachedThisRun.Add(m.Score))ShowReached(m.Name);}
  void ShowReached(string destination){if(reachedFeedback==null)return;if(feedbackRoutine!=null)StopCoroutine(feedbackRoutine);feedbackRoutine=StartCoroutine(AnimateReached(destination));}
  IEnumerator AnimateReached(string destination){reachedText.text="DESTINATION REACHED\n"+destination;reachedFeedback.gameObject.SetActive(true);var rect=(RectTransform)reachedFeedback.transform;for(float t=0;t<.22f;t+=Time.unscaledDeltaTime){float p=t/.22f;reachedFeedback.alpha=p;rect.localScale=Vector3.one*Mathf.Lerp(.92f,1.04f,p);yield return null;}reachedFeedback.alpha=1;rect.localScale=Vector3.one;yield return new WaitForSecondsRealtime(1.15f);for(float t=0;t<.35f;t+=Time.unscaledDeltaTime){reachedFeedback.alpha=1-t/.35f;yield return null;}reachedFeedback.alpha=0;reachedFeedback.gameObject.SetActive(false);feedbackRoutine=null;}
  public void ResetJourneyFeedback(){
   reachedThisRun.Clear();if(feedbackRoutine!=null)StopCoroutine(feedbackRoutine);feedbackRoutine=null;
   if(reachedText!=null)reachedText.text=string.Empty;
   if(reachedFeedback!=null){reachedFeedback.alpha=0;reachedFeedback.transform.localScale=Vector3.one;reachedFeedback.gameObject.SetActive(false);}
  }
  public bool PresentPlanetEnergy(LineClearResult clear,BoardView board,int before,int after,Action complete){if(planetView==null){complete?.Invoke();return false;}planetView.RenderEnergy(before,false);return planetView.PresentClear(clear,board,before,after,complete);}
  public void ResetPlanetFeedback(){if(planetView!=null)planetView.ResetTransient();}
  void OnDestroy(){if(session!=null&&retryButton!=null)retryButton.onClick.RemoveListener(session.Retry);}
 }
}
