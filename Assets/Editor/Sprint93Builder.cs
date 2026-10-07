using System.IO;
using CosmicBlock.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public static class Sprint93Builder {
 public static void Build(){
  const string path="Assets/Scenes/Game.unity";string original=File.ReadAllText(path);var scene=EditorSceneManager.OpenScene(path);
  var safe=GameObject.Find("GameCanvas").transform.Find("SafeArea");var panel=safe.Find("PlanetRestoration");
  Set(panel.Find("Title"),.22f,.54f,.57f,.70f,30);
  Set(panel.Find("Stage"),.22f,.10f,.50f,.22f,23);
  Set(panel.Find("Energy"),.56f,.10f,.92f,.22f,23);
  Set(panel.Find("RestorationPercent"),.22f,.27f,.45f,.43f,26);
  var best=safe.Find("GameScorePresentation").GetComponent<GameVisualPresentation>().BestText;
  best.fontSize=best.fontSizeMax=53.65f;best.enableAutoSizing=true;best.fontSizeMin=15;
  best.rectTransform.anchorMin=new Vector2(.40f,.17f);best.rectTransform.anchorMax=new Vector2(.90f,.59f);best.rectTransform.offsetMin=best.rectTransform.offsetMax=Vector2.zero;
  Canvas.ForceUpdateCanvases();safe.Find("GameScorePresentation").GetComponent<GameVisualPresentation>().Layout();Canvas.ForceUpdateCanvases();
  Sprint9GameBuilder.PreserveHome(original,safe);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("SPRINT93_SCENE_READY");
 }
 static void Set(Transform t,float x0,float y0,float x1,float y1,int size){var r=(RectTransform)t;r.anchorMin=new Vector2(x0,y0);r.anchorMax=new Vector2(x1,y1);r.offsetMin=r.offsetMax=Vector2.zero;var text=t.GetComponent<Text>();text.enabled=true;text.fontSize=text.resizeTextMaxSize=size;text.resizeTextMinSize=14;text.resizeTextForBestFit=true;text.alignment=TextAnchor.MiddleLeft;text.raycastTarget=false;}
}
