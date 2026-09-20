using System;
using System.Collections.Generic;
using CosmicBlock.Core;
using CosmicBlock.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Sprint5Builder
{
    public const string ScenePath="Assets/Scenes/Game.unity";
    static readonly Color Gold=new Color(.93f,.78f,.43f,1),White=new Color(.91f,.93f,1,1),Blue=new Color(.68f,.78f,1,1),Navy=new Color(.035f,.07f,.16f,.94f);
    [MenuItem("COSMIC BLOCK/Sprint 5/Build Home And Game Flow")]
    public static void Build()
    {
        if(!Application.isBatchMode&&!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())return;
        var scene=EditorSceneManager.OpenScene(ScenePath);var canvas=GameObject.Find("GameCanvas");var safe=canvas.transform.Find("SafeArea") as RectTransform;var session=UnityEngine.Object.FindAnyObjectByType<GameSession>();GameHud hud=null;foreach(var candidate in UnityEngine.Object.FindObjectsByType<GameHud>(FindObjectsInactive.Include,FindObjectsSortMode.None))if(candidate.JourneyFeedbackRoot!=null){hud=candidate;break;}
        if(canvas==null||safe==null||session==null)throw new Exception("Game scene foundation missing.");
        var homeRoot=safe.Find("HomeRoot") as RectTransform;if(homeRoot==null)homeRoot=UI("HomeRoot",safe);Stretch(homeRoot);homeRoot.SetAsLastSibling();
        var reached=new List<Transform>();var gameOvers=new List<Transform>();foreach(Transform child in safe){if(child.name=="ReachedFeedback")reached.Add(child);if(child.name=="GameOverPanel")gameOvers.Add(child);}
        if(hud==null||hud.GameOverRoot==null)throw new Exception("Connected transient UI missing.");
        Transform journeyFeedback=hud.JourneyFeedbackRoot.transform,gameOverPanel=hud.GameOverRoot.transform;
        var staleFlowGroup=gameOverPanel.GetComponent<CanvasGroup>();if(staleFlowGroup!=null)UnityEngine.Object.DestroyImmediate(staleFlowGroup);
        foreach(var duplicate in reached)if(duplicate!=journeyFeedback)UnityEngine.Object.DestroyImmediate(duplicate.gameObject);
        foreach(var duplicate in gameOvers)if(duplicate!=gameOverPanel)UnityEngine.Object.DestroyImmediate(duplicate.gameObject);
        var gameRoots=new List<GameObject>();foreach(Transform child in safe)if(child!=homeRoot&&child!=journeyFeedback&&child!=gameOverPanel)gameRoots.Add(child.gameObject);
        var homeImage=Ensure<Image>(homeRoot.gameObject);homeImage.color=new Color(.01f,.018f,.07f,.18f);homeImage.raycastTarget=true;
        var title=Text("Title",homeRoot,"COSMIC BLOCK",72,Gold,FontStyle.Bold);Set(title.rectTransform,new Vector2(.05f,.72f),new Vector2(.95f,.84f));title.resizeTextForBestFit=true;title.resizeTextMinSize=48;title.resizeTextMaxSize=72;
        var subtitle=Text("Subtitle",homeRoot,"PLAY YOUR NEXT WORLD",26,Blue,FontStyle.Normal);Set(subtitle.rectTransform,new Vector2(.10f,.65f),new Vector2(.90f,.71f));
        var play=MakeButton("PlayButton",homeRoot,"PLAY",42);Set(play.GetComponent<RectTransform>(),new Vector2(.18f,.43f),new Vector2(.82f,.54f));
        var best=Text("BestScore",homeRoot,"BEST  0",40,White,FontStyle.Bold);Set(best.rectTransform,new Vector2(.12f,.29f),new Vector2(.88f,.36f));
        var journey=Text("BestJourney",homeRoot,"BEST JOURNEY\nSTART",32,Blue,FontStyle.Normal);Set(journey.rectTransform,new Vector2(.12f,.18f),new Vector2(.88f,.28f));
        var card=safe.Find("GameOverPanel/Card") as RectTransform;var retry=card.Find("RetryButton").GetComponent<Button>();Set(retry.GetComponent<RectTransform>(),new Vector2(.10f,.19f),new Vector2(.90f,.30f));Ensure<ButtonPressFeedback>(retry.gameObject);
        var ad=card.Find("AdPlaceholderButton");if(ad!=null)ad.gameObject.SetActive(false);
        var home=card.Find("HomeButton")!=null?card.Find("HomeButton").GetComponent<Button>():MakeButton("HomeButton",card,"HOME",34);Set(home.GetComponent<RectTransform>(),new Vector2(.10f,.06f),new Vector2(.90f,.17f));
        var flow=canvas.GetComponent<GameFlowController>();if(flow==null)flow=canvas.AddComponent<GameFlowController>();flow.Configure(homeRoot.gameObject,gameRoots.ToArray(),session,best,journey,play,home);
        foreach(var root in gameRoots){root.SetActive(true);var group=Ensure<CanvasGroup>(root);group.alpha=0;group.interactable=false;group.blocksRaycasts=false;}homeRoot.gameObject.SetActive(true);
        EditorUtility.SetDirty(flow);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Validate();Debug.Log("SPRINT5_SCENE_READY");
    }
    [MenuItem("COSMIC BLOCK/Sprint 5/Validate Home And Game Flow Scene")]
    public static void Validate()
    {
        var canvas=GameObject.Find("GameCanvas");var safe=canvas.transform.Find("SafeArea");var home=safe.Find("HomeRoot");var flow=canvas.GetComponent<GameFlowController>();
        Require(home!=null&&flow!=null&&flow.GameRoots!=null&&flow.GameRoots.Length>0,"Home/game visibility controller");
        Require(home.Find("Title")!=null&&home.Find("Subtitle")!=null&&home.Find("PlayButton")!=null,"Home primary UI");Require(home.Find("BestScore")!=null&&home.Find("BestJourney")!=null,"Home best UI");
        int gameOverCount=0;foreach(Transform child in safe)if(child.name=="GameOverPanel")gameOverCount++;Require(gameOverCount==1&&flow.GameRoots!=null&&Array.IndexOf(flow.GameRoots,safe.Find("GameOverPanel").gameObject)<0,"One independently managed Game Over panel");
        Require(safe.Find("GameOverPanel/Card/HomeButton")!=null,"Game Over HOME");var ad=safe.Find("GameOverPanel/Card/AdPlaceholderButton");Require(ad==null||!ad.gameObject.activeSelf,"No ad placeholder UI");
    }
    static RectTransform UI(string name,Transform parent){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);return r;}
    static void Stretch(RectTransform r)=>Set(r,Vector2.zero,Vector2.one);static void Set(RectTransform r,Vector2 min,Vector2 max){r.anchorMin=min;r.anchorMax=max;r.offsetMin=r.offsetMax=Vector2.zero;r.localScale=Vector3.one;}
    static T Ensure<T>(GameObject go)where T:Component{var value=go.GetComponent<T>();return value==null?go.AddComponent<T>():value;}
    static Text Text(string name,Transform parent,string value,int size,Color color,FontStyle style){var child=parent.Find(name);var r=child as RectTransform;if(r==null)r=UI(name,parent);var t=Ensure<Text>(r.gameObject);t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.text=value;t.fontSize=size;t.fontStyle=style;t.alignment=TextAnchor.MiddleCenter;t.color=color;t.raycastTarget=false;return t;}
    static Button MakeButton(string name,Transform parent,string label,int size){var child=parent.Find(name);var r=child as RectTransform;if(r==null)r=UI(name,parent);var image=Ensure<Image>(r.gameObject);image.color=Navy;image.raycastTarget=true;var outline=Ensure<Outline>(r.gameObject);outline.effectColor=Gold;outline.effectDistance=new Vector2(3,-3);outline.useGraphicAlpha=false;var b=Ensure<Button>(r.gameObject);b.targetGraphic=image;b.transition=Selectable.Transition.ColorTint;Ensure<ButtonPressFeedback>(r.gameObject);var t=Text("Label",r,label,size,Gold,FontStyle.Bold);Stretch(t.rectTransform);return b;}
    static void Require(bool value,string message){if(!value)throw new Exception("Sprint 5 scene validation failed: "+message);}
}
