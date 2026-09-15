using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using CosmicBlock.Core;
using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI {
 public sealed class GameHud:MonoBehaviour {
  [SerializeField] Text scoreText,comboText,finalScoreText,journeyText,reachedText;
  [SerializeField] GameObject gameOverPanel;
  [SerializeField] Button retryButton,adPlaceholderButton;
  [SerializeField] Image journeyFill;
  [SerializeField] CanvasGroup reachedFeedback;
  public int ReachedCount=>reachedThisRun.Count; GameSession session; readonly HashSet<int> reachedThisRun=new HashSet<int>(); Coroutine feedbackRoutine;
  public void Configure(Text score,Text combo,GameObject panel,Text finalScore,Button retry,Button ad){scoreText=score;comboText=combo;gameOverPanel=panel;finalScoreText=finalScore;retryButton=retry;adPlaceholderButton=ad;}
  public void ConfigureJourney(Text journey,Image fill,CanvasGroup feedback,Text feedbackText){journeyText=journey;journeyFill=fill;reachedFeedback=feedback;reachedText=feedbackText;}
  public void Connect(GameSession owner){if(session!=null)retryButton.onClick.RemoveListener(session.Retry);session=owner;retryButton.onClick.AddListener(session.Retry);adPlaceholderButton.interactable=false;reachedThisRun.Clear();}
  public void Render(GameSession owner){
   string score=owner.Score.ToString("N0",CultureInfo.InvariantCulture),best=owner.BestScore.ToString("N0",CultureInfo.InvariantCulture);
   scoreText.text="BEST  "+best+"     SCORE  "+score;
   comboText.text=owner.Combo==0?"":owner.Combo<3?"COMBO "+owner.Combo:owner.Combo==3?"STAR COMBO 3":"COSMIC COMBO "+owner.Combo;
   var j=owner.Journey;
   journeyText.text=j.Next.HasValue?j.Current.Name+"  >  "+j.Next.Value.Name+"\n"+score+" / "+j.Next.Value.Score.ToString("N0",CultureInfo.InvariantCulture):j.Current.Name+"  -  JOURNEY COMPLETE\n"+score;
   journeyFill.fillAmount=j.Progress;
   string route=j.Next.HasValue?j.Current.Name+"  >  "+j.Next.Value.Name:j.Current.Name;
   finalScoreText.text="SCORE  "+score+"\n\nJOURNEY\n"+route+"  "+Mathf.RoundToInt(j.Progress*100)+"%\n\nBEST  "+best+"\nBEST JOURNEY  "+owner.BestJourney.Current.Name;
   gameOverPanel.SetActive(owner.State==GameState.GameOver);
  }
  public void NotifyJourneyCrossings(int before,int after){if(after<=before)return;foreach(var m in JourneyProgress.Milestones)if(m.Score>0&&before<m.Score&&after>=m.Score&&reachedThisRun.Add(m.Score))ShowReached(m.Name);}
  void ShowReached(string destination){if(reachedFeedback==null)return;if(feedbackRoutine!=null)StopCoroutine(feedbackRoutine);feedbackRoutine=StartCoroutine(AnimateReached(destination));}
  IEnumerator AnimateReached(string destination){reachedText.text="DESTINATION REACHED\n"+destination;reachedFeedback.gameObject.SetActive(true);var rect=(RectTransform)reachedFeedback.transform;for(float t=0;t<.22f;t+=Time.unscaledDeltaTime){float p=t/.22f;reachedFeedback.alpha=p;rect.localScale=Vector3.one*Mathf.Lerp(.92f,1.04f,p);yield return null;}reachedFeedback.alpha=1;rect.localScale=Vector3.one;yield return new WaitForSecondsRealtime(1.15f);for(float t=0;t<.35f;t+=Time.unscaledDeltaTime){reachedFeedback.alpha=1-t/.35f;yield return null;}reachedFeedback.alpha=0;reachedFeedback.gameObject.SetActive(false);feedbackRoutine=null;}
  public void ResetJourneyFeedback(){reachedThisRun.Clear();if(feedbackRoutine!=null)StopCoroutine(feedbackRoutine);feedbackRoutine=null;if(reachedFeedback!=null){reachedFeedback.alpha=0;reachedFeedback.gameObject.SetActive(false);}}
  void OnDestroy(){if(session!=null&&retryButton!=null)retryButton.onClick.RemoveListener(session.Retry);}
 }
}
