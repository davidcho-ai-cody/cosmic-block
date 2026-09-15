using System;
using System.IO;
using CosmicBlock.Core;
using CosmicBlock.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public static class Sprint3Builder {
 public const string ScenePath="Assets/Scenes/Game.unity";
 [MenuItem("COSMIC BLOCK/Sprint 3/Build Star Journey")]
 public static void Build(){
  if(!Application.isBatchMode&&!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())return;
  var scene=EditorSceneManager.OpenScene(ScenePath);var canvas=GameObject.Find("GameCanvas");var safe=canvas.transform.Find("SafeArea");
  var session=UnityEngine.Object.FindFirstObjectByType<GameSession>();var hud=canvas.GetComponent<GameHud>();if(safe==null||session==null||hud==null)throw new Exception("Sprint 2 foundation missing.");
  var bg=canvas.transform.Find("Background").GetComponent<Image>();bg.color=new Color(.025f,.025f,.10f);bg.sprite=null;
  var title=safe.Find("Title").GetComponent<Text>();title.text="COSMIC BLOCK";title.color=new Color(.93f,.78f,.43f);title.fontSize=44;Set((RectTransform)title.transform,new Vector2(.08f,.92f),new Vector2(.92f,.975f));
  var score=safe.Find("ScorePlaceholder").GetComponent<Text>();score.color=new Color(.91f,.93f,1);Set((RectTransform)score.transform,new Vector2(.08f,.835f),new Vector2(.92f,.875f));
  var combo=safe.Find("ComboText").GetComponent<Text>();combo.color=new Color(.93f,.72f,.38f);Set((RectTransform)combo.transform,new Vector2(.08f,.785f),new Vector2(.92f,.82f));
  var old=safe.Find("Journey");if(old!=null)UnityEngine.Object.DestroyImmediate(old.gameObject);
  var journey=UI("Journey",safe);Set(journey,new Vector2(.12f,.875f),new Vector2(.88f,.925f));
  var jt=TextOn(UI("JourneyText",journey),"START  >  STAR FIELD\n0 / 1,000",25,new Color(.74f,.81f,1));Set((RectTransform)jt.transform,new Vector2(0,.28f),Vector2.one);
  var track=UI("Track",journey);Set(track,new Vector2(.04f,.08f),new Vector2(.96f,.23f));track.gameObject.AddComponent<Image>().color=new Color(.11f,.14f,.29f,.82f);
  var fill=UI("Fill",track);Set(fill,Vector2.zero,Vector2.one);var fi=fill.gameObject.AddComponent<Image>();fi.color=new Color(.94f,.69f,.30f);fi.type=Image.Type.Filled;fi.fillMethod=Image.FillMethod.Horizontal;fi.fillOrigin=0;fi.fillAmount=0;
  var feedback=UI("ReachedFeedback",safe);Set(feedback,new Vector2(.16f,.54f),new Vector2(.84f,.68f));feedback.gameObject.AddComponent<Image>().color=new Color(.07f,.06f,.20f,.94f);var cg=feedback.gameObject.AddComponent<CanvasGroup>();cg.alpha=0;
  var ft=TextOn(UI("Label",feedback),"DESTINATION REACHED\nMOON",42,new Color(1,.77f,.35f));Set((RectTransform)ft.transform,new Vector2(.04f,.08f),new Vector2(.96f,.92f));feedback.gameObject.SetActive(false);
  foreach(Transform slot in safe.Find("BlockArea")){var image=slot.GetComponent<Image>();if(image!=null)image.color=new Color(.12f,.14f,.29f,.12f);}
  var panel=safe.Find("GameOverPanel");var card=panel.Find("Card");card.GetComponent<Image>().color=new Color(.07f,.065f,.18f,.98f);var final=card.Find("FinalScore").GetComponent<Text>();final.fontSize=38;Set((RectTransform)final.transform,new Vector2(.08f,.34f),new Vector2(.92f,.75f));
  hud.ConfigureJourney(jt,fi,cg,ft);session.ConfigureHud(hud);EditorUtility.SetDirty(hud);EditorUtility.SetDirty(session);
  EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Validate();Debug.Log("SPRINT3_SCENE_READY");
 }
 static RectTransform UI(string n,Transform p){var r=new GameObject(n,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(p,false);return r;}
 static void Set(RectTransform r,Vector2 a,Vector2 b){r.anchorMin=a;r.anchorMax=b;r.offsetMin=r.offsetMax=Vector2.zero;}
 static Text TextOn(RectTransform r,string value,int size,Color color){var t=r.gameObject.AddComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.text=value;t.fontSize=size;t.alignment=TextAnchor.MiddleCenter;t.color=color;t.raycastTarget=false;t.resizeTextForBestFit=true;t.resizeTextMinSize=14;t.resizeTextMaxSize=size;return t;}
 [MenuItem("COSMIC BLOCK/Sprint 3/Validate Journey Rules")]
 public static void Validate(){
  int[] scores={0,1000,3000,6000,10000,4500};string[] names={"START","STAR FIELD","MOON","SATURN","DEEP SPACE","MOON"};
  for(int i=0;i<scores.Length;i++)Require(JourneyProgress.At(scores[i]).Current.Name==names[i],"Destination "+scores[i]);
  var p=JourneyProgress.At(4500);Require(p.Next.HasValue&&p.Next.Value.Name=="SATURN"&&Mathf.Approximately(p.Progress,.5f),"4500 progress");
  p=JourneyProgress.At(10000);Require(!p.Next.HasValue&&Mathf.Approximately(p.Progress,1),"last destination");
  Require(JourneyProgress.Milestones.Count==5,"milestone count");
  Directory.CreateDirectory("Validation");File.WriteAllText("Validation/sprint3.txt","PASS domain tests 1-5: START, STAR FIELD, MOON, SATURN, DEEP SPACE; 4500=MOON to SATURN 50%; last destination continues.\n");
 }
 public static void Require(bool c,string m){if(!c)throw new Exception("Sprint 3 test failed: "+m);}
}
