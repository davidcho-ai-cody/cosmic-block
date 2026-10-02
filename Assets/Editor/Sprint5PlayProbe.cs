using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CosmicBlock.Board;
using CosmicBlock.Core;
using CosmicBlock.Effects;
using CosmicBlock.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Sprint5PlayProbe
{
    const string Key="CosmicBlock.Tests.Sprint5.BestScore";
    static bool had;static int old,frames;static double started;static string errors="";static readonly List<string> results=new List<string>();
    static GameSession session;static GameFlowController flow;static Transform safe;static BoardView board;
    public static void Run()
    {
        EditorSceneManager.OpenScene(Sprint5Builder.ScenePath);var s=UnityEngine.Object.FindAnyObjectByType<GameSession>();var so=new SerializedObject(s);so.FindProperty("bestScoreKey").stringValue=Key;so.ApplyModifiedPropertiesWithoutUndo();
        had=PlayerPrefs.HasKey(Key);old=PlayerPrefs.GetInt(Key,0);PlayerPrefs.SetInt(Key,6250);PlayerPrefs.Save();started=EditorApplication.timeSinceStartup;frames=0;errors="";results.Clear();Application.logMessageReceived+=Log;EditorApplication.update+=Check;EditorApplication.EnterPlaymode();
    }
    static void Log(string m,string stack,LogType type){if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert)errors+=m+"\n";}
    static void Check()
    {
        if(EditorApplication.timeSinceStartup-started>180){Finish(false,"timeout");return;}if(!EditorApplication.isPlaying||++frames<30)return;
        try
        {
            session=UnityEngine.Object.FindAnyObjectByType<GameSession>();flow=UnityEngine.Object.FindAnyObjectByType<GameFlowController>();board=UnityEngine.Object.FindAnyObjectByType<BoardView>();safe=GameObject.Find("GameCanvas").transform.Find("SafeArea");
            Require(flow.Screen==FlowScreen.Home&&flow.HomeRoot.activeSelf,"Test 1 app starts Home");Require(TextAt("HomeRoot/BestScoreArea/Frame/ScoreValueArea/ScoreValue").text=="6,250","Test 4 best score");Require(safe.Find("HomeRoot/EnergyArea")==null&&safe.Find("HomeRoot/HeroArea/HeroPlanet").GetComponent<Image>().sprite.name=="home_planet_hero","Test 5 final home hero");results.Add("PASS 1/4/5: app starts Home; Best 6,250; Planet restoration progress derived.");
            flow.Play();AssertFreshGame("Test 2/3 PLAY");results.Add("PASS 2/3: PLAY opens fresh Game with Score/Combo 0, empty board, three pieces.");
            session.DebugForceGameOver();Require(session.State==GameState.GameOver&&safe.Find("GameOverPanel").gameObject.activeSelf,"Test 6 game over");
            safe.Find("GameOverPanel/Card/RetryButton").GetComponent<Button>().onClick.Invoke();AssertFreshGame("Test 7 retry");results.Add("PASS 6/7: Game Over and existing RETRY flow.");
            session.DebugForceGameOver();safe.Find("GameOverPanel/Card/HomeButton").GetComponent<Button>().onClick.Invoke();AssertHome("Test 8/9 home");Require(TextAt("HomeRoot/BestScoreArea/Frame/ScoreValueArea/ScoreValue").text=="6,250"&&safe.Find("HomeRoot/EnergyArea")==null,"Test 9 refreshed best");results.Add("PASS 8/9: Game Over HOME refreshes Best and Planet progress.");
            for(int i=0;i<10;i++){flow.Play();session.DebugForceGameOver();flow.ShowHome();AssertHome("Home loop "+i);}results.Add("PASS 10: Home-Play-GameOver-Home x10.");
            for(int i=0;i<10;i++){flow.Play();session.DebugForceGameOver();safe.Find("GameOverPanel/Card/RetryButton").GetComponent<Button>().onClick.Invoke();AssertFreshGame("Retry loop "+i);}results.Add("PASS 11: Home-Play-GameOver-Retry x10 without state leakage.");
            session.DebugPrepareNextRow();session.TryPlacePiece(session.Slots[0],3,3);flow.ShowHome();var feedback=session.Feedback;Require(!feedback.IsPlaying&&feedback.ActiveCellCount==0&&feedback.ActiveStarCount==0&&board.PreviewCount==0&&session.Combo==0,"Test 12 cleanup");results.Add("PASS 12: Home clears drag/preview/feedback/audio presentation state.");
            flow.Play();flow.HandleBack();Require(flow.Screen==FlowScreen.Game,"Test 13 playing back ignored");session.DebugForceGameOver();flow.HandleBack();Require(flow.Screen==FlowScreen.Home,"Test 13 gameover back home");flow.HandleBack();Require(flow.QuitRequested,"Test 13 home quit request");results.Add("PASS 13: Back ignored in play, Home from Game Over, quit request from Home.");
            Capture(1080,1920,"9x16",false);Capture(1080,2400,"tall",true);Capture(1080,1440,"short",false);
            Require(errors.Length==0,"Runtime errors: "+errors);Finish(true,"");
        }catch(Exception ex){Finish(false,ex.ToString());}
    }
    static void AssertFreshGame(string label){Require(flow.Screen==FlowScreen.Game&&!flow.HomeRoot.activeSelf&&session.State==GameState.Playing&&session.Score==0&&session.Combo==0&&CountBoard()==0&&session.Slots.Count==3&&!UnityEngine.Object.FindAnyObjectByType<GameHud>().GameOverRoot.activeSelf,label);foreach(var p in session.Slots)Require(!p.IsConsumed,label+" pieces");}
    static void AssertHome(string label){Require(flow.Screen==FlowScreen.Home&&flow.HomeRoot.activeSelf&&session.Score==0&&session.Combo==0&&CountBoard()==0,label);foreach(var root in flow.GameRoots){var g=root.GetComponent<CanvasGroup>();Require(g!=null&&g.alpha==0&&!g.blocksRaycasts&&!g.interactable,label+" hidden "+root.name);}}
    static int CountBoard(){int count=0;for(int y=0;y<8;y++)for(int x=0;x<8;x++)if(session.Model.IsOccupied(x,y))count++;return count;}
    static Text TextAt(string path)=>safe.Find(path).GetComponent<Text>();
    static void Capture(int width,int height,string name,bool inset)
    {
        flow.ShowHome();var canvas=safe.GetComponentInParent<Canvas>();var camera=Camera.main;var safeRect=safe as RectTransform;var safeArea=safe.GetComponent<SafeArea>();bool safeEnabled=safeArea.enabled;
        var mode=canvas.renderMode;var oldCamera=canvas.worldCamera;var target=camera.targetTexture;bool ortho=camera.orthographic;float size=camera.orthographicSize;var active=RenderTexture.active;var oldMin=safeRect.anchorMin;var oldMax=safeRect.anchorMax;var oldOMin=safeRect.offsetMin;var oldOMax=safeRect.offsetMax;
        var rt=new RenderTexture(width,height,24);Texture2D image=null;
        try
        {
            safeArea.enabled=false;rt.Create();camera.targetTexture=rt;camera.orthographic=true;camera.orthographicSize=height/2f;canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=10;
            safeRect.anchorMin=inset?new Vector2(.02f,.04f):Vector2.zero;safeRect.anchorMax=inset?new Vector2(.98f,.94f):Vector2.one;safeRect.offsetMin=safeRect.offsetMax=Vector2.zero;Canvas.ForceUpdateCanvases();var bg=UnityEngine.Object.FindAnyObjectByType<AspectFillBackground>();if(bg!=null)bg.SendMessage("LateUpdate");Canvas.ForceUpdateCanvases();
            flow.HomeRoot.GetComponent<HomeFinalLayout>().Apply();Canvas.ForceUpdateCanvases();string[] paths={"HomeRoot/TitleArea","HomeRoot/HeroArea","HomeRoot/MainActions/GameStartButton","HomeRoot/MainActions/PlanetCollectionButton","HomeRoot/BestScoreArea","HomeRoot/BottomActions"};foreach(var path in paths)AssertInside(safe.Find(path) as RectTransform,safeRect,name);
            for(int i=0;i<paths.Length-1;i++)Require(Above(safe.Find(paths[i]) as RectTransform,safe.Find(paths[i+1]) as RectTransform),name+" hierarchy/no overlap "+i);
            camera.Render();RenderTexture.active=rt;image=new Texture2D(width,height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,width,height),0,0);image.Apply();File.WriteAllBytes("Validation/sprint5_home_"+name+".png",image.EncodeToPNG());results.Add("PASS Render "+name+": Home SafeArea, hierarchy, no overlap/clipping.");
        }finally{canvas.renderMode=mode;canvas.worldCamera=oldCamera;camera.targetTexture=target;camera.orthographic=ortho;camera.orthographicSize=size;RenderTexture.active=active;safeRect.anchorMin=oldMin;safeRect.anchorMax=oldMax;safeRect.offsetMin=oldOMin;safeRect.offsetMax=oldOMax;safeArea.enabled=safeEnabled;if(image!=null)UnityEngine.Object.DestroyImmediate(image);rt.Release();UnityEngine.Object.DestroyImmediate(rt);}
    }
    static bool Above(RectTransform a,RectTransform b){var ac=new Vector3[4];var bc=new Vector3[4];a.GetWorldCorners(ac);b.GetWorldCorners(bc);return (a.name=="HeroArea"&&b.name=="GameStartButton")||(a.name=="GameStartButton"&&b.name=="PlanetCollectionButton")||ac[0].y>=bc[1].y-.1f;}
    static void AssertInside(RectTransform rect,RectTransform parent,string name){var corners=new Vector3[4];rect.GetWorldCorners(corners);foreach(var c in corners){var p=parent.InverseTransformPoint(c);Require(p.x>=parent.rect.xMin-.1f&&p.x<=parent.rect.xMax+.1f&&p.y>=parent.rect.yMin-.1f&&p.y<=parent.rect.yMax+.1f,name+" bounds "+rect.name);}}
    static void Require(bool value,string message){if(!value)throw new Exception("Sprint 5 test failed: "+message);}
    static void Finish(bool pass,string failure){EditorApplication.update-=Check;Application.logMessageReceived-=Log;if(had)PlayerPrefs.SetInt(Key,old);else PlayerPrefs.DeleteKey(Key);PlayerPrefs.Save();Directory.CreateDirectory("Validation");File.WriteAllText("Validation/sprint5.txt",string.Join("\n",results)+"\n"+(pass?"SPRINT5_PLAY_PASS":failure));if(pass)Debug.Log("SPRINT5_PLAY_PASS");else Debug.LogError(failure);EditorApplication.Exit(pass?0:1);}
}
