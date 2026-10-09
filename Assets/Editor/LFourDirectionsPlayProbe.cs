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
public static class LFourDirectionsPlayProbe {
 static readonly Dictionary<string,int> backup=new Dictionary<string,int>();static readonly List<string> keys=new List<string>();static readonly List<string> results=new List<string>();static GameSession game;static GameFlowController flow;static PlanetCompletionPopup popup;static double began;static int frames;static string errors="";static bool expectedWriteFailure;static int expectedWarnings;
 public static void Run(){keys.Add(PlanetSelection.Key);keys.Add(PlanetRestoration.VersionKey);keys.Add(GameSession.DefaultBestScoreKey);foreach(var d in PlanetDefinitions.All){keys.Add(d.EnergyKey);keys.Add(d.UnlockKey);keys.Add(PlanetCompletionNotice.Key(d.Id));}foreach(var k in keys){if(PlayerPrefs.HasKey(k))backup[k]=PlayerPrefs.GetInt(k);PlayerPrefs.DeleteKey(k);}PlayerPrefs.SetInt(PlanetRestoration.VersionKey,2);PlayerPrefs.SetInt(GameSession.DefaultBestScoreKey,19000);foreach(var d in PlanetDefinitions.All)PlayerPrefs.SetInt(d.UnlockKey,1);PlayerPrefs.Save();EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");began=EditorApplication.timeSinceStartup;Application.logMessageReceived+=Log;EditorApplication.update+=Check;EditorApplication.EnterPlaymode();}
 static void Log(string m,string s,LogType t){if(expectedWriteFailure&&t==LogType.Warning&&m.StartsWith("Run save could not be written:")){expectedWarnings++;return;}if(t==LogType.Error||t==LogType.Exception||t==LogType.Warning)errors+=m+"\n";}
 static void Check(){if(EditorApplication.timeSinceStartup-began>240){Finish(false,"timeout");return;}if(!EditorApplication.isPlaying||++frames<30)return;EditorApplication.update-=Check;RunSaveStore.PathOverride=Path.GetFullPath("Validation/l_four-run.json");RunSaveStore.Delete();game=UnityEngine.Object.FindAnyObjectByType<GameSession>();flow=UnityEngine.Object.FindAnyObjectByType<GameFlowController>();popup=UnityEngine.Object.FindAnyObjectByType<PlanetCompletionPopup>();game.StartCoroutine(Guard());}
 static IEnumerator Guard(){var e=Body();while(true){object v;try{if(!e.MoveNext())break;v=e.Current;}catch(Exception x){Finish(false,x.ToString());yield break;}yield return v;}Finish(errors.Length==0,errors);}
 static IEnumerator Body(){
  var board=UnityEngine.Object.FindAnyObjectByType<BoardView>();
  var layer=(RectTransform)GameObject.Find("GameCanvas").transform.Find("DragLayer");
  string[] ids={"single","horizontal_2","horizontal_3","vertical_2","vertical_3","square_2","l_small","reverse_l_small","l_small_top","reverse_l_small_top"};
  Req(BlockCatalog.Shapes.Count==10,"catalog size");for(int i=0;i<10;i++)Req(BlockCatalog.Shapes[i].Id==ids[i],"stable index "+i);
  var expected=new[]{new[]{0,8,9},new[]{1,8,9},new[]{0,1,8},new[]{0,1,9}};
  var seen=new HashSet<string>();var gen=new BlockGenerator(42);for(int i=0;i<10000;i++)seen.Add(gen.Next().Id);Req(seen.Count==10,"seeded random supplies all ten shapes");
  for(int index=6;index<10;index++){
   var shape=BlockCatalog.Shapes[index];Req(shape.Cells.Count==3&&shape.Width==2&&shape.Height==2,"three cells/2x2 "+shape.Id);
   foreach(var c in shape.Cells)Req(Array.IndexOf(expected[index-6],c.y*8+c.x)>=0,"exact offsets "+shape.Id);
   flow.StartNewRun();Shapes(shape);var p=game.Slots[0];var h=p.GetComponent<BlockDragHandler>();
   h.OnBeginDrag(Data(p,new Vector2Int(1,1),board,layer));h.OnDrag(Data(p,new Vector2Int(1,1),board,layer));Req(h.IsDragging&&p.DragHighlightActive&&board.PreviewValid&&board.PreviewCount==3,"valid drag/preview "+shape.Id);h.OnEndDrag(Data(p,new Vector2Int(1,1),board,layer));Req(p.IsConsumed&&game.Score==30,"drop "+shape.Id);
   flow.StartNewRun();Shapes(shape);p=game.Slots[0];h=p.GetComponent<BlockDragHandler>();foreach(var c in shape.Cells)game.Model.SetOccupied(1+c.x,1+c.y,true);
   h.OnBeginDrag(Data(p,new Vector2Int(1,1),board,layer));h.OnDrag(Data(p,new Vector2Int(1,1),board,layer));Req(!board.PreviewValid,"invalid preview "+shape.Id);h.OnEndDrag(Data(p,new Vector2Int(1,1),board,layer));Req(!p.IsConsumed&&game.Score==0,"invalid no placement");
   flow.StartNewRun();Shapes(shape);for(int y=0;y<8;y++)for(int x=0;x<8;x++)game.Model.SetOccupied(x,y,true);foreach(var c in shape.Cells)game.Model.SetOccupied(2+c.x,2+c.y,false);
   Req(game.HasPlaceableRemainingBlock(),"only exact L cavity fits "+shape.Id);var hint=PlacementHint.Find(game.Model,game.Slots);Req(hint!=null&&hint.Shape==shape&&hint.Anchor==new Vector2Int(2,2),"hint exact L cavity");game.EvaluateGameOver();Req(game.State==GameState.Playing,"not false gameover");game.Model.SetOccupied(2+shape.Cells[0].x,2+shape.Cells[0].y,true);game.EvaluateGameOver();Req(game.State==GameState.GameOver,"blocked L gameover");
   flow.StartNewRun();Shapes(shape);Req(game.TryPlacePiece(game.Slots[0],1,1),"save placement");var save=RunSaveStore.Load();Req(save.shapes[1]==index,"saved index");flow.ShowHome();flow.Play();Req(game.Slots[1].Shape==shape&&game.Score==30&&game.Slots[0].IsConsumed,"resume exact shape/state");
   flow.StartNewRun();Shapes(shape);int row=2;for(int x=0;x<8;x++)game.Model.SetOccupied(x,row,true);foreach(var c in shape.Cells)if(c.y==0)game.Model.SetOccupied(2+c.x,row,false);Req(game.TryPlacePiece(game.Slots[0],2,row)&&game.LastClear.LineCount==1&&game.Score==130,"line clear score unchanged");game.DebugCompleteTurnForProbe();
   results.Add("PASS "+shape.Id+": coordinates/drag/preview/drop/hint/gameover/save/resume/clear");
  }
  seen.Clear();for(int n=0;n<200&&seen.Count<10;n++){flow.StartNewRun();Req(game.TryRefreshBlocks(),"refresh allowed");foreach(var p in game.Slots)seen.Add(p.Shape.Id);yield return null;}Req(seen.Count==10,"actual refresh supplies all shapes");results.Add("PASS existing IDs/indexes retained; random/refresh all ten shapes");
 }
 static PointerEventData Data(BlockPiece p,Vector2Int anchor,BoardView board,RectTransform layer){Canvas.ForceUpdateCanvases();float pitch=board.CellSize+board.CellSpacing;var canvas=layer.GetComponentInParent<Canvas>();Camera cam=canvas.renderMode==RenderMode.ScreenSpaceOverlay?null:canvas.worldCamera;Vector3 center=board.GetCellWorld(anchor)+layer.TransformVector(new Vector3((p.Shape.Width-1)*pitch/2,-(p.Shape.Height-1)*pitch/2-game.DragFingerOffset));return new PointerEventData(EventSystem.current){pointerId=-1,position=RectTransformUtility.WorldToScreenPoint(cam,center)};}
 static RunSnapshot Snapshot()=>(RunSnapshot)typeof(GameSession).GetMethod("Capture",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(game,new object[]{false});
 static void Shapes(BlockShape shape)=>typeof(GameSession).GetMethod("DebugSetShapes",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(game,new object[]{shape});
 static bool SameBoard(RunSnapshot a,RunSnapshot b){for(int i=0;i<64;i++)if(a.cells[i]!=b.cells[i])return false;return true;}
 static void Click(GameObject root,string name){root.transform.Find("Card/"+name).GetComponent<Button>().onClick.Invoke();}
 static void Req(bool ok,string m){if(!ok)throw new Exception(m);}
 static void Finish(bool pass,string reason){EditorApplication.update-=Check;Application.logMessageReceived-=Log;if(game!=null)game.LeaveForHome();RunSaveStore.Delete();foreach(var k in keys){if(backup.TryGetValue(k,out int v))PlayerPrefs.SetInt(k,v);else PlayerPrefs.DeleteKey(k);}PlayerPrefs.Save();File.WriteAllText("Validation/l_four_play_tests.txt",string.Join("\n",results)+"\n"+(pass?"L_FOUR_PASS":reason));if(pass)Debug.Log("L_FOUR_PASS");else Debug.LogError(reason);EditorApplication.Exit(pass?0:1);}
}
