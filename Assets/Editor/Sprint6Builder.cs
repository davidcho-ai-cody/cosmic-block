using System;
using System.Collections.Generic;
using CosmicBlock.Core;
using CosmicBlock.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Sprint6Builder
{
 public const string ScenePath="Assets/Scenes/Game.unity";
 static readonly string[] Paths={"Assets/Art/Planets/Planet01/Planet01_Stage01_Desolate.png","Assets/Art/Planets/Planet01/Planet01_Stage02_Awakening.png","Assets/Art/Planets/Planet01/Planet01_Stage03_Recovering.png","Assets/Art/Planets/Planet01/Planet01_Stage04_Thriving.png","Assets/Art/Planets/Planet01/Planet01_Stage05_Restored.png"};
 static readonly Color Gold=new Color(1,.78f,.35f,1),Blue=new Color(.45f,.8f,1,1),Navy=new Color(.025f,.045f,.12f,.88f);
 [MenuItem("COSMIC BLOCK/Sprint 6/Build Planet Restoration")]
 public static void Build()
 {
  ConfigureImports();AssetDatabase.Refresh();
  var scene=EditorSceneManager.OpenScene(ScenePath);var canvas=GameObject.Find("GameCanvas");var safe=canvas.transform.Find("SafeArea");var hud=UnityEngine.Object.FindAnyObjectByType<GameHud>();
  if(safe==null||hud==null)throw new Exception("Scene foundation missing.");
  var old=safe.Find("PlanetRestoration");if(old!=null)UnityEngine.Object.DestroyImmediate(old.gameObject);
  var journey=safe.Find("Journey");if(journey!=null)journey.gameObject.SetActive(false);if(hud.JourneyFeedbackRoot!=null)hud.JourneyFeedbackRoot.SetActive(false);
  var panel=UI("PlanetRestoration",safe);Set(panel,new Vector2(.05f,.735f),new Vector2(.95f,.872f));var bg=panel.gameObject.AddComponent<Image>();bg.color=Navy;bg.raycastTarget=false;
  var root=UI("PlanetVisual",panel);Set(root,new Vector2(.02f,.04f),new Vector2(.30f,.96f));
  var current=ImageOn(UI("Current",root));Stretch(current.rectTransform);current.preserveAspect=true;current.raycastTarget=false;
  var next=ImageOn(UI("Next",root));Stretch(next.rectTransform);next.preserveAspect=true;next.raycastTarget=false;next.gameObject.SetActive(false);
  var title=TextOn(UI("Title",panel),"PLANET 01",26,Gold,FontStyle.Bold);Set(title.rectTransform,new Vector2(.32f,.60f),new Vector2(.72f,.96f));
  var percent=TextOn(UI("Percent",panel),"0%",30,Gold,FontStyle.Bold);Set(percent.rectTransform,new Vector2(.72f,.60f),new Vector2(.97f,.96f));
  var energy=TextOn(UI("Energy",panel),"0 / 500 ENERGY",24,Color.white,FontStyle.Bold);Set(energy.rectTransform,new Vector2(.32f,.30f),new Vector2(.97f,.62f));
  var bar=UI("ProgressBar",panel);Set(bar,new Vector2(.33f,.13f),new Vector2(.96f,.27f));bar.gameObject.AddComponent<Image>().color=new Color(.08f,.12f,.24f,.95f);
  var fill=ImageOn(UI("Fill",bar));Stretch(fill.rectTransform);fill.color=Gold;fill.type=Image.Type.Filled;fill.fillMethod=Image.FillMethod.Horizontal;fill.fillAmount=0;fill.raycastTarget=false;
  var award=UI("EnergyAward",panel);Set(award,new Vector2(.34f,.00f),new Vector2(.96f,.35f));var awardText=TextOn(award,"+10 ENERGY",28,Gold,FontStyle.Bold);var awardGroup=award.gameObject.AddComponent<CanvasGroup>();awardGroup.alpha=0;award.gameObject.SetActive(false);
  var sparks=new List<Image>();for(int i=0;i<6;i++){var s=ImageOn(UI("Spark_"+i,root));s.rectTransform.anchorMin=s.rectTransform.anchorMax=new Vector2(.5f,.5f);s.rectTransform.sizeDelta=new Vector2(12,12);s.raycastTarget=false;s.gameObject.SetActive(false);sparks.Add(s);}
  var complete=UI("PlanetCompletion",safe);Set(complete,new Vector2(.14f,.48f),new Vector2(.86f,.64f));complete.gameObject.AddComponent<Image>().color=new Color(.03f,.04f,.14f,.96f);var completeText=TextOn(UI("Text",complete),"PLANET RESTORED!\nNEXT WORLD AWAITS...",38,Gold,FontStyle.Bold);Stretch(completeText.rectTransform);var completeGroup=complete.gameObject.AddComponent<CanvasGroup>();completeGroup.alpha=0;complete.gameObject.SetActive(false);
  var view=panel.gameObject.AddComponent<PlanetRestorationView>();var sprites=new Sprite[5];for(int i=0;i<5;i++)sprites[i]=AssetDatabase.LoadAssetAtPath<Sprite>(Paths[i]);
  view.Configure(current,next,fill,title,percent,energy,awardText,awardGroup,completeText,completeGroup,root,sparks.ToArray(),sprites);hud.ConfigurePlanet(view);var flow=canvas.GetComponent<GameFlowController>();var roots=new List<GameObject>();foreach(var go in flow.GameRoots)if(go!=null&&go!=journey.gameObject&&go!=hud.JourneyFeedbackRoot)roots.Add(go);roots.Add(panel.gameObject);var flowSo=new SerializedObject(flow);var rootProp=flowSo.FindProperty("gameRoots");rootProp.arraySize=roots.Count;for(int i=0;i<roots.Count;i++)rootProp.GetArrayElementAtIndex(i).objectReferenceValue=roots[i];flowSo.ApplyModifiedPropertiesWithoutUndo();
  var home=safe.Find("HomeRoot/BestJourney");if(home!=null){home.name="PlanetProgress";home.GetComponent<Text>().text="PLANET 01  ·  0% RESTORED";}
  EditorUtility.SetDirty(hud);EditorUtility.SetDirty(view);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Validate();Debug.Log("SPRINT6_SCENE_READY");
 }
 static void ConfigureImports(){foreach(var path in Paths){var ti=AssetImporter.GetAtPath(path) as TextureImporter;if(ti==null)continue;ti.textureType=TextureImporterType.Sprite;ti.spriteImportMode=SpriteImportMode.Single;ti.alphaSource=TextureImporterAlphaSource.FromInput;ti.alphaIsTransparency=true;var settings=new TextureImporterSettings();ti.ReadTextureSettings(settings);settings.spriteMeshType=SpriteMeshType.FullRect;ti.SetTextureSettings(settings);ti.filterMode=FilterMode.Bilinear;ti.textureCompression=TextureImporterCompression.CompressedHQ;ti.mipmapEnabled=false;ti.SaveAndReimport();}}
 [MenuItem("COSMIC BLOCK/Sprint 6/Validate")]
 public static void Validate(){var safe=GameObject.Find("GameCanvas").transform.Find("SafeArea");var panel=safe.Find("PlanetRestoration");Require(panel!=null&&safe.Find("Journey")!=null&&!safe.Find("Journey").gameObject.activeSelf,"Planet replaces Journey");Require(UnityEngine.Object.FindObjectsByType<PlanetRestorationView>(FindObjectsInactive.Include,FindObjectsSortMode.None).Length==1,"one Planet view");Require(panel.Find("PlanetVisual/Current")!=null&&panel.Find("ProgressBar/Fill")!=null,"Planet visuals");}
 static RectTransform UI(string name,Transform parent){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);return r;}
 static Image ImageOn(RectTransform r)=>r.gameObject.AddComponent<Image>();
 static Text TextOn(RectTransform r,string value,int size,Color color,FontStyle style){var t=r.gameObject.AddComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.text=value;t.fontSize=size;t.fontStyle=style;t.color=color;t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;t.resizeTextForBestFit=true;t.resizeTextMinSize=14;t.resizeTextMaxSize=size;return t;}
 static void Set(RectTransform r,Vector2 min,Vector2 max){r.anchorMin=min;r.anchorMax=max;r.offsetMin=r.offsetMax=Vector2.zero;r.localScale=Vector3.one;}static void Stretch(RectTransform r)=>Set(r,Vector2.zero,Vector2.one);
 static void Require(bool v,string m){if(!v)throw new Exception("Sprint 6 validation failed: "+m);}
}