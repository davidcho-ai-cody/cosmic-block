using System;
using System.IO;
using CosmicBlock.Board;
using CosmicBlock.Blocks;
using CosmicBlock.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public static class VisualReadabilityBuilder {
 public const string ScenePath="Assets/Scenes/Game.unity";
 [MenuItem("COSMIC BLOCK/Visual/Build Readability And Preview")]
 public static void Build(){
  if(!Application.isBatchMode&&!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())return;
  var scene=EditorSceneManager.OpenScene(ScenePath);MissingScriptDiagnostics.RemoveMissingScriptsInOpenScene();var board=UnityEngine.Object.FindAnyObjectByType<BoardView>();var area=GameObject.Find("GameCanvas").transform.Find("SafeArea/BlockArea");
  if(board==null||board.transform.childCount!=64||area==null||area.childCount!=3)throw new Exception("Sprint 3 scene foundation missing.");
  var panel=board.GetComponent<Image>();if(panel==null)panel=board.gameObject.AddComponent<Image>();panel.color=new Color(.018f,.035f,.10f,.46f);panel.raycastTarget=false;
  var boardBorder=board.GetComponent<Outline>();if(boardBorder==null)boardBorder=board.gameObject.AddComponent<Outline>();boardBorder.effectColor=new Color(.24f,.58f,.78f,.30f);boardBorder.effectDistance=new Vector2(2,-2);boardBorder.useGraphicAlpha=false;
  for(int i=0;i<64;i++){var cell=board.transform.GetChild(i).GetComponent<Image>();var outline=cell.GetComponent<Outline>();if(outline==null)outline=cell.gameObject.AddComponent<Outline>();outline.effectColor=BoardView.EmptyBorder;outline.effectDistance=new Vector2(1.5f,-1.5f);outline.useGraphicAlpha=false;}
  foreach(Transform child in area){var image=child.GetComponent<Image>();if(image==null)image=child.gameObject.AddComponent<Image>();var outline=child.GetComponent<Outline>();if(outline==null)outline=child.gameObject.AddComponent<Outline>();var slot=child.GetComponent<SlotVisual>();if(slot==null)slot=child.gameObject.AddComponent<SlotVisual>();var drag=child.GetComponent<SlotDragHandler>();if(drag==null)drag=child.gameObject.AddComponent<SlotDragHandler>();drag.Configure(child.GetComponentInChildren<BlockPiece>(true));slot.SetSelected(false);}
  EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Validate();Debug.Log("VISUAL_READABILITY_SCENE_READY");
 }
 [MenuItem("COSMIC BLOCK/Visual/Validate Readability Scene")]
 public static void Validate(){
  var board=UnityEngine.Object.FindAnyObjectByType<BoardView>();Require(board!=null&&board.GetComponent<Image>()!=null&&board.GetComponent<Outline>()!=null,"Board container");
  for(int i=0;i<64;i++)Require(board.transform.GetChild(i).GetComponent<Outline>()!=null,"Cell outline "+i);
  var area=GameObject.Find("GameCanvas").transform.Find("SafeArea/BlockArea");Require(area.childCount==3,"Three slots");foreach(Transform child in area)Require(child.GetComponent<SlotVisual>()!=null&&child.GetComponent<Outline>()!=null&&child.GetComponent<SlotDragHandler>()!=null,"Slot visual/input "+child.name);
  Directory.CreateDirectory("Validation");File.WriteAllText("Validation/visual_readability.txt","PASS scene: board container, 64 outlined cells, three independent outlined slots.\n");
 }
 public static void Require(bool condition,string message){if(!condition)throw new Exception("Visual readability test failed: "+message);}
}
