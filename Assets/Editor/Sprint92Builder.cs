using System;
using System.IO;
using CosmicBlock.Board;
using CosmicBlock.Effects;
using CosmicBlock.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public static class Sprint92Builder {
 public static void Build(){
  const string scenePath="Assets/Scenes/Game.unity";string before=File.ReadAllText(scenePath);
  foreach(string name in new[]{"game_block_blue","game_block_purple","game_block_gold","game_clear_starlight","game_empty_cell"}){string path="Assets/Art/UI/Game/"+name+".png";var importer=(TextureImporter)AssetImporter.GetAtPath(path);if(importer==null)throw new Exception("Missing "+path);importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.alphaSource=TextureImporterAlphaSource.FromInput;importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.filterMode=FilterMode.Bilinear;importer.wrapMode=TextureWrapMode.Clamp;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=2048;importer.spritePixelsPerUnit=100;var settings=new TextureImporterSettings();importer.ReadTextureSettings(settings);settings.spriteMeshType=SpriteMeshType.FullRect;settings.spritePivot=new Vector2(.5f,.5f);importer.SetTextureSettings(settings);var platform=importer.GetDefaultPlatformTextureSettings();platform.maxTextureSize=2048;platform.textureCompression=TextureImporterCompression.Uncompressed;importer.SetPlatformTextureSettings(platform);importer.SaveAndReimport();}
  var scene=EditorSceneManager.OpenScene(scenePath);var safe=GameObject.Find("GameCanvas").transform.Find("SafeArea");var board=safe.Find("Board").GetComponent<BoardView>();var root=safe.Find("FeedbackLayer");var feedback=root.GetComponent<GameFeedbackController>();var art=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/Game/game_clear_starlight.png");if(art==null)throw new Exception("Starlight import failed");
  foreach(var image in root.GetComponentsInChildren<Image>(true))if(image.name.StartsWith("ClearCell_")){var density=image.GetComponent<CellSpriteDensity>();if(density==null)density=image.gameObject.AddComponent<CellSpriteDensity>();density.Configure(board.SpriteVisualScale);image.preserveAspect=true;}
  var lights=new Image[48];for(int i=0;i<lights.Length;i++){var old=root.Find("Starlight_"+i);var rect=old==null?new GameObject("Starlight_"+i,typeof(RectTransform),typeof(Image)).GetComponent<RectTransform>():(RectTransform)old;rect.SetParent(root,false);rect.anchorMin=rect.anchorMax=new Vector2(.5f,.5f);rect.sizeDelta=Vector2.one*30;lights[i]=rect.GetComponent<Image>();lights[i].sprite=art;lights[i].preserveAspect=true;lights[i].raycastTarget=false;rect.gameObject.SetActive(false);}
  foreach(var t in root.GetComponentsInChildren<Text>(true))if(t.name.StartsWith("Star_"))t.gameObject.SetActive(false);
  var combo=root.Find("ComboPop").GetComponent<Text>();combo.fontSize=52;combo.fontStyle=FontStyle.Bold;combo.alignment=TextAnchor.MiddleCenter;var glow=combo.GetComponent<Shadow>();if(glow==null)glow=combo.gameObject.AddComponent<Shadow>();glow.effectColor=new Color(.3f,.8f,1,.55f);glow.effectDistance=new Vector2(2,-2);
  foreach(var image in safe.Find("PlanetEnergyFlight").GetComponentsInChildren<Image>(true))if(image.name.StartsWith("Fragment_")){image.sprite=art;image.preserveAspect=true;}
  foreach(string textName in new[]{"ComboPop","ScorePop"}){var text=root.Find(textName).GetComponent<Text>();var outline=text.GetComponent<Outline>();if(outline==null)outline=text.gameObject.AddComponent<Outline>();outline.effectColor=new Color(.015f,.025f,.06f,.96f);outline.effectDistance=new Vector2(2,-2);outline.useGraphicAlpha=true;}
  feedback.ConfigureStarlights(lights);Canvas.ForceUpdateCanvases();Sprint9GameBuilder.PreserveHome(before,safe);EditorUtility.SetDirty(feedback);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("SPRINT92_SCENE_READY");
 }
}
