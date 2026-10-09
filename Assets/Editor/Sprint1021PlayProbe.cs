using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CosmicBlock.Core;
using CosmicBlock.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CosmicBlock.Blocks;
using CosmicBlock.Board;
using System.Reflection;
public static class Sprint1021PlayProbe {
 static readonly Dictionary<string,int> backup=new Dictionary<string,int>();static readonly List<string> keys=new List<string>();static readonly List<string> results=new List<string>();static GameSession game;static GameFlowController flow;static PlanetCompletionPopup popup;static double began;static int frames;static string errors="";static bool expectedWriteFailure;static int expectedWarnings;
 public static void Run(){keys.Add(PlanetSelection.Key);keys.Add(PlanetRestoration.VersionKey);keys.Add(GameSession.DefaultBestScoreKey);foreach(var d in PlanetDefinitions.All){keys.Add(d.EnergyKey);keys.Add(d.UnlockKey);keys.Add(PlanetCompletionNotice.Key(d.Id));}foreach(var k in keys){if(PlayerPrefs.HasKey(k))backup[k]=PlayerPrefs.GetInt(k);PlayerPrefs.DeleteKey(k);}PlayerPrefs.SetInt(PlanetRestoration.VersionKey,2);PlayerPrefs.SetInt(GameSession.DefaultBestScoreKey,19000);foreach(var d in PlanetDefinitions.All)PlayerPrefs.SetInt(d.UnlockKey,1);PlayerPrefs.Save();EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");began=EditorApplication.timeSinceStartup;Application.logMessageReceived+=Log;EditorApplication.update+=Check;EditorApplication.EnterPlaymode();}
 static void Log(string m,string s,LogType t){if(expectedWriteFailure&&t==LogType.Warning&&m.StartsWith("Run save could not be written:")){expectedWarnings++;return;}if(t==LogType.Error||t==LogType.Exception||t==LogType.Warning)errors+=m+"\n";}
 static void Check(){if(EditorApplication.timeSinceStartup-began>240){Finish(false,"timeout");return;}if(!EditorApplication.isPlaying||++frames<30)return;EditorApplication.update-=Check;RunSaveStore.PathOverride=Path.GetFullPath("Validation/sprint1021-run.json");RunSaveStore.Delete();game=UnityEngine.Object.FindAnyObjectByType<GameSession>();flow=UnityEngine.Object.FindAnyObjectByType<GameFlowController>();popup=UnityEngine.Object.FindAnyObjectByType<PlanetCompletionPopup>();game.StartCoroutine(Guard());}
 static IEnumerator Guard(){var e=Body();while(true){object v;try{if(!e.MoveNext())break;v=e.Current;}catch(Exception x){Finish(false,x.ToString());yield break;}yield return v;}Finish(errors.Length==0,errors);}
 static IEnumerator Body(){
  var popup=UnityEngine.Object.FindAnyObjectByType<PlanetCompletionPopup>();
  var hud=UnityEngine.Object.FindAnyObjectByType<GameHud>();
  for(int id=1;id<=2;id++){
   PlayerPrefs.SetInt(PlanetDefinitions.Get(id).EnergyKey,PlanetDefinitions.Get(id).Total);PlayerPrefs.SetInt(PlanetCompletionNotice.Key(id),2);
   PlayerPrefs.SetInt(PlanetDefinitions.Get(id+1).EnergyKey,37);PlanetSelection.Select(id);flow.StartNewRun();game.DebugSetScore(230);game.DebugPrepareNextRow();
   Req(game.TryPlacePiece(game.Slots[1],0,0),"색상 보드/소비 슬롯 준비");yield return new WaitForSecondsRealtime(.3f);
   Req(game.TryRefreshBlocks(),"새로고침 사용");yield return new WaitForSecondsRealtime(.3f);Req(game.TryHint(),"힌트 사용");
   // Completion blocks assists, so expose the pending notice only after the Run fixture is prepared.
   typeof(GameSession).GetField("<Combo>k__BackingField",BindingFlags.NonPublic|BindingFlags.Instance).SetValue(game,3);
   game.SaveCurrentRun();var before=Snapshot();int best=game.BestScore;var slotObjects=new BlockPiece[3];for(int n=0;n<3;n++)slotObjects[n]=game.Slots[n];
   PlayerPrefs.SetInt(PlanetCompletionNotice.Key(id),1);popup.Show(id);popup.PopupRoot.transform.Find("Card/Confirm").GetComponent<Button>().onClick.Invoke();
   var visual=UnityEngine.Object.FindAnyObjectByType<GameVisualPresentation>();visual.RefreshVisual();Req(hud.PlanetView.PresentedPlanetId==id&&((Text)typeof(GameVisualPresentation).GetField("restorationPercent",BindingFlags.NonPublic|BindingFlags.Instance).GetValue(visual)).text=="복원도 100%","Fade Out의 이전 행성/퍼센트 유지");
   Req(flow.PlanetSwitchActive&&game.ModalInputBlocked&&!game.TryHint()&&!game.TryRefreshBlocks(),"전환 입력 차단");Req(!popup.Visible&&!popup.ConfirmationVisible,"추가 확인창 없음");
   var saved=RunSaveStore.Load();Req(saved.planetId==id+1&&SameRun(before,saved),"전환 시작 시 원자적 저장");
   while(flow.PlanetSwitchActive){Req(game.Score==before.score&&Snapshot().cells[0]==before.cells[0],"전환 중 점수/보드 유지");yield return null;}
   Req(SameRun(before,Snapshot())&&game.BestScore==best&&game.RunActive,"SCORE/보드색상/슬롯/Combo/횟수 유지");
   for(int n=0;n<3;n++)Req(game.Slots[n]==slotObjects[n],"Piece 객체 재생성 없음");
   Req(game.Restoration.CurrentEnergy==37&&PlayerPrefs.GetInt(PlanetDefinitions.Get(id).EnergyKey)==PlanetDefinitions.Get(id).Total,"별빛 이월 없음");
   Req(!PlanetCompletionNotice.Pending(id)&&hud.PlanetView.PresentedEnergy==37&&!game.HintVisible&&!game.ModalInputBlocked,"HUD/피드백 초기화");
   game.DebugPrepareNextRow();Req(game.TryPlacePiece(game.Slots[0],3,3),"새 행성 실제 Line Clear");game.DebugCompleteTurnForProbe();Req(game.Restoration.CurrentEnergy==47,"새 별빛만 +10");
   flow.ShowHome();flow.Play();Req(game.Restoration.PlanetId==id+1&&game.Restoration.CurrentEnergy==47,"HOME 이어하기");
   flow.ShowHome();Req(game.ResumeSavedRun(),"재실행 동일 복원 경로");Req(game.Restoration.PlanetId==id+1&&game.RefreshUses==1&&game.HintUses==1,"저장 횟수 유지");flow.Play();
   results.Add("PASS 행성 "+id+"→"+(id+1)+" SCORE/보드/슬롯/Combo/횟수/별빛/저장/전환");
  }
  flow.ShowHome();flow.ShowCollection();int old=game.Restoration.PlanetId;PlayerPrefs.SetInt(PlanetDefinitions.Get(5).UnlockKey,0);PlayerPrefs.SetInt(PlanetDefinitions.Get(4).EnergyKey,0);Req(!flow.SelectCollectionPlanet(5),"잠긴 행성 차단");
  var beforeCollection=RunSaveStore.Load();Req(flow.SelectCollectionPlanet(1),"완료 행성 도감 선택");while(flow.PlanetSwitchActive)yield return null;Req(flow.Screen==FlowScreen.Game&&SameRun(beforeCollection,Snapshot())&&game.Restoration.PlanetId==1,"도감 현재Run 보존");
  Req(game.Restoration.IsRestored,"완료 행성 복귀");game.DebugPrepareNextRow();Req(game.TryPlacePiece(game.Slots[0],3,3),"완료 행성 계속 플레이");game.DebugCompleteTurnForProbe();yield return null;Req(!popup.Visible&&game.Restoration.IsRestored,"완료 보상/팝업 반복 없음");
  flow.ShowHome();flow.ShowCollection();Req(flow.SelectCollectionPlanet(1)&&!flow.PlanetSwitchActive,"동일 행성 불필요한 전환 없음");results.Add("PASS 도감 잠금/완료 행성 복귀/동일 선택");
  game.SaveCurrentRun();string original=File.ReadAllText(RunSaveStore.SavePath);int selected=PlanetSelection.Current;expectedWriteFailure=true;
  using(var fileLock=new FileStream(RunSaveStore.SavePath,FileMode.Open,FileAccess.Read,FileShare.Read)){Req(!flow.TrySwitchRunPlanet(2),"저장 실패 이동 거부");}expectedWriteFailure=false;
  Req(File.ReadAllText(RunSaveStore.SavePath)==original&&game.Restoration.PlanetId==1&&PlanetSelection.Current==selected&&!flow.PlanetSwitchActive,"실패 시 모델/선택/Run 보존");
  Req(flow.TrySwitchRunPlanet(2,1),"전환 중 종료 fixture");var committed=RunSaveStore.Load();flow.ShowHome();Req(RunSaveStore.Write(committed),"중단 시 확정 snapshot 유지");PlanetSelection.Select(1);PlayerPrefs.SetInt(PlanetCompletionNotice.Key(1),1);game.LoadSelectedPlanet();flow.Play();Req(game.Restoration.PlanetId==2&&PlanetSelection.Current==2&&!PlanetCompletionNotice.Pending(1)&&SameRun(committed,Snapshot())&&!flow.PlanetSwitchActive&&!game.ModalInputBlocked,"전환 중 HOME 정리 및 거래 복원");results.Add("PASS 원자적 실패/중단 복구");
  PlayerPrefs.SetInt(PlanetDefinitions.Get(5).UnlockKey,1);PlayerPrefs.SetInt(PlanetDefinitions.Get(5).EnergyKey,24000);PlayerPrefs.SetInt(PlanetCompletionNotice.Key(5),1);Req(flow.TrySwitchRunPlanet(5),"05 이동");while(flow.PlanetSwitchActive){Req(!popup.Visible,"전환 중 다음 완료 팝업 지연");yield return null;}yield return null;Req(popup.Visible&&game.ModalInputBlocked,"05 완료 팝업 입력 차단");Req(popup.PopupRoot.transform.Find("Card/Confirm").GetComponentInChildren<Text>().text=="행성 도감","05 다음 행성 버튼 없음");popup.Continue();
  Req(flow.TrySwitchRunPlanet(2),"pause 전환");flow.SendMessage("OnApplicationPause",true);game.SendMessage("OnApplicationPause",true);Req(!flow.PlanetSwitchActive&&!game.ModalInputBlocked&&game.Restoration.PlanetId==2,"pause 전환 cleanup");game.SendMessage("OnApplicationPause",false);
  game.DebugForceGameOver();Req(game.State==GameState.GameOver&&!flow.HasSavedRun,"Game Over 저장 종료");game.Retry();Req(game.Score==0&&game.Combo==0&&game.RefreshUses==0&&game.HintUses==0&&!game.Slots[0].IsConsumed,"Retry 새Run");
  game.DebugSetScore(230);flow.ShowHome();flow.StartNewRun();Req(game.Score==0&&game.Combo==0&&game.RefreshUses==0&&game.HintUses==0,"명시적 새Run 초기화");
  flow.ShowHome();RunSaveStore.Delete();flow.ShowCollection();Req(flow.SelectCollectionPlanet(2)&&flow.Screen==FlowScreen.Home,"Run 없음 기존 선택 흐름");flow.Play();Req(game.Score==0&&game.Restoration.PlanetId==2,"Run 없음 새게임");results.Add("PASS 05/계속/GameOver/Retry/새게임/Run 없음");
 }
 static bool SameRun(RunSnapshot a,RunSnapshot b){return SameBoard(a,b)&&a.score==b.score&&a.combo==b.combo&&a.blockSetNumber==b.blockSetNumber&&a.refreshUses==b.refreshUses&&a.hintUses==b.hintUses&&Equal(a.shapes,b.shapes)&&Equal(a.appearances,b.appearances)&&Equal(a.consumed,b.consumed);}
 static bool Equal<T>(T[] a,T[] b){for(int i=0;i<a.Length;i++)if(!EqualityComparer<T>.Default.Equals(a[i],b[i]))return false;return true;}
 static RunSnapshot Snapshot()=>(RunSnapshot)typeof(GameSession).GetMethod("Capture",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(game,new object[]{false});
 static void Shapes(BlockShape shape)=>typeof(GameSession).GetMethod("DebugSetShapes",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(game,new object[]{shape});
 static bool SameBoard(RunSnapshot a,RunSnapshot b){for(int i=0;i<64;i++)if(a.cells[i]!=b.cells[i])return false;return true;}
 static void Click(GameObject root,string name){root.transform.Find("Card/"+name).GetComponent<Button>().onClick.Invoke();}
 static void Req(bool ok,string m){if(!ok)throw new Exception(m);}
 static void Finish(bool pass,string reason){EditorApplication.update-=Check;Application.logMessageReceived-=Log;if(game!=null)game.LeaveForHome();RunSaveStore.Delete();foreach(var k in keys){if(backup.TryGetValue(k,out int v))PlayerPrefs.SetInt(k,v);else PlayerPrefs.DeleteKey(k);}PlayerPrefs.Save();File.WriteAllText("Validation/sprint1021_play_tests.txt",string.Join("\n",results)+"\n"+(pass?"SPRINT1021_PASS":reason));if(pass)Debug.Log("SPRINT1021_PASS");else Debug.LogError(reason);EditorApplication.Exit(pass?0:1);}
}
