using System;
using System.IO;
using CosmicBlock.Board;
using CosmicBlock.Core;
using CosmicBlock.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering;
using UnityEngine.UI;
public static class Sprint0Builder {
 const string ScenePath="Assets/Scenes/Game.unity";
 [MenuItem("COSMIC BLOCK/Sprint 0/Create Board Prototype")]
 public static void Build() {
  if(File.Exists(ScenePath)) { Debug.Log("Game.unity already exists; validating without overwriting."); Validate(); return; }
  if(!Application.isBatchMode&&!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())return;
  foreach(var folder in new[]{"Art","Audio","Data","Prefabs","Scenes","Scripts/Blocks","Scripts/Effects","Scripts/Localization","Scripts/Ads"})
   Directory.CreateDirectory("Assets/"+folder);
  AssetDatabase.Refresh();
  PlayerSettings.defaultInterfaceOrientation=UIOrientation.Portrait;
  PlayerSettings.allowedAutorotateToLandscapeLeft=false;
  PlayerSettings.allowedAutorotateToLandscapeRight=false;
  PlayerSettings.allowedAutorotateToPortraitUpsideDown=false;
  PlayerSettings.defaultScreenWidth=1080; PlayerSettings.defaultScreenHeight=1920;
  PlayerSettings.Android.minSdkVersion=(AndroidSdkVersions)26;
  PlayerSettings.Android.targetSdkVersion=(AndroidSdkVersions)36;
  GraphicsSettings.defaultRenderPipeline=AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>("Assets/Settings/UniversalRP.asset");
  var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
  var camera=new GameObject("Main Camera",typeof(Camera)); camera.tag="MainCamera";
  camera.GetComponent<Camera>().clearFlags=CameraClearFlags.SolidColor;
  camera.GetComponent<Camera>().backgroundColor=new Color(.055f,.065f,.16f);
  var root=new GameObject("GameCanvas",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));
  root.GetComponent<Canvas>().renderMode=RenderMode.ScreenSpaceOverlay;
  var scaler=root.GetComponent<CanvasScaler>(); scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;
  scaler.referenceResolution=new Vector2(1080,1920); scaler.matchWidthOrHeight=.5f;
  var bg=UI("Background",root.transform); Stretch(bg);
  bg.gameObject.AddComponent<Image>().color=new Color(.055f,.065f,.16f);
  var safe=UI("SafeArea",root.transform); Stretch(safe); safe.gameObject.AddComponent<SafeArea>();
  Label("Title",safe,"COSMIC BLOCK",new Vector2(.5f,.92f),52);
  Label("ScorePlaceholder",safe,"BEST  —     SCORE  —",new Vector2(.5f,.83f),36);
  Label("TrayPlaceholder",safe,"BLOCK AREA · NEXT SPRINT",new Vector2(.5f,.16f),30);
  var board=UI("Board",safe); board.anchorMin=board.anchorMax=new Vector2(.5f,.5f);
  board.sizeDelta=new Vector2(900,900); board.gameObject.AddComponent<SquareBoardLayout>();
  var grid=board.gameObject.AddComponent<GridLayoutGroup>(); grid.constraint=GridLayoutGroup.Constraint.FixedColumnCount;
  grid.constraintCount=8; grid.spacing=new Vector2(8,8); grid.padding=new RectOffset(8,8,8,8); grid.cellSize=new Vector2(103.5f,103.5f);
  var images=new Image[64];
  for(int i=0;i<64;i++) {
   var cell=UI("Cell_"+(i%8)+"_"+(i/8),board); images[i]=cell.gameObject.AddComponent<Image>();
   images[i].color=new Color(.22f,.27f,.47f,.72f);
  }
  var view=board.gameObject.AddComponent<BoardView>(); view.Configure(images);
  new GameObject("GameSession").AddComponent<GameSession>().Configure(view);
  var events=new GameObject("EventSystem",typeof(EventSystem),typeof(InputSystemUIInputModule));
  events.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
  EditorSceneManager.SaveScene(scene,ScenePath);
  EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};
  AssetDatabase.SaveAssets(); Validate();
 }
 static RectTransform UI(string name,Transform parent) {
  var rect=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>(); rect.SetParent(parent,false); return rect;
 }
 static void Stretch(RectTransform rect) { rect.anchorMin=Vector2.zero; rect.anchorMax=Vector2.one; rect.offsetMin=rect.offsetMax=Vector2.zero; }
 static void Label(string name,Transform parent,string value,Vector2 anchor,int size) {
  var rect=UI(name,parent); rect.anchorMin=rect.anchorMax=anchor; rect.sizeDelta=new Vector2(950,100);
  var text=rect.gameObject.AddComponent<Text>(); text.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
  text.text=value; text.fontSize=size; text.alignment=TextAnchor.MiddleCenter; text.color=new Color(.9f,.82f,.62f); text.raycastTarget=false;
 }
 [MenuItem("COSMIC BLOCK/Sprint 0/Validate Prototype")]
 public static void Validate() {
  if(!Application.isBatchMode&&!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())return;
  EditorSceneManager.OpenScene(ScenePath);
  var view=UnityEngine.Object.FindAnyObjectByType<BoardView>();
  if(view==null||view.CellCount!=64||view.transform.childCount!=64)throw new Exception("Expected 64 cells.");
  var model=new BoardModel(); int changes=0; model.CellChanged+=(_,_,_)=>changes++;
  model.SetOccupied(7,7,true);
  if(!model.IsOccupied(7,7)||model.IsOccupied(0,0))throw new Exception("State isolation failed.");
  model.SetOccupied(7,7,true); model.Clear();
  if(model.IsOccupied(7,7)||changes!=2)throw new Exception("State events failed.");
  try {model.SetOccupied(8,0,true); throw new Exception("Bounds check missing.");} catch(ArgumentOutOfRangeException) {}
  if(UnityEngine.Object.FindAnyObjectByType<SafeArea>()==null||UnityEngine.Object.FindAnyObjectByType<InputSystemUIInputModule>()==null)throw new Exception("UI foundation missing.");
  Directory.CreateDirectory("Validation");
  File.WriteAllText("Validation/sprint0.txt","PASS: Scene load, 64 cells, model isolation/events/bounds, SafeArea and New Input UI module. Runtime visual/device checks are separate.");
  Debug.Log("SPRINT0_VALIDATION_PASS");
 }
}
