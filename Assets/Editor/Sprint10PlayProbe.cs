using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CosmicBlock.Core;
using CosmicBlock.UI;
using CosmicBlock.Blocks;
using CosmicBlock.Board;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public static class Sprint10PlayProbe {
 static readonly int[] totals={1500,3000,6000,12000,24000};
 static readonly int[][] amounts={new[]{150,250,350,450,300},new[]{300,500,700,900,600},new[]{600,1000,1400,1800,1200},new[]{1200,2000,2800,3600,2400},new[]{2400,4000,5600,7200,4800}};
 static List<string> keys=new List<string>(),results=new List<string>();static Dictionary<string,int> saved=new Dictionary<string,int>();
 static int frames,assertions;static double started;static string errors="",warnings="";static GameSession session;static GameFlowController flow;static PlanetRestorationView view;static Image current;
 public static void Run(){
  keys.Add(PlanetSelection.Key);keys.Add(PlanetRestoration.VersionKey);keys.Add(GameSession.DefaultBestScoreKey);foreach(var d in PlanetDefinitions.All){keys.Add(d.EnergyKey);keys.Add(d.UnlockKey);}
  foreach(var k in keys){if(PlayerPrefs.HasKey(k))saved[k]=PlayerPrefs.GetInt(k);PlayerPrefs.DeleteKey(k);}PlayerPrefs.SetInt(PlanetRestoration.VersionKey,2);PlayerPrefs.SetInt(GameSession.DefaultBestScoreKey,15930);PlayerPrefs.Save();
  try{Domain();}catch(Exception e){Finish(false,e.ToString());return;}
  EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");started=EditorApplication.timeSinceStartup;Application.logMessageReceived+=Log;EditorApplication.update+=Check;EditorApplication.EnterPlaymode();
 }
 static void Domain(){Req(PlanetDefinitions.All.Length==5,"five definitions");var art=PlanetArtCatalog.Current;Req(art!=null&&art.planets.Length==5,"five common catalog entries");int sum=0;
  for(int id=1;id<=5;id++){var d=PlanetDefinitions.Get(id);Req(d.Total==totals[id-1]&&d.ContentReady,"ready/total "+id);sum+=d.Total;int start=0;
   for(int stage=1;stage<=5;stage++){Req(d.Amounts[stage-1]==amounts[id-1][stage-1],"independent phase amount");Req(PlanetRestoration.StartForStage(stage,id)==start,"cumulative boundary");
    var sprite=art.Sprite(id,stage);Req(sprite!=null&&AssetDatabase.GetAssetPath(sprite)==d.StagePaths[stage-1],"exact sprite path");var t=AssetImporter.GetAtPath(d.StagePaths[stage-1])as TextureImporter;
    Req(t.textureType==TextureImporterType.Sprite&&t.spriteImportMode==SpriteImportMode.Single&&t.alphaIsTransparency&&t.alphaSource==TextureImporterAlphaSource.FromInput&&!t.mipmapEnabled&&!t.isReadable,"sprite alpha/import");var a=t.GetPlatformTextureSettings("Android");Req(a.overridden&&a.format==TextureImporterFormat.ASTC_6x6,"ASTC RGBA Android");
    var m=new PlanetRestoration(d.EnergyKey,PlanetRestoration.VersionKey,id);m.SetEnergy(start);Req(m.Stage==stage&&m.SpriteStage==stage&&!m.IsRestored,"stage artwork not completed");Req(m.StageEnergy==0&&m.StageRequired==amounts[id-1][stage-1],"phase local zero");
    if(start>0){m.SetEnergy(start-1);Req(m.Stage==stage-1,"boundary minus1");}start+=amounts[id-1][stage-1];
   }
   var end=new PlanetRestoration(d.EnergyKey,PlanetRestoration.VersionKey,id);end.SetEnergy(d.Total-1);Req(!end.IsRestored&&end.Stage==5&&end.SpriteStage==5,"total minus1 still restoring");end.AddEnergy(1);Req(end.IsRestored&&end.Percent==100&&end.Stage==5,"total completion");if(id<5)Req(PlanetDefinitions.IsUnlocked(id+1),"next unlock");
  }Req(sum==46500,"total46500");
  PlayerPrefs.SetInt(PlanetRestoration.Planet02Key,2000);PlayerPrefs.SetInt(PlanetDefinitions.Get(3).UnlockKey,1);Req(new PlanetRestoration(PlanetRestoration.Planet02Key,PlanetRestoration.VersionKey,2).CurrentEnergy==2000,"legacy p2 absolute2000");Req(PlanetDefinitions.IsUnlocked(3),"legacy unlock not reversed");Req(PlayerPrefs.GetInt(GameSession.DefaultBestScoreKey)==15930&&PlayerPrefs.GetInt(PlanetRestoration.VersionKey)==2,"BEST/migration retained");
  foreach(var d in PlanetDefinitions.All){PlayerPrefs.SetInt(d.EnergyKey,0);PlayerPrefs.DeleteKey(d.UnlockKey);}PlayerPrefs.SetInt(PlanetSelection.Key,1);PlayerPrefs.Save();results.Add("PASS independent domain thresholds/import/25 sprites/compatibility");
 }
 static void Check(){if(EditorApplication.timeSinceStartup-started>420){Finish(false,"timeout");return;}if(!EditorApplication.isPlaying||++frames<35)return;EditorApplication.update-=Check;RunSaveStore.PathOverride=Path.GetFullPath("Validation/sprint10-run.json");RunSaveStore.Delete();session=UnityEngine.Object.FindAnyObjectByType<GameSession>();flow=UnityEngine.Object.FindAnyObjectByType<GameFlowController>();view=UnityEngine.Object.FindAnyObjectByType<PlanetRestorationView>();current=Field<Image>(view,"currentImage");session.StartCoroutine(Guard());}
 static IEnumerator Guard(){var steps=Body();while(true){object value;try{if(!steps.MoveNext())break;value=steps.Current;}catch(Exception e){Finish(false,e.ToString());yield break;}yield return value;}Finish(errors.Length==0&&warnings.Length==0,errors+warnings);}
 static IEnumerator Body(){
  flow.ShowCollection();for(int id=2;id<=5;id++){flow.CollectionView.SelectPlanet(id-1);Req(!flow.CollectionView.CanSelect&&!flow.CollectionView.Choose(),"locked selection "+id);Req(flow.CollectionView.HeroSprite==PlanetArtCatalog.Current.lockedPlanet&&flow.CollectionView.transform.Find("Hero/HeroLock").gameObject.activeSelf,"locked silhouette/icon "+id);}
  flow.CollectionView.GoBack();
  for(int id=1;id<=5;id++){
   var d=PlanetDefinitions.Get(id);Req(PlanetSelection.Select(id),"sequential select "+id);flow.StartNewRun();Req(session.Restoration.PlanetId==id,"new run selected ID");
   for(int stage=1;stage<=5;stage++){int energy=PlanetRestoration.StartForStage(stage,id);session.DebugSetPlanetEnergy(energy);Req(current.sprite==PlanetArtCatalog.Current.Sprite(id,stage)&&view.ShownStage==stage,"HUD actual sprite "+id+"/"+stage);Req(Mathf.Abs(view.ProgressFill-energy/(float)d.Total)<.00001f,"HUD overall actual fill");
    flow.ShowHome();flow.ShowCollection();flow.CollectionView.SelectPlanet(id-1);Req(flow.CollectionView.HeroSprite==current.sprite&&flow.CollectionView.CurrentStage==stage,"HUD Collection shared sprite");Req(flow.CollectionView.Status.Contains(stage+"단계")&&!flow.CollectionView.Status.Contains("복원 완료"),"phase5 not complete status");int stored=PlayerPrefs.GetInt(d.EnergyKey);flow.CollectionView.Preview(1);Req(PlayerPrefs.GetInt(d.EnergyKey)==stored,"preview read-only");flow.CollectionView.GoBack();flow.Play();Req(session.Restoration.PlanetId==id&&session.Restoration.CurrentEnergy==energy,"resume same phase");
   }
   // All four phase changes and final completion use the production coroutine timeline.
   for(int stage=1;stage<=5;stage++){flow.StartNewRun();int boundary=stage==5?d.Total:PlanetRestoration.StartForStage(stage+1,id);int before=boundary-5;session.DebugSetPlanetEnergy(before);session.DebugPrepareNextRow();int count=view.CompletionPlayCount;
    Req(session.TryPlacePiece(session.Slots[0],3,3),"boundary clear "+id+"/"+stage);int after=Math.Min(d.Total,before+10);Req(session.Score==110&&session.Restoration.CurrentEnergy==after,"score/energy immediate");Req(view.PresentedEnergy==before,"fragment delay intact");Req(!session.TryPlacePiece(session.Slots[1],0,0),"transition input lock intact");
    float deadline=Time.unscaledTime+12;while(view.FeedbackActive||session.State==GameState.Resolving){Req(Time.unscaledTime<deadline,"transition deadline");yield return null;}
    Req(view.CompletionPlayCount==count+1&&view.PresentedEnergy==after,"one transition correct overflow");Req(current.sprite==PlanetArtCatalog.Current.Sprite(id,session.Restoration.Stage)&&session.Score==110,"final correct sprite and score");Req(session.Restoration.IsRestored==(stage==5),"completion only total");
   }
   if(id<5)Req(PlanetDefinitions.IsUnlocked(id+1),"completion unlock sequential "+id);
   int completed=view.CompletionPlayCount;session.DebugPrepareNextRow();session.TryPlacePiece(session.Slots[0],3,3);yield return new WaitForSecondsRealtime(.9f);Req(view.CompletionPlayCount==completed&&session.Restoration.CurrentEnergy==d.Total&&session.State==GameState.Playing,"completed endless play no repeat");
   flow.StartNewRun();session.DebugSetPlanetEnergy(PlanetRestoration.StartForStage(3,id)+20);session.DebugSetScore(980);Req(session.TryPlacePiece(session.Slots[0],0,0),"saved board placement");var snapshot=RunSaveStore.Load();int best=session.BestScore;flow.ShowHome();Req(PlanetSelection.Select(1),"HOME other selection");session.LoadSelectedPlanet();flow.RefreshHome();flow.Play();Req(session.Restoration.PlanetId==id&&session.Score==snapshot.score&&session.Combo==snapshot.combo&&session.BestScore==best,"single run identity preserved "+id);Req(current.sprite==PlanetArtCatalog.Current.Sprite(id,3),"resumed sprite");Req(session.Restoration.CurrentEnergy==snapshot.planetEnergy,"restore no energy award");var cells=UnityEngine.Object.FindAnyObjectByType<BoardView>().CaptureCells();for(int i=0;i<64;i++)Req(cells[i]==snapshot.cells[i],"saved board exact");for(int i=0;i<3;i++)Req(session.Slots[i].Shape==BlockCatalog.Shapes[snapshot.shapes[i]]&&session.Slots[i].IsConsumed==snapshot.consumed[i]&&session.Slots[i].AppearanceIndex==snapshot.appearances[i],"saved piece exact");
   session.DebugForceGameOver();Req(!flow.HasSavedRun,"gameover invalidates");session.Retry();Req(session.Score==0&&session.State==GameState.Playing&&session.Restoration.PlanetId==id,"retry clean sameplanet");session.DebugSetPlanetEnergy(d.Total);results.Add("PASS planet "+id+" five timeline boundaries/overflow/unlock/endless/resume/retry");
  }
  for(int id=1;id<=5;id++)Req(PlanetSelection.Select(id),"all completed playable");
 }
 static T Field<T>(object o,string name)=>(T)o.GetType().GetField(name,BindingFlags.NonPublic|BindingFlags.Instance).GetValue(o);
 static void Req(bool v,string label){assertions++;if(!v)throw new Exception(label);}
 static void Log(string m,string trace,LogType t){if(t==LogType.Error||t==LogType.Exception||t==LogType.Assert)errors+=m+"\n";if(t==LogType.Warning)warnings+=m+"\n";}
 static void Finish(bool pass,string failure){EditorApplication.update-=Check;Application.logMessageReceived-=Log;if(session!=null)session.LeaveForHome();RunSaveStore.Delete();foreach(var k in keys){if(saved.TryGetValue(k,out int v))PlayerPrefs.SetInt(k,v);else PlayerPrefs.DeleteKey(k);}PlayerPrefs.Save();File.WriteAllText("Validation/sprint10_tests.txt",string.Join("\n",results)+"\nAssertions="+assertions+"\nWarnings="+warnings+"\nErrors="+errors+"\n"+(pass?"SPRINT10_PASS":failure));EditorApplication.Exit(pass?0:1);}
}
