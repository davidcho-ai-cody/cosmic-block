using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CosmicBlock.Core;
using CosmicBlock.Board;
using CosmicBlock.Blocks;
using CosmicBlock.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class ClearScorePlayProbe {
 static GameSession session;static BoardView board;static GameFlowController flow;static GameVisualPresentation visual;
 static double started;static int frames,count;static string errors="",warnings="";static List<string> results=new List<string>();
 static Dictionary<string,int> saved=new Dictionary<string,int>();static List<string> keys=new List<string>();
 public static void Run(){
  keys.Add(GameSession.DefaultBestScoreKey);keys.Add(PlanetRestoration.VersionKey);keys.Add(PlanetSelection.Key);
  foreach(var d in PlanetDefinitions.All){keys.Add(d.EnergyKey);keys.Add(d.UnlockKey);}
  foreach(var k in keys){if(PlayerPrefs.HasKey(k))saved[k]=PlayerPrefs.GetInt(k);PlayerPrefs.DeleteKey(k);}
  PlayerPrefs.SetInt(PlanetRestoration.VersionKey,2);PlayerPrefs.SetInt(PlanetRestoration.DefaultKey,1500);PlayerPrefs.Save();
  EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");started=EditorApplication.timeSinceStartup;
  Application.logMessageReceived+=Log;EditorApplication.update+=Check;EditorApplication.EnterPlaymode();
 }
 static void Check(){if(EditorApplication.timeSinceStartup-started>180){Finish(false,"timeout");return;}if(!EditorApplication.isPlaying||++frames<35)return;
  EditorApplication.update-=Check;RunSaveStore.PathOverride=Path.GetFullPath("Validation/clear-score-isolated.json");RunSaveStore.Delete();
  session=UnityEngine.Object.FindAnyObjectByType<GameSession>();board=UnityEngine.Object.FindAnyObjectByType<BoardView>();flow=UnityEngine.Object.FindAnyObjectByType<GameFlowController>();visual=UnityEngine.Object.FindAnyObjectByType<GameVisualPresentation>();
  session.StartCoroutine(Guard());
 }
 static IEnumerator Guard(){var body=Tests();while(true){object v;try{if(!body.MoveNext())break;v=body.Current;}catch(Exception e){Finish(false,e.ToString());yield break;}yield return v;}Finish(errors.Length==0&&warnings.Length==0,errors+warnings);}
 static IEnumerator Tests(){
  Req(UnityEngine.Object.FindObjectsByType<GameSession>().Length==1,"single session");
  Req(UnityEngine.Object.FindObjectsByType<GameVisualPresentation>().Length==1,"single SCORE HUD");
  var shapes=new[]{BlockCatalog.Shapes[0],BlockCatalog.Shapes[0],BlockCatalog.Shapes[1],BlockCatalog.Shapes[0]};
  var expected=new[]{10,110,120,260};
  for(int i=0;i<4;i++){
   Prepare(shapes[i],i>0,i==3);int before=session.Feedback.PlayCount;
   Req(session.TryPlacePiece(session.Slots[0],3,3),"placement case "+i);
   Req(RunSaveStore.Load().score==expected[i],"immediate logical checkpoint case "+i);
   if(i>0){Req(session.Score==shapes[i].Cells.Count*10+100,"first actual clear score");Req(session.Feedback.LastScoreDelta==100,"CLEAR +100");}
   yield return new WaitForSecondsRealtime(1.4f);yield return null;
   Verify(expected[i],"case "+i);Req(session.Feedback.PlayCount-before==(i==0?0:i==3?2:1),"feedback once per line");
   if(i==3)Req(session.Feedback.LastScoreDelta==150,"second line popup +150");
  }
  Prepare(BlockCatalog.Shapes[0],true,false);session.TryPlacePiece(session.Slots[0],3,3);yield return new WaitForSecondsRealtime(.9f);
  session.DebugPrepareNextRow();Req(session.TryPlacePiece(session.Slots[0],3,3),"placement combo2");yield return new WaitForSecondsRealtime(.9f);yield return null;
  Req(session.Combo==2,"placement combo2 state");Verify(220,"two successive single-line clears, +110 each");
  flow.RequestHome();flow.ConfirmHome();flow.Play();yield return null;Verify(220,"HOME resume");
  session.DebugPrepareNextRow();session.TryPlacePiece(session.Slots[0],3,3);yield return new WaitForSecondsRealtime(.9f);yield return null;Verify(330,"clear after HOME resume");
  Prepare(BlockCatalog.Shapes[0],true,true);session.TryPlacePiece(session.Slots[0],3,3);
  Req(session.Score==110&&RunSaveStore.Load().score==260,"first frame actual110 vs complete checkpoint260");
  session.SendMessage("OnApplicationPause",true);Req(session.Score==260,"pause settles pending second line");
  flow.ShowHome();flow.Play();yield return new WaitForSecondsRealtime(1.2f);yield return null;Verify(260,"immediate clear save and resume no duplicate or overwrite");
  Prepare(BlockCatalog.Shapes[0],true,false);session.TryPlacePiece(session.Slots[0],3,3);
  flow.RequestHome();flow.ConfirmHome();flow.Play();yield return new WaitForSecondsRealtime(1.2f);yield return null;Verify(110,"HOME during CLEAR resumes full score");
  results.Add("RULES UNCHANGED: 1 cell/no clear10; 1 cell/1 line110; 2 cells/1 line120; 1 cell/2 lines260; placement Combo2 adds110.");
 }
 static void Prepare(BlockShape shape,bool row,bool col){flow.StartNewRun();session.DebugSetPlanetEnergy(1500);
  var layer=GameObject.Find("GameCanvas").transform.Find("DragLayer")as RectTransform;foreach(var piece in session.Slots)piece.Initialize(shape,board,layer,session);
  for(int y=0;y<8;y++)for(int x=0;x<8;x++)if(row&&y==3||col&&x==3){bool gap=false;foreach(var c in shape.Cells)if(x==3+c.x&&y==3+c.y)gap=true;if(!gap){board.Model.SetOccupied(x,y,true);board.PaintPlacement(BlockCatalog.Shapes[0],x,y,board.BlockSprite(0));}}
 }
 static void Verify(int expected,string label){Req(session.Score==expected,label+" actual SCORE");Req(visual.ScoreText.text==expected.ToString("N0"),label+" visible TMP SCORE");Req(RunSaveStore.Load().score==expected,label+" disk score");Req(session.BestScore>=expected,label+" BEST");Req(PlayerPrefs.GetInt(GameSession.DefaultBestScoreKey)==session.BestScore,label+" BEST saved");Req(visual.BestText.text==session.BestScore.ToString("N0"),label+" visible BEST");results.Add("PASS "+label+" SCORE/HUD/SAVE="+expected+" BEST="+session.BestScore);}
 static void Req(bool v,string label){count++;if(!v)throw new Exception(label);}
 static void Log(string m,string trace,LogType t){if(t==LogType.Error||t==LogType.Exception||t==LogType.Assert)errors+=m+"\n";if(t==LogType.Warning)warnings+=m+"\n";}
 static void Finish(bool pass,string why){EditorApplication.update-=Check;Application.logMessageReceived-=Log;if(session!=null)session.LeaveForHome();RunSaveStore.Delete();foreach(var k in keys){if(saved.TryGetValue(k,out int v))PlayerPrefs.SetInt(k,v);else PlayerPrefs.DeleteKey(k);}PlayerPrefs.Save();File.WriteAllText("Validation/clear_score_tests.txt",string.Join("\n",results)+"\nAssertions="+count+"\nWarnings="+warnings+"\nErrors="+errors+"\n"+(pass?"CLEAR_SCORE_PASS":why));EditorApplication.Exit(pass?0:1);}
}
