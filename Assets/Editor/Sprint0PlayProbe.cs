using System;
using System.IO;
using CosmicBlock.Board;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public static class Sprint0PlayProbe {
 static double started;
 static int frames;
 public static void Run() {
  EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");
  started=EditorApplication.timeSinceStartup;
  EditorApplication.update+=Check;
  EditorApplication.EnterPlaymode();
 }
 static void Check() {
  if(EditorApplication.timeSinceStartup-started>90) {Fail("Play mode timeout");return;}
  if(!EditorApplication.isPlaying)return;
  if(++frames<30)return;
  try {
   var view=UnityEngine.Object.FindFirstObjectByType<BoardView>();
   if(view==null||view.Model==null||view.CellCount!=64)throw new Exception("Runtime binding failed.");
   Canvas.ForceUpdateCanvases();
   var rect=((RectTransform)view.transform).rect;
   if(rect.width<=0||Mathf.Abs(rect.width-rect.height)>.1f)throw new Exception("Board aspect failed.");
   var grid=view.GetComponent<GridLayoutGroup>();
   if(grid.cellSize.x<=0||Mathf.Abs(grid.cellSize.x-grid.cellSize.y)>.1f)throw new Exception("Cell aspect failed.");
   view.Model.SetOccupied(0,0,true);
   var first=view.transform.GetChild(0).GetComponent<Image>();
   var second=view.transform.GetChild(1).GetComponent<Image>();
   if(first.color==second.color)throw new Exception("State visual update failed.");
   view.Model.Clear();
   if(first.color!=second.color)throw new Exception("Clear visual update failed.");
   File.AppendAllText("Validation/sprint0.txt","\nPASS: Play Mode binding, square board/cells, state-to-Image update and reset. Board="+rect.width+"x"+rect.height);
   Debug.Log("SPRINT0_PLAY_PASS");
   EditorApplication.update-=Check;
   EditorApplication.Exit(0);
  } catch(Exception ex) {Fail(ex.ToString());}
 }
 static void Fail(string message) {Debug.LogError(message);EditorApplication.update-=Check;EditorApplication.Exit(1);}
}

