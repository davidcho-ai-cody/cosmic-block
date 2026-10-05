using System;
using System.Collections.Generic;
using System.IO;
using CosmicBlock.Core;
using CosmicBlock.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public static class Sprint8CollectionPlayProbe
{
    static double started;static int frames;static string errors="";
    static readonly List<string> results=new List<string>();
    static readonly string[] keys={PlanetRestoration.DefaultKey,PlanetRestoration.VersionKey,GameSession.DefaultBestScoreKey};
    static readonly int[] saved=new int[3];static readonly bool[] had=new bool[3];
    static GameSession session;static GameFlowController flow;static PlanetCollectionView view;static Transform safe;
    public static void Run()
    {
        for(int i=0;i<3;i++){had[i]=PlayerPrefs.HasKey(keys[i]);saved[i]=PlayerPrefs.GetInt(keys[i]);}
        PlayerPrefs.SetInt(keys[0],0);PlayerPrefs.SetInt(keys[1],PlanetRestoration.CurrentVersion);PlayerPrefs.Save();
        EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");started=EditorApplication.timeSinceStartup;
        Application.logMessageReceived+=Log;EditorApplication.update+=Check;EditorApplication.EnterPlaymode();
    }
    static void Check()
    {
        if(EditorApplication.timeSinceStartup-started>180){Finish(false,"timeout");return;}
        if(!EditorApplication.isPlaying||++frames<35)return;
        try
        {
            session=UnityEngine.Object.FindAnyObjectByType<GameSession>();flow=UnityEngine.Object.FindAnyObjectByType<GameFlowController>();view=flow.CollectionView;
            safe=GameObject.Find("GameCanvas").transform.Find("SafeArea");Req(view!=null&&flow.Screen==FlowScreen.Home,"cold HOME");Req(safe.GetComponentsInChildren<PlanetCollectionView>(true).Length==1,"single collection view");
            int score=session.Score,best=session.BestScore;var board=session.Model;
            for(int stage=1;stage<=5;stage++)
            {
                int energy=(stage-1)*100;session.DebugSetPlanetEnergy(energy);
                flow.HomeRoot.transform.Find("MainActions/PlanetCollectionButton").GetComponent<Button>().onClick.Invoke();
                Req(flow.Screen==FlowScreen.Collection&&!flow.HomeRoot.activeSelf&&view.gameObject.activeSelf,"entry");
                Req(view.CurrentStage==stage&&view.PreviewStage==stage&&view.HeroSprite.name.Contains("Stage0"+stage),"current stage "+stage);
                Req(view.Percentage==session.Restoration.Percent+"%"&&Mathf.Abs(view.Progress-session.Restoration.Percent/100f)<.001f,"percentage and actual fill");
                Req(view.transform.Find("ProgressFrameArea/ProgressFrame").gameObject.activeSelf,"progress frame");
                Req(!view.transform.Find("Status").gameObject.activeSelf,"plain progress status removed");
                Req(view.Slots.Length==5,"five slots");view.SendMessage("Update");Req(view.Slots[stage-1].transform.localScale.x>=1&&view.Slots[stage-1].transform.localScale.x<=1.041f,"current pulse");
                for(int j=1;j<=5;j++)
                {
                    Req(view.IsStageLocked(j)==(j>stage),"lock "+j);Req(view.Slots[j-1].interactable==(j<=stage),"slot input "+j);
                    Req(view.Slots[j-1].transform.Find("Lock").gameObject.activeSelf==(j>stage),"lock visual "+j);
                    Req(view.Slots[j-1].transform.Find("Thumbnail").GetComponent<Image>().color.a==(j<=stage?1:.3f),"thumbnail alpha");
                    Req(view.Slots[j-1].transform.Find("CurrentRing").gameObject.activeSelf==(j==stage),"current stage ring");
                    var label=view.Slots[j-1].transform.parent.Find("StageLabel"+j).GetComponent<Text>();
                    var number=view.Slots[j-1].transform.parent.Find("StageNumber"+j).GetComponent<Text>();
                    Req(number.fontSize<label.fontSize,"number/name hierarchy");
                    Req(label.color.a==(j<=stage?1:.65f),"future labels dim");
                }
                var slotShapes=new CosmicBlock.Blocks.BlockShape[3];for(int q=0;q<3;q++)slotShapes[q]=session.Slots[q].Shape;string currentLore=view.Message;view.Slots[0].onClick.Invoke();Req(view.PreviewStage==1&&view.HeroSprite.name.Contains("Stage01"),"past preview");
                Req(stage==1||view.Message!=currentLore,"lore changes");view.Preview(5);Req(view.PreviewStage==(stage==5?5:1),"future preview blocked");
                Req(session.Restoration.CurrentEnergy==energy&&PlayerPrefs.GetInt(keys[0])==energy&&PlayerPrefs.GetInt(keys[1])==PlanetRestoration.CurrentVersion&&view.CurrentStage==stage&&PlayerPrefs.GetInt(keys[2])==best&&session.Score==score&&session.BestScore==best&&session.Model==board,"save and run isolation");
                view.GoBack();Req(flow.Screen==FlowScreen.Home&&session.Score==score,"back HOME");flow.ShowCollection();Req(view.PreviewStage==stage,"reentry current");
                if(stage==5)Req(view.Progress==1&&view.Percentage=="100%","completed");
                for(int p=1;p<5;p++){view.transform.Find("Next").GetComponent<Button>().onClick.Invoke();Req(view.PlanetIndex==p&&view.Message==PlanetCollectionData.LockedMessage,"locked planet "+p);for(int j=1;j<=5;j++)Req(view.IsStageLocked(j)&&!view.Slots[j-1].interactable,"all locked");Req(!view.transform.Find("Hero").gameObject.activeSelf&&view.transform.Find("HeroLock").gameObject.activeSelf,"lock hero");Req(!view.transform.Find("ProgressFrameArea/ProgressFrame/Progress").gameObject.activeSelf,"locked progress hidden");}
                view.transform.Find("Next").GetComponent<Button>().onClick.Invoke();Req(view.PlanetIndex==4&&!view.transform.Find("Next").GetComponent<Button>().interactable,"next boundary");for(int p=3;p>=0;p--){view.transform.Find("Previous").GetComponent<Button>().onClick.Invoke();Req(view.PlanetIndex==p,"previous");}view.transform.Find("Previous").GetComponent<Button>().onClick.Invoke();Req(view.PlanetIndex==0&&!view.transform.Find("Previous").GetComponent<Button>().interactable,"prev boundary");
                flow.HandleBack();Req(flow.Screen==FlowScreen.Home,"Android back");for(int q=0;q<3;q++)Req(session.Slots[q].Shape==slotShapes[q],"no piece resupply on collection navigation");results.Add("PASS stage "+stage+": sprites, locks, lore, preview isolation, reentry, navigation.");
            }
            session.DebugSetPlanetEnergy(0);flow.ShowCollection();Capture(1080,1920,"stage1",false);
            session.DebugSetPlanetEnergy(400);view.Open(session.Restoration);Capture(1080,1920,"complete",false);
            session.DebugSetPlanetEnergy(205);view.Open(session.Restoration);Capture(1080,1920,"stage3",false);
            Capture(1080,1920,"9x16",false);Capture(1080,2400,"tall_safe",true);Capture(1080,1440,"short",false);
            view.transform.Find("Next").GetComponent<Button>().onClick.Invoke();Capture(1080,1920,"locked_9x16",false);Capture(1080,2400,"locked_tall_safe",true);Capture(1080,1440,"locked_short",false);
            flow.ReturnFromCollection();flow.Play();Req(flow.Screen==FlowScreen.Game&&session.State==GameState.Playing,"HOME GAME");flow.RequestHome();flow.ConfirmHome();Req(flow.Screen==FlowScreen.Home,"GAME HOME");
            Req(!PlanetDebugPanel.ShouldShow(false),"release DEV hidden");Req(view.GetComponentsInChildren<PlanetDebugPanel>(true).Length==0,"no collection DEV UI");Req(errors.Length==0,"runtime errors "+errors);
            results.Add("PASS responsive bounds/text, HOME GAME flow, release DEV contract; runtime errors 0.");Finish(true,"");
        }
        catch(Exception e){Finish(false,e.ToString());}
    }
    static void Capture(int w,int h,string name,bool inset)
    {
        var canvas=GameObject.Find("GameCanvas").GetComponent<Canvas>();var cam=Camera.main;var sr=(RectTransform)safe;var sa=sr.GetComponent<SafeArea>();bool enabled=sa.enabled;
        var mode=canvas.renderMode;var wc=canvas.worldCamera;var target=cam.targetTexture;bool ortho=cam.orthographic;float size=cam.orthographicSize;var active=RenderTexture.active;
        var min=sr.anchorMin;var max=sr.anchorMax;var omin=sr.offsetMin;var omax=sr.offsetMax;var rt=new RenderTexture(w,h,24);Texture2D tex=null;
        try
        {
            sa.enabled=false;rt.Create();cam.targetTexture=rt;cam.orthographic=true;cam.orthographicSize=h/2f;canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=cam;canvas.planeDistance=10;
            sr.anchorMin=inset?new Vector2(.02f,.04f):Vector2.zero;sr.anchorMax=inset?new Vector2(.98f,.94f):Vector2.one;sr.offsetMin=sr.offsetMax=Vector2.zero;Canvas.ForceUpdateCanvases();
            var bg=view.transform.Find("Background");bg.GetComponent<AspectFillBackground>().enabled=false;
            var br=(RectTransform)bg;var img=bg.GetComponent<Image>();var full=(RectTransform)canvas.transform;float scale=Mathf.Max(full.rect.width/img.sprite.rect.width,full.rect.height/img.sprite.rect.height);br.anchorMin=br.anchorMax=new Vector2(.5f,.5f);br.sizeDelta=img.sprite.rect.size*scale;br.anchoredPosition=sr.InverseTransformPoint(full.TransformPoint(full.rect.center));
            Canvas.ForceUpdateCanvases();Bounds(name);cam.Render();RenderTexture.active=rt;tex=new Texture2D(w,h,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,w,h),0,0);tex.Apply();Directory.CreateDirectory("Validation");File.WriteAllBytes("Validation/sprint8_2_1_collection_"+name+".png",tex.EncodeToPNG());bg.GetComponent<AspectFillBackground>().enabled=true;
        }
        finally{canvas.renderMode=mode;canvas.worldCamera=wc;cam.targetTexture=target;cam.orthographic=ortho;cam.orthographicSize=size;RenderTexture.active=active;sr.anchorMin=min;sr.anchorMax=max;sr.offsetMin=omin;sr.offsetMax=omax;sa.enabled=enabled;if(tex!=null)UnityEngine.Object.DestroyImmediate(tex);rt.Release();UnityEngine.Object.DestroyImmediate(rt);}
    }
    static void Bounds(string name)
    {
        Rect area=World((RectTransform)safe);
        Rect prev=World((RectTransform)view.transform.Find("Previous")),next=World((RectTransform)view.transform.Find("Next"));
        Req(Vector2.Distance(prev.size,next.size)<.1f,"matching navigation hit rects");
        Rect previousIcon=World((RectTransform)view.transform.Find("Previous/Icon")),nextIcon=World((RectTransform)view.transform.Find("Next/Icon"));
        Req(Vector2.Distance(previousIcon.size,nextIcon.size)<.1f,"matching navigation visual rects");
        Req(Mathf.Abs(previousIcon.width/(area.width*.15f*.9f)-1.18f)<.0001f&&Mathf.Abs(previousIcon.height/(area.height*.105f*.9f)-1.18f)<.0001f,"navigation visual enlargement 18 percent");
        Rect heroRect=World((RectTransform)view.transform.Find("Hero"));var heroSprite=view.transform.Find("Hero").GetComponent<Image>().sprite;
        float heroScale=Mathf.Min(heroRect.width/heroSprite.rect.width,heroRect.height/heroSprite.rect.height);Vector2 heroRadii=heroSprite.rect.size*heroScale*.5f;
        foreach(string nav in new[]{"Previous","Next"}){
            Rect hit=World((RectTransform)view.transform.Find(nav)),icon=World((RectTransform)view.transform.Find(nav+"/Icon"));
            Req(hit.xMin>=area.xMin&&hit.xMax<=area.xMax&&hit.yMin>=area.yMin&&hit.yMax<=area.yMax,"navigation hit within safe area");
            Req(icon.xMin>=hit.xMin&&icon.xMax<=hit.xMax&&icon.yMin>=hit.yMin&&icon.yMax<=hit.yMax,"navigation visual fully touchable");
            Req(Mathf.Abs(hit.center.x-(area.xMin+area.width*(nav=="Previous"?.085f:.915f)))<.1f&&Mathf.Abs(hit.center.y-(area.yMin+area.height*.6125f))<.1f,"navigation center unchanged");
            Vector2 nearest=new Vector2(Mathf.Clamp(heroRect.center.x,hit.xMin,hit.xMax),Mathf.Clamp(heroRect.center.y,hit.yMin,hit.yMax));Vector2 delta=nearest-heroRect.center;
            Req(delta.x*delta.x/(heroRadii.x*heroRadii.x)+delta.y*delta.y/(heroRadii.y*heroRadii.y)>1,"navigation hit clear of rendered planet disk");
            Req(view.transform.Find(nav+"/Icon").GetComponent<Image>().preserveAspect,"navigation aspect preserved");
        }
        results.Add("NAV "+name+": visual="+previousIcon.size+" hit="+prev.size+" scale=1.18 centers unchanged");
        var pf=view.transform.Find("ProgressFrameArea/ProgressFrame") as RectTransform;
        if(pf.gameObject.activeSelf){
            Rect pr=World(pf);
            Rect actualTrack=World((RectTransform)pf.Find("Progress")),actualFill=World((RectTransform)pf.Find("Progress/Fill"));
            float visibleFill=actualFill.width/actualTrack.width;
            Req(Mathf.Abs(visibleFill-view.Progress)<.0001f&&Mathf.Abs(visibleFill-session.Restoration.Percent/100f)<.0001f,"actual rendered fill and percentage agree");
            foreach(string item in new[]{"RestorationLabel","Percentage","Progress"}){
                Rect child=World((RectTransform)pf.Find(item));Req(child.xMin>=pr.xMin&&child.xMax<=pr.xMax&&child.yMin>=pr.yMin&&child.yMax<=pr.yMax,"progress content inside frame");
            }
            Req(!World((RectTransform)pf.Find("Percentage")).Overlaps(World((RectTransform)pf.Find("Progress"))),"percentage and bar separate");
        }
        float titleX=World((RectTransform)view.transform.Find("Title")).center.x;
        Req(Mathf.Abs(titleX-area.center.x)<.1f,"title centered");
        results.Add("Title "+name+": baseline offset=0"+" current offset="+(titleX-area.center.x));
        foreach(var image in view.GetComponentsInChildren<Image>(true))Req(image.sprite==null||image.sprite.name!="collection_stage_slot","old slot sprite absent");
        Rect frame=World((RectTransform)view.Slots[0].transform.parent);
        for(int i=0;i<5;i++){
            Rect thumb=World((RectTransform)view.Slots[i].transform.Find("Thumbnail"));
            Req(thumb.xMin>=frame.xMin&&thumb.xMax<=frame.xMax&&thumb.yMin>=frame.yMin&&thumb.yMax<=frame.yMax,"thumbnail inside frame");
            Rect label=World((RectTransform)view.Slots[i].transform.parent.Find("StageLabel"+(i+1)));
            Req(label.yMin>=frame.yMin+frame.height*.22f&&label.yMax<=frame.yMax,"label inside visible frame padding");
            Rect number=World((RectTransform)view.Slots[i].transform.parent.Find("StageNumber"+(i+1)));
            Req(label.yMax<=number.yMin+.1f&&number.yMax<=thumb.yMin+2,"labels separated from thumbnail");
            Req(view.Slots[i].transform.Find("CurrentRing").gameObject.activeSelf==(view.PlanetIndex==0&&i+1==view.CurrentStage),"current ring");
            if(i>0){
                Rect prior=World((RectTransform)view.Slots[i-1].transform);
                Rect current=World((RectTransform)view.Slots[i].transform);
                Req(Mathf.Abs(prior.width/prior.height-current.width/current.height)<.02f,"equal thumbnail proportions");
                Req(Mathf.Abs((current.center.x-prior.center.x)/frame.width-.18f)<.005f,"equal spacing");
            }
        }
        foreach(string path in new[]{"Title","Back","Previous","Next","Hero","HeroLock","NameFrameArea","ProgressFrameArea","StageArea","MessageFrameArea"})
        {
            var t=view.transform.Find(path);if(!t.gameObject.activeSelf)continue;Rect r=World((RectTransform)t);Req(r.xMin>=area.xMin-.1f&&r.xMax<=area.xMax+.1f&&r.yMin>=area.yMin-.1f&&r.yMax<=area.yMax+.1f,"safe bounds "+name+" "+path);
        }
        foreach(var t in view.GetComponentsInChildren<Text>())Req(t.cachedTextGenerator.characterCountVisible>=t.text.Replace("\n","").Length,"text clipping "+name+" "+t.name);
        for(int i=1;i<5;i++)Req(!World((RectTransform)view.Slots[i-1].transform).Overlaps(World((RectTransform)view.Slots[i].transform)),"slots overlap");
        Rect stage=World((RectTransform)view.Slots[0].transform.parent);
        Rect lore=World((RectTransform)view.transform.Find("MessageFrameArea/MessageFrame"));
        Req(stage.yMin+stage.height*.14f-(lore.yMax-lore.height*.12f)>=8,"stage/message visible frame gap");
        float previousStageHeight=Mathf.Min(area.width*.93f/(2048f/683f),area.height*.21f);
        float previousMessageY=area.yMin+area.height*.0675f;
        results.Add("LAYOUT "+name+": hit="+prev.size+" stageHeight="+stage.height+" previousHeight="+previousStageHeight+" thumbnail="+World((RectTransform)view.Slots[0].transform.Find("Thumbnail")).size+" messageMoveY="+(lore.center.y-previousMessageY)+" percentage="+view.Percentage+" fill="+view.Progress);
        results.Add("PASS "+name+": Safe Area, title, progress frame/text/fill, navigation hits, five planets, stage padding and message.");
    }
    static Rect World(RectTransform r){var c=new Vector3[4];r.GetWorldCorners(c);return Rect.MinMaxRect(c[0].x,c[0].y,c[2].x,c[2].y);}
    static void Req(bool v,string m){if(!v)throw new Exception("Sprint8: "+m);}
    static void Log(string m,string trace,LogType t){if(t==LogType.Error||t==LogType.Exception||t==LogType.Assert)errors+=m+"\n";}
    static void Finish(bool pass,string fail)
    {
        EditorApplication.update-=Check;Application.logMessageReceived-=Log;
        for(int i=0;i<3;i++){if(had[i])PlayerPrefs.SetInt(keys[i],saved[i]);else PlayerPrefs.DeleteKey(keys[i]);}PlayerPrefs.Save();
        Directory.CreateDirectory("Validation");File.WriteAllText("Validation/sprint8_2_1_collection.txt",string.Join("\n",results)+"\n"+(pass?"SPRINT8_COLLECTION_PASS":fail));if(pass)Debug.Log("SPRINT8_COLLECTION_PASS");else Debug.LogError(fail);EditorApplication.Exit(pass?0:1);
    }
}
