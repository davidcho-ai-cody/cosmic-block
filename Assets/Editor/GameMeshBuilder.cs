using System.Collections.Generic;
using CosmicBlock.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
public static class GameMeshBuilder {
 public static void Run(){
  var scene=EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");var safe=GameObject.Find("GameCanvas").transform.Find("SafeArea");
  var medium=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/CosmicPopupMedium.asset");var semi=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/CosmicPopupSemiBold.asset");foreach(var f in new[]{medium,semi}){f.atlasPopulationMode=AtlasPopulationMode.Dynamic;f.ClearFontAssetData();if(!f.TryAddCharacters(CollectionMeshBuilder.Characters))throw new System.Exception("GAME glyph missing");f.atlasPopulationMode=AtlasPopulationMode.Static;EditorUtility.SetDirty(f);foreach(var a in f.atlasTextures)EditorUtility.SetDirty(a);}
  foreach(string path in new[]{"BoardFrame","PlanetRestoration/StatusFrame","GameScorePresentation/BestFrameArea/BestFrame","PlayingHomeButton","BlockArea/Slot_0","BlockArea/Slot_1","BlockArea/Slot_2"}){
   var t=safe.Find(path);var image=t.GetComponent<Image>();image.color=Color.clear;foreach(var e in t.GetComponents<Shadow>())e.enabled=false;
   var face=GameMeshPresentation.Surface(t,t.GetComponent<Button>()!=null);if(t.GetComponent<Button>()!=null){image.raycastTarget=false;t.GetComponent<Button>().targetGraphic=face;}
   if(path.StartsWith("BlockArea")){(t.GetComponent<GameSlotMesh>()??t.gameObject.AddComponent<GameSlotMesh>()).Configure(face);face.Style(new Color(.025f,.055f,.13f,.88f),new Color(.24f,.63f,.85f,.75f),24,1.5f);}
  }
  var home=safe.Find("PlayingHomeButton");var icon=GameMeshPresentation.Node(home,"MeshIcon");GameMeshPresentation.Anchor(icon,.26f,.25f,.74f,.75f);(icon.GetComponent<GameMeshIconGraphic>()??icon.gameObject.AddComponent<GameMeshIconGraphic>()).Configure(GameMeshIcon.Home);
  var scores=safe.Find("GameScorePresentation");var visual=scores.GetComponent<GameVisualPresentation>();visual.ScoreText.font=semi;visual.ScoreText.fontStyle=FontStyles.Normal;visual.BestText.font=semi;visual.BestText.fontStyle=FontStyles.Normal;scores.Find("ScoreLabel").GetComponent<TMP_Text>().font=medium;
  var best=scores.Find("BestFrameArea/BestFrame");var fit=best.GetComponent<AspectRatioFitter>();if(fit!=null)Object.DestroyImmediate(fit);GameMeshPresentation.Anchor((RectTransform)best,0,0,1,1);
  var title=GameMeshPresentation.NewText(best,"MeshBestTitle",medium);title.text="최고 점수";GameMeshPresentation.Anchor(title.rectTransform,.29f,.55f,.94f,.90f);
  var trophy=GameMeshPresentation.Node(best,"MeshTrophy");GameMeshPresentation.Anchor(trophy,.055f,.22f,.25f,.78f);var tg=trophy.GetComponent<HomeMenuIconGraphic>()??trophy.gameObject.AddComponent<HomeMenuIconGraphic>();tg.Configure(HomeMenuIcon.Trophy);tg.color=new Color(1,.84f,.5f);
  var captions=new List<GameMeshPresentation.Caption>();foreach(string n in new[]{"Title","RestorationPercent","Stage","Energy"}){var old=safe.Find("PlanetRestoration/"+n).GetComponent<Text>();old.enabled=false;var tmp=GameMeshPresentation.NewText(old.transform,"MeshLabel",n=="Title"?semi:medium);GameMeshPresentation.Anchor(tmp.rectTransform,0,0,1,1);tmp.alignment=n=="Title"||n=="Stage"?TextAlignmentOptions.Left:TextAlignmentOptions.Center;captions.Add(new GameMeshPresentation.Caption{source=old,target=tmp,size=n=="Title"?35:n=="RestorationPercent"?30:26});}
  var bar=safe.Find("PlanetRestoration/ProgressBar");bar.GetComponent<Image>().color=Color.clear;var track=GameMeshPresentation.Surface(bar,false);track.Style(new Color(.02f,.035f,.085f,.98f),new Color(.36f,.62f,.82f,.85f),14,1.4f);var fill=bar.Find("Fill");fill.GetComponent<Image>().color=Color.clear;GameMeshPresentation.Surface(fill,false).Style(new Color(.10f,.78f,.94f),new Color(.28f,.89f,1,.9f),14,0);
  foreach(var cell in safe.Find("Board").GetComponentsInChildren<Image>()){if(cell.name.StartsWith("Cell")){var face=GameMeshPresentation.Surface(cell.transform,false);GameMeshPresentation.Anchor(face.rectTransform,-.10f,-.10f,1.10f,1.10f);var effect=cell.GetComponent<GameEmptyCellMesh>()??cell.gameObject.AddComponent<GameEmptyCellMesh>();effect.Configure(face);}} var presentation=scores.GetComponent<GameMeshPresentation>()??scores.gameObject.AddComponent<GameMeshPresentation>();presentation.Configure(captions.ToArray(),medium,semi);
  EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("GAME_MESH_READY");EditorApplication.Exit(0);
 }
}
