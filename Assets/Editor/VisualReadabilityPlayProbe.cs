using System;
using System.Collections.Generic;
using System.IO;
using CosmicBlock.Blocks;
using CosmicBlock.Board;
using CosmicBlock.Core;
using CosmicBlock.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public static class VisualReadabilityPlayProbe {
 static double started;static int frames,stage;static string errors="";static GameSession session;static BoardView board;static RectTransform layer;static BlockPiece[] pieces;static readonly List<string> results=new List<string>();
 public static void Run(){EditorSceneManager.OpenScene(VisualReadabilityBuilder.ScenePath);started=EditorApplication.timeSinceStartup;Application.logMessageReceived+=Log;EditorApplication.update+=Check;EditorApplication.EnterPlaymode();}
 static void Log(string m,string st,LogType t){if(t==LogType.Error||t==LogType.Exception||t==LogType.Assert)errors+=m+"\n";}
 static void Check(){if(EditorApplication.timeSinceStartup-started>150){Finish(false,"timeout");return;}if(!EditorApplication.isPlaying||++frames<30)return;try{if(stage==0){session=UnityEngine.Object.FindAnyObjectByType<GameSession>();board=UnityEngine.Object.FindAnyObjectByType<BoardView>();layer=GameObject.Find("GameCanvas").transform.Find("DragLayer") as RectTransform;pieces=new BlockPiece[session.Slots.Count];for(int i=0;i<pieces.Length;i++)pieces[i]=session.Slots[i];
  Require(board.GetComponent<Image>()!=null&&board.GetComponent<Outline>()!=null,"Test 1 board container");for(int i=0;i<64;i++){var image=board.transform.GetChild(i).GetComponent<Image>();Require(image.color==BoardView.EmptyFill&&image.GetComponent<Outline>().effectColor==BoardView.EmptyBorder,"Test 1 empty cell "+i);}results.Add("PASS Test 1: Board container and 64 dark-fill/cyan-outline cells.");
  foreach(var p in pieces)Require(p.transform.parent.GetComponent<SlotVisual>()!=null&&p.transform.parent.GetComponent<Outline>()!=null,"Test 2 slots");results.Add("PASS Test 2: three independent slot panels preserve pieces and touch roots.");
  TestShape(2,new Vector2Int(1,1),3,"Horizontal 3");TestShape(4,new Vector2Int(5,1),3,"Vertical 3");TestShape(5,new Vector2Int(3,3),4,"Square 2x2");TestShape(6,new Vector2Int(1,5),3,"L");
  session.Retry();pieces[0].Initialize(BlockCatalog.Shapes[2],board,layer,session);board.Model.SetOccupied(2,2,true);DragTo(0,new Vector2Int(1,2),false);Require(!board.PreviewValid&&board.PreviewCount==3&&AllPreview(BoardView.InvalidBorder),"Test 9 occupied invalid");Cancel(0);results.Add("PASS Test 9: collision marks the whole H3 preview red.");
  pieces[0].Initialize(BlockCatalog.Shapes[2],board,layer,session);DragTo(0,new Vector2Int(7,4),false);Require(!board.PreviewValid&&board.PreviewCount==1&&AllPreview(BoardView.InvalidBorder),"Test 10 boundary invalid count="+board.PreviewCount+" valid="+board.PreviewValid+" outline="+AllPreview(BoardView.InvalidBorder));string before=Sprint1Builder.Snapshot(board.Model);End(0,new Vector2Int(7,4));Require(Sprint1Builder.Snapshot(board.Model)==before&&!pieces[0].IsConsumed&&pieces[0].transform.parent.name=="Slot_0"&&board.PreviewCount==0,"Test 10/11/12 failed cleanup");results.Add("PASS Tests 10-12: boundary red, failed drop inert, slot/scale/border/preview restored.");
  Require(errors.Length==0,"runtime errors "+errors);stage=1;frames=0;return;}Finish(true,"");}catch(Exception e){Finish(false,e.ToString());}}
 static void TestShape(int shape,Vector2Int anchor,int count,string name){session.Retry();pieces[0].Initialize(BlockCatalog.Shapes[shape],board,layer,session);var slot=pieces[0].transform.parent.GetComponent<SlotVisual>();DragTo(0,anchor,true);Require(board.PreviewValid&&board.PreviewCount==count&&AllPreview(BoardView.ValidBorder)&&pieces[0].Rect.localScale==Vector3.one*1.05f&&slot.IsSelected,"Tests 3-7 "+name);string before=Sprint1Builder.Snapshot(board.Model);End(0,anchor);Require(board.PreviewCount==0&&pieces[0].IsConsumed&&Sprint1Builder.Count(board.Model)==count&&before!=Sprint1Builder.Snapshot(board.Model),"Test 8 "+name);results.Add("PASS "+name+": full shape gold outline, selected slot, exact valid drop and cleanup.");}
 static bool AllPreview(Color color){for(int i=0;i<64;i++){var o=board.transform.GetChild(i).GetComponent<Outline>();if(o.effectColor==color)continue;if(board.transform.GetChild(i).GetComponent<Image>().color==BoardView.ValidFill||board.transform.GetChild(i).GetComponent<Image>().color==BoardView.InvalidFill)return false;}return true;}
 static Camera Cam(){var c=layer.GetComponentInParent<Canvas>();return c.renderMode==RenderMode.ScreenSpaceOverlay?null:c.worldCamera;}
 static Vector2 Screen(Vector3 w)=>RectTransformUtility.WorldToScreenPoint(Cam(),w);
 static PointerEventData Data(int index,Vector2Int anchor){Canvas.ForceUpdateCanvases();var piece=pieces[index];float pitch=board.CellSize+board.CellSpacing;Vector3 center=board.GetCellWorld(anchor)+layer.TransformVector(new Vector3((piece.Shape.Width-1)*pitch/2,-(piece.Shape.Height-1)*pitch/2-session.DragFingerOffset));return new PointerEventData(EventSystem.current){pointerId=-1,position=Screen(center)};}
 static void DragTo(int index,Vector2Int anchor,bool valid){var begin=new PointerEventData(EventSystem.current){pointerId=-1,position=Screen(pieces[index].transform.position)};ExecuteEvents.Execute(pieces[index].gameObject,begin,ExecuteEvents.beginDragHandler);ExecuteEvents.Execute(pieces[index].gameObject,Data(index,anchor),ExecuteEvents.dragHandler);Require(board.PreviewValid==valid,"preview validity");}
 static void End(int index,Vector2Int anchor)=>ExecuteEvents.Execute(pieces[index].gameObject,Data(index,anchor),ExecuteEvents.endDragHandler);
 static void Cancel(int index)=>ExecuteEvents.Execute(pieces[index].gameObject,new BaseEventData(EventSystem.current),ExecuteEvents.cancelHandler);
 static void Require(bool c,string m){if(!c)throw new Exception("Visual readability test failed: "+m);}
 static void Finish(bool pass,string failure){EditorApplication.update-=Check;Application.logMessageReceived-=Log;Directory.CreateDirectory("Validation");File.WriteAllText("Validation/visual_readability_play.txt",string.Join("\n",results)+"\n"+(pass?"VISUAL_READABILITY_PLAY_PASS":failure));if(pass)Debug.Log("VISUAL_READABILITY_PLAY_PASS");else Debug.LogError(failure);EditorApplication.Exit(pass?0:1);}
}