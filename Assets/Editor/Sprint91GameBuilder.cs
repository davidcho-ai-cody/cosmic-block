using System.IO;
using CosmicBlock.Blocks;
using CosmicBlock.Board;
using CosmicBlock.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public static class Sprint91GameBuilder {
 public static void Build(){
  const string path="Assets/Scenes/Game.unity";string original=File.ReadAllText(path);var scene=EditorSceneManager.OpenScene(path);
  var safe=GameObject.Find("GameCanvas").transform.Find("SafeArea");var presentation=safe.Find("GameScorePresentation").GetComponent<GameVisualPresentation>();
  var frame=safe.Find("BoardFrame").GetComponent<Image>();frame.preserveAspect=false;if(frame.GetComponent<GameFrameMesh>()==null)frame.gameObject.AddComponent<GameFrameMesh>();
  presentation.ConfigureLayoutMatch();safe.Find("Board").GetComponent<SquareBoardLayout>().ConfigureLayoutMatch(presentation);
  var score=presentation.ScoreText;score.fontSize=score.fontSizeMax=106;score.fontSizeMin=15;score.color=new Color(1,.83f,.38f);score.fontStyle=FontStyles.Bold;
  var label=safe.Find("GameScorePresentation/ScoreLabel").GetComponent<TMP_Text>();label.fontSize=label.fontSizeMax=32;
  var panel=safe.Find("PlanetRestoration");var status=panel.Find("StatusFrame").GetComponent<Image>();status.preserveAspect=false;var statusMesh=status.GetComponent<GameFrameMesh>();if(statusMesh==null)statusMesh=status.gameObject.AddComponent<GameFrameMesh>();statusMesh.ConfigurePlanetStatus();Set((RectTransform)panel.Find("Title"),.22f,.54f,.57f,.70f);Set((RectTransform)panel.Find("ProgressBar"),.48f,.33f,.91f,.43f);panel.Find("Stage").GetComponent<Text>().enabled=false;panel.Find("Energy").GetComponent<Text>().enabled=false;
  var title=panel.Find("Title").GetComponent<Text>();title.fontSize=title.resizeTextMaxSize=32;title.color=new Color(.91f,.97f,1);
  var percent=panel.Find("RestorationPercent").GetComponent<Text>();percent.fontSize=percent.resizeTextMaxSize=28;Set((RectTransform)percent.transform,.22f,.29f,.44f,.47f);
  var slots=safe.Find("BlockArea");var layout=slots.GetComponent<HorizontalLayoutGroup>();layout.spacing=16;layout.padding=new RectOffset(8,8,0,0);
  foreach(var piece in slots.GetComponentsInChildren<BlockPiece>(true)){piece.ConfigureRestCellLimit(78);EditorUtility.SetDirty(piece);}
  Canvas.ForceUpdateCanvases();presentation.Layout();Canvas.ForceUpdateCanvases();Sprint9GameBuilder.PreserveHome(original,safe);
  EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("SPRINT91_LAYOUT_READY");
 }
 static void Set(RectTransform r,float x0,float y0,float x1,float y1){r.anchorMin=new Vector2(x0,y0);r.anchorMax=new Vector2(x1,y1);r.offsetMin=r.offsetMax=Vector2.zero;}
}