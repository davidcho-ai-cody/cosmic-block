using System;
using System.Collections.Generic;
using System.IO;
using CosmicBlock.Core;
using CosmicBlock.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Sprint5JourneyFeedbackPlayProbe
{
    static GameSession session; static GameFlowController flow; static GameHud hud; static Transform safe;
    static int stage,frames; static double started,waitStarted; static string errors="";
    static readonly List<string> results=new List<string>();

    public static void Run()
    {
        EditorSceneManager.OpenScene(Sprint5Builder.ScenePath);
        started=EditorApplication.timeSinceStartup;stage=frames=0;errors="";results.Clear();
        Application.logMessageReceived+=Log;EditorApplication.update+=Check;EditorApplication.EnterPlaymode();
    }

    static void Log(string message,string stack,LogType type)
    {
        if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert)errors+=message+"\n";
    }

    static void Check()
    {
        if(EditorApplication.timeSinceStartup-started>180){Finish(false,"timeout");return;}
        if(!EditorApplication.isPlaying)return;
        try
        {
            if(stage==0)
            {
                if(++frames<30)return;
                session=UnityEngine.Object.FindAnyObjectByType<GameSession>();
                flow=UnityEngine.Object.FindAnyObjectByType<GameFlowController>();
                hud=UnityEngine.Object.FindAnyObjectByType<GameHud>();
                safe=GameObject.Find("GameCanvas").transform.Find("SafeArea");
                int overlays=0;foreach(Transform child in safe)if(child.name=="ReachedFeedback")overlays++;
                Require(overlays==1,"exactly one Journey overlay");
                flow.StartNewRun();session.DebugSetScore(3000);waitStarted=EditorApplication.timeSinceStartup;stage=1;return;
            }
            if(stage==1)
            {
                if(EditorApplication.timeSinceStartup-waitStarted<.12)return;
                AssertHidden("A retired milestone");
                flow.ShowHome();AssertHidden("A Home");flow.StartNewRun();AssertFreshHidden("A PLAY");
                session.DebugSetScore(1000);session.DebugForceGameOver();flow.ShowHome();flow.StartNewRun();AssertFreshHidden("B GameOver HOME PLAY");
                session.DebugSetScore(1000);session.DebugForceGameOver();safe.Find("GameOverPanel/Card/RetryButton").GetComponent<Button>().onClick.Invoke();AssertFreshHidden("C GameOver RETRY");
                for(int i=0;i<10;i++)
                {
                    session.DebugSetScore(1000);session.DebugForceGameOver();
                    if((i&1)==0){flow.ShowHome();AssertHidden("E Home "+i);flow.StartNewRun();}
                    else safe.Find("GameOverPanel/Card/RetryButton").GetComponent<Button>().onClick.Invoke();
                    AssertFreshHidden("E fresh "+i);
                }
                results.Add("PASS A/B/C/E: Home, PLAY and RETRY clear stale Journey feedback; x10.");
                session.DebugSetScore(950);session.DebugPrepareNextRow();
                Require(session.TryPlacePiece(session.Slots[0],3,3),"D placement");
                waitStarted=EditorApplication.timeSinceStartup;stage=2;return;
            }
            if(stage==2)
            {
                if(EditorApplication.timeSinceStartup-waitStarted<.12)return;
                Require(!hud.IsJourneyFeedbackVisible,"D retired feedback remains hidden");
                Require(hud.ReachedCount==0,"D retired milestone flags remain empty");
                results.Add("PASS D: real milestone crossing does not revive retired Journey feedback.");
                waitStarted=EditorApplication.timeSinceStartup;stage=3;return;
            }
            if(stage==3)
            {
                if(EditorApplication.timeSinceStartup-waitStarted<2.0)return;
                Require(!hud.IsJourneyFeedbackVisible,"D feedback faded and hidden");
                results.Add("PASS D: retired feedback remains hidden after its former animation window.");
                Require(errors.Length==0,"Runtime errors: "+errors);Finish(true,"");
            }
        }
        catch(Exception ex){Finish(false,ex.ToString());}
    }

    static void AssertFreshHidden(string label)
    {
        Require(flow.Screen==FlowScreen.Game&&session.State==GameState.Playing&&session.Score==0,label+" fresh run");
        AssertHidden(label);
    }

    static void AssertHidden(string label)
    {
        Require(!hud.IsJourneyFeedbackVisible,label+" overlay hidden");
        Require(string.IsNullOrEmpty(hud.JourneyFeedbackText),label+" text reset");
        Require(hud.ReachedCount==0,label+" milestone flags reset");
    }

    static void Require(bool value,string message){if(!value)throw new Exception("Journey feedback regression failed: "+message);}

    static void Finish(bool pass,string failure)
    {
        EditorApplication.update-=Check;Application.logMessageReceived-=Log;
        Directory.CreateDirectory("Validation");
        File.WriteAllText("Validation/sprint5_journey_feedback.txt",string.Join("\n",results)+"\n"+(pass?"SPRINT5_JOURNEY_FEEDBACK_PASS":failure));
        if(pass)Debug.Log("SPRINT5_JOURNEY_FEEDBACK_PASS");else Debug.LogError(failure);
        EditorApplication.Exit(pass?0:1);
    }
}
