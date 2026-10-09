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
public static class Sprint101PlayProbe {
 static readonly Dictionary<string,int> backup=new Dictionary<string,int>();static readonly List<string> keys=new List<string>();static readonly List<string> results=new List<string>();static GameSession game;static GameFlowController flow;static PlanetCompletionPopup popup;static double began;static int frames;static string errors="";static bool expectedWriteFailure;static int expectedWarnings;
 public static void Run(){keys.Add(PlanetSelection.Key);keys.Add(PlanetRestoration.VersionKey);keys.Add(GameSession.DefaultBestScoreKey);foreach(var d in PlanetDefinitions.All){keys.Add(d.EnergyKey);keys.Add(d.UnlockKey);keys.Add(PlanetCompletionNotice.Key(d.Id));}foreach(var k in keys){if(PlayerPrefs.HasKey(k))backup[k]=PlayerPrefs.GetInt(k);PlayerPrefs.DeleteKey(k);}PlayerPrefs.SetInt(PlanetRestoration.VersionKey,2);PlayerPrefs.SetInt(GameSession.DefaultBestScoreKey,19000);foreach(var d in PlanetDefinitions.All)PlayerPrefs.SetInt(d.UnlockKey,1);PlayerPrefs.Save();EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");began=EditorApplication.timeSinceStartup;Application.logMessageReceived+=Log;EditorApplication.update+=Check;EditorApplication.EnterPlaymode();}
 static void Log(string m,string s,LogType t){if(expectedWriteFailure&&t==LogType.Warning&&m.StartsWith("Run save could not be written:")){expectedWarnings++;return;}if(t==LogType.Error||t==LogType.Exception||t==LogType.Warning)errors+=m+"\n";}
 static void Check(){if(EditorApplication.timeSinceStartup-began>240){Finish(false,"timeout");return;}if(!EditorApplication.isPlaying||++frames<30)return;EditorApplication.update-=Check;RunSaveStore.PathOverride=Path.GetFullPath("Validation/sprint101-run.json");RunSaveStore.Delete();game=UnityEngine.Object.FindAnyObjectByType<GameSession>();flow=UnityEngine.Object.FindAnyObjectByType<GameFlowController>();popup=UnityEngine.Object.FindAnyObjectByType<PlanetCompletionPopup>();game.StartCoroutine(Guard());}
 static IEnumerator Guard(){var e=Body();while(true){object v;try{if(!e.MoveNext())break;v=e.Current;}catch(Exception x){Finish(false,x.ToString());yield break;}yield return v;}Finish(errors.Length==0,errors);}
 static IEnumerator Body(){
  Req(popup!=null,"one popup controller");Req(UnityEngine.Object.FindObjectsByType<PlanetCompletionPopup>().Length==1,"no duplicate controller");
  PlanetSelection.Select(1);flow.StartNewRun();game.SaveCurrentRun();string original=File.ReadAllText(RunSaveStore.SavePath);int originalPlanet=game.Restoration.PlanetId;expectedWriteFailure=true;
  using(var fileLock=new FileStream(RunSaveStore.SavePath,FileMode.Open,FileAccess.Read,FileShare.Read)){Req(!flow.TrySwitchRunPlanet(2,1),"failed atomic write rejects next run");}expectedWriteFailure=false;
  Req(expectedWarnings==1&&File.ReadAllText(RunSaveStore.SavePath)==original&&game.Restoration.PlanetId==originalPlanet,"failed move preserves previous run bytes and model");results.Add("PASS atomic file failure preserves previous run (one deliberately induced I/O warning)");
  for(int id=1;id<=5;id++){
   var d=PlanetDefinitions.Get(id);PlayerPrefs.DeleteKey(PlanetCompletionNotice.Key(id));PlayerPrefs.SetInt(d.EnergyKey,d.Total);var legacy=new PlanetRestoration(d.EnergyKey,PlanetRestoration.VersionKey,id);Req(!PlanetCompletionNotice.Pending(id),"legacy complete acknowledged "+id);
   PlayerPrefs.SetInt(PlanetCompletionNotice.Key(id),0);PlayerPrefs.SetInt(d.EnergyKey,0);PlanetSelection.Select(id);flow.StartNewRun();game.DebugSetPlanetEnergy(d.Total-5);game.DebugPrepareNextRow();Req(game.TryPlacePiece(game.Slots[0],3,3),"real final clear "+id);Req(PlanetCompletionNotice.Pending(id)&&!popup.Visible,"persist pending before effects "+id);
   float until=Time.unscaledTime+15;while(!popup.Visible){Req(Time.unscaledTime<until,"popup deadline");yield return null;}
   Req(!game.ClearSequenceActive&&!game.Feedback.IsPlaying,"after feedback "+id);Req(game.ModalInputBlocked&&!game.TryPlacePiece(game.Slots[1],0,0),"modal input lock");game.SaveCurrentRun();string before=File.ReadAllText(RunSaveStore.SavePath);int score=game.Score;Click(popup.PopupRoot,"Confirm");
   if(id<5){Req(!popup.ConfirmationVisible&&!popup.Visible&&game.Score==score,"direct next keeps score without confirmation");while(flow.PlanetSwitchActive)yield return null;Req(game.Restoration.PlanetId==id+1&&game.State==GameState.Playing&&!game.ModalInputBlocked,"next keeps run");Req(!PlanetCompletionNotice.Pending(id)&&PlayerPrefs.GetInt(d.EnergyKey)==d.Total&&game.BestScore==19000,"ack energy BEST preserved");Req(RunSaveStore.Load().completionAcknowledgedPlanet==id,"durable transaction receipt");}
   else {Req(flow.Screen==FlowScreen.Collection&&!popup.Visible,"final planet collection");}
   results.Add("PASS real completion/cancel/next/receipt planet "+id);
  }
  PlayerPrefs.SetInt(PlanetCompletionNotice.Key(1),0);PlayerPrefs.SetInt(PlanetDefinitions.Get(1).EnergyKey,1495);PlanetSelection.Select(1);flow.StartNewRun();game.Restoration.AddEnergy(5);yield return null;Req(popup.Visible,"pending surfaced");game.SaveCurrentRun();string saved=File.ReadAllText(RunSaveStore.SavePath);int count=popup.ShowCount;Click(popup.PopupRoot,"Cancel");Req(!popup.Visible&&!game.ModalInputBlocked,"continue unlock");Req(File.ReadAllText(RunSaveStore.SavePath)==saved,"continue preserves exact score/board/pieces/combo snapshot");flow.ShowHome();flow.Play();yield return null;Req(!popup.Visible&&popup.ShowCount==count,"no repeat resume");Req(game.Restoration.CurrentEnergy==1500,"resume complete");game.DebugPrepareNextRow();Req(game.TryPlacePiece(game.Slots[0],3,3),"completed planet still playable");yield return new WaitForSecondsRealtime(1.2f);Req(!popup.Visible&&popup.ShowCount==count&&game.Restoration.CurrentEnergy==1500,"later clear no duplicate award or popup");
  PlayerPrefs.SetInt(PlanetCompletionNotice.Key(1),0);PlayerPrefs.SetInt(PlanetDefinitions.Get(1).EnergyKey,1495);game.LoadSelectedPlanet();game.Restoration.RestoreCheckpointEnergy(1500);Req(PlanetCompletionNotice.Pending(1),"checkpoint recovery queues missing notice");yield return null;Req(popup.Visible,"recovered popup");flow.ShowHome();Req(!popup.Visible&&!game.ModalInputBlocked,"home cleanup");flow.Play();yield return null;Req(popup.Visible,"pending survives home");Click(popup.PopupRoot,"Cancel");results.Add("PASS continue/resume/pending recovery/home cleanup");
 }
 static void Click(GameObject root,string name){root.transform.Find("Card/"+name).GetComponent<Button>().onClick.Invoke();}
 static void Req(bool ok,string m){if(!ok)throw new Exception(m);}
 static void Finish(bool pass,string reason){EditorApplication.update-=Check;Application.logMessageReceived-=Log;if(game!=null)game.LeaveForHome();RunSaveStore.Delete();foreach(var k in keys){if(backup.TryGetValue(k,out int v))PlayerPrefs.SetInt(k,v);else PlayerPrefs.DeleteKey(k);}PlayerPrefs.Save();File.WriteAllText("Validation/sprint101_play_tests.txt",string.Join("\n",results)+"\n"+(pass?"SPRINT101_PASS":reason));if(pass)Debug.Log("SPRINT101_PASS");else Debug.LogError(reason);EditorApplication.Exit(pass?0:1);}
}
