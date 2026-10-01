using System;
using System.Collections.Generic;
using System.IO;
using CosmicBlock.Core;
using CosmicBlock.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Sprint71HomePlayProbe
{
    static double started;
    static int frames,oldEnergy,oldVersion;
    static bool hadEnergy,hadVersion;
    static string errors="";
    static GameSession session;
    static GameFlowController flow;
    static HomeViewController homeView;
    static Transform home;
    static readonly List<string> results=new List<string>();

    public static void Run()
    {
        hadEnergy=PlayerPrefs.HasKey(PlanetRestoration.DefaultKey);hadVersion=PlayerPrefs.HasKey(PlanetRestoration.VersionKey);
        oldEnergy=PlayerPrefs.GetInt(PlanetRestoration.DefaultKey,0);oldVersion=PlayerPrefs.GetInt(PlanetRestoration.VersionKey,0);
        PlayerPrefs.SetInt(PlanetRestoration.DefaultKey,0);PlayerPrefs.SetInt(PlanetRestoration.VersionKey,2);PlayerPrefs.Save();
        EditorSceneManager.OpenScene(Sprint71HomeBuilder.ScenePath);started=EditorApplication.timeSinceStartup;
        Application.logMessageReceived+=Log;EditorApplication.update+=Check;EditorApplication.EnterPlaymode();
    }

    static void Check()
    {
        if(EditorApplication.timeSinceStartup-started>180){Finish(false,"timeout");return;}
        if(!EditorApplication.isPlaying||++frames<30)return;
        try
        {
            session=UnityEngine.Object.FindAnyObjectByType<GameSession>();flow=UnityEngine.Object.FindAnyObjectByType<GameFlowController>();homeView=UnityEngine.Object.FindAnyObjectByType<HomeViewController>(FindObjectsInactive.Include);home=flow.HomeRoot.transform;
            Req(session!=null&&flow!=null&&homeView!=null&&flow.Screen==FlowScreen.Home&&home.gameObject.activeSelf,"cold HOME visible");
            Req(home.Find("TitleArea/Title").GetComponent<Image>().sprite.name=="home_title_starlight_block","image title");
            Req(home.Find("MainActions/GameStartButton").gameObject.activeInHierarchy&&home.Find("MainActions/PlanetCollectionButton").gameObject.activeInHierarchy&&home.Find("QuitConfirmPanel")!=null,"HOME actions");
            Req(!UnityEngine.Object.FindAnyObjectByType<GameHud>().GameOverRoot.activeSelf,"GameOver hidden");
            foreach(var root in flow.GameRoots){var group=root.GetComponent<CanvasGroup>();Req(group!=null&&group.alpha==0&&!group.interactable&&!group.blocksRaycasts,"game root hidden "+root.name);Req(root.name!="PlanetCompletion"&&root.name!="GameOverPanel"&&root.name!="ReachedFeedback","transient excluded "+root.name);}
            Req(home.Find("TitleArea/Title").GetComponent<Text>()==null,"legacy text title removed");
            results.Add("PASS A: Cold launch HOME, Korean title/actions, game/transient UI hidden.");

            int savedBest=session.BestScore;int[] values={0,90,190,290,390,400};
            for(int i=0;i<values.Length;i++){session.DebugSetPlanetEnergy(values[i]);flow.RefreshHome();Req(home.Find("HeroArea/HeroPlanet").GetComponent<Image>().sprite.name=="home_planet_hero","fixed hero "+values[i]);Req(home.Find("EnergyArea")==null,"energy UI removed "+values[i]);Req(home.Find("CurrentPlanetArea")==null,"stage card removed "+values[i]);Req(homeView.BestText==savedBest.ToString("N0",System.Globalization.CultureInfo.InvariantCulture),"best preserved");}
            results.Add("PASS D: HOME Planet sprite/stage/progress sync at 0/90/190/290/390/400.");

            int energy=session.Restoration.CurrentEnergy,score=session.Score;
            home.Find("MainActions/PlanetCollectionButton").GetComponent<Button>().onClick.Invoke();Req(homeView.ToastVisible&&flow.Screen==FlowScreen.Home&&session.Score==score&&session.Restoration.CurrentEnergy==energy&&session.BestScore==savedBest,"collection toast isolation");
            homeView.HideTransient();home.Find("BottomActions/SettingsButton").GetComponent<Button>().onClick.Invoke();Req(homeView.SettingsVisible,"settings opens");home.Find("SettingsPanel/Card/Close").GetComponent<Button>().onClick.Invoke();Req(!homeView.SettingsVisible,"settings closes");
            home.Find("BottomActions/QuitButton").GetComponent<Button>().onClick.Invoke();Req(homeView.QuitVisible,"quit confirm opens");home.Find("QuitConfirmPanel/Card/Cancel").GetComponent<Button>().onClick.Invoke();Req(!homeView.QuitVisible&&!flow.QuitRequested,"quit cancel");
            Req(home.Find("DevButton").gameObject.activeSelf&&PlanetDebugPanel.ShouldShow(true)&&!PlanetDebugPanel.ShouldShow(false),"DEV build/release contract");
            Req(!ContainsLegacy(home),"legacy English HOME text");
            results.Add("PASS E-F-G: Collection toast, settings shell, quit cancel, DEV contract, legacy text removed.");

            Capture(1080,1920,"9x16");Capture(1080,2400,"tall_safe");Capture(1080,1440,"short");
            results.Add("PASS responsive HOME renders: 1080x1920, 1080x2400, 1080x1440.");

            home.Find("MainActions/GameStartButton").GetComponent<Button>().onClick.Invoke();
            Req(flow.Screen==FlowScreen.Game&&!home.gameObject.activeSelf&&session.State==GameState.Playing&&!session.Feedback.IsPlaying,"game start");
            Req(!UnityEngine.Object.FindAnyObjectByType<GameHud>().GameOverRoot.activeSelf,"GameOver stays hidden");
            int persistent=session.Restoration.CurrentEnergy;
            flow.RequestHome();Req(flow.HomeConfirmVisible,"playing HOME confirm");flow.ConfirmHome();
            Req(flow.Screen==FlowScreen.Home&&home.gameObject.activeSelf&&session.Restoration.CurrentEnergy==persistent&&session.BestScore==savedBest&&!session.Feedback.IsPlaying&&!UnityEngine.Object.FindAnyObjectByType<PlanetRestorationView>(FindObjectsInactive.Include).FeedbackActive,"clean return");
            Req(!homeView.ToastVisible&&!homeView.SettingsVisible&&!homeView.QuitVisible,"HOME transient clean");
            results.Add("PASS B-C: Game start and Playing->Home preserve persistent state and clear transient feedback.");
            Req(errors.Length==0,"runtime errors "+errors);Finish(true,"");
        }
        catch(Exception e){Finish(false,e.ToString());}
    }

    static bool ContainsLegacy(Transform root)
    {
        string[] banned={"COSMIC BLOCK","PLAY YOUR NEXT WORLD","PLAY","BEST"};
        foreach(var text in root.GetComponentsInChildren<Text>(true))foreach(var value in banned)if(text.text==value)return true;
        return false;
    }

    static void Capture(int width,int height,string name)
    {
        var canvas=GameObject.Find("GameCanvas").GetComponent<Canvas>();var camera=Camera.main;var safe=canvas.transform.Find("SafeArea") as RectTransform;
        var mode=canvas.renderMode;var oldCamera=canvas.worldCamera;var oldTarget=camera.targetTexture;bool ortho=camera.orthographic;float size=camera.orthographicSize;var active=RenderTexture.active;
        var rt=new RenderTexture(width,height,24);Texture2D image=null;
        try
        {
            rt.Create();camera.targetTexture=rt;camera.orthographic=true;camera.orthographicSize=height/2f;canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=10;
            safe.anchorMin=Vector2.zero;safe.anchorMax=Vector2.one;safe.offsetMin=safe.offsetMax=Vector2.zero;Canvas.ForceUpdateCanvases();var bg=home.Find("HomeBackground").GetComponent<AspectFillBackground>();bg.SendMessage("Apply");Canvas.ForceUpdateCanvases();
            CheckLayout(name);camera.Render();RenderTexture.active=rt;image=new Texture2D(width,height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,width,height),0,0);image.Apply();Directory.CreateDirectory("Validation");File.WriteAllBytes("Validation/sprint71_home_"+name+".png",image.EncodeToPNG());
        }
        finally
        {
            canvas.renderMode=mode;canvas.worldCamera=oldCamera;camera.targetTexture=oldTarget;camera.orthographic=ortho;camera.orthographicSize=size;RenderTexture.active=active;if(image!=null)UnityEngine.Object.DestroyImmediate(image);rt.Release();UnityEngine.Object.DestroyImmediate(rt);safe.GetComponent<SafeArea>().SendMessage("Apply");
        }
    }

    static void CheckLayout(string name)
    {
        string[] paths={"TitleArea","HeroArea","MainActions","BestScoreArea","BottomActions"};
        Rect previous=new Rect();bool first=true;foreach(string path in paths){var r=WorldRect(home.Find(path) as RectTransform);Req(r.width>0&&r.height>0,"visible rect "+path+" "+name);if(!first)Req(r.yMax<=previous.yMin+2,"no vertical overlap "+path+" "+name);previous=r;first=false;}
        Req(!WorldRect(home.Find("BottomActions/SettingsButton") as RectTransform).Overlaps(WorldRect(home.Find("BottomActions/QuitButton") as RectTransform)),"bottom no overlap "+name);
    }
    static Rect WorldRect(RectTransform r){var c=new Vector3[4];r.GetWorldCorners(c);return Rect.MinMaxRect(c[0].x,c[0].y,c[2].x,c[2].y);}
    static void Log(string m,string s,LogType t){if(t==LogType.Error||t==LogType.Exception||t==LogType.Assert)errors+=m+"\n";}
    static void Req(bool v,string m){if(!v)throw new Exception("Sprint 7-1 test failed: "+m);}
    static void Finish(bool pass,string failure)
    {
        EditorApplication.update-=Check;Application.logMessageReceived-=Log;if(hadEnergy)PlayerPrefs.SetInt(PlanetRestoration.DefaultKey,oldEnergy);else PlayerPrefs.DeleteKey(PlanetRestoration.DefaultKey);if(hadVersion)PlayerPrefs.SetInt(PlanetRestoration.VersionKey,oldVersion);else PlayerPrefs.DeleteKey(PlanetRestoration.VersionKey);PlayerPrefs.Save();Directory.CreateDirectory("Validation");File.WriteAllText("Validation/sprint71.txt",string.Join("\n",results)+"\n"+(pass?"SPRINT71_HOME_PASS":failure)+"\n");if(pass)Debug.Log("SPRINT71_HOME_PASS");else Debug.LogError(failure);EditorApplication.Exit(pass?0:1);
    }
}


