using System;
using System.Collections.Generic;
using System.IO;
using CosmicBlock.Core;
using CosmicBlock.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Sprint5NewRunGameOverPlayProbe
{
    static GameSession session;static GameFlowController flow;static GameHud hud;static Transform safe;static int frames;static double started;static string errors="";
    static readonly List<string> results=new List<string>();
    public static void Run()
    {
        EditorSceneManager.OpenScene(Sprint5Builder.ScenePath);started=EditorApplication.timeSinceStartup;frames=0;errors="";results.Clear();
        Application.logMessageReceived+=Log;EditorApplication.update+=Check;EditorApplication.EnterPlaymode();
    }
    static void Log(string m,string stack,LogType type){if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert)errors+=m+"\n";}
    static void Check()
    {
        if(EditorApplication.timeSinceStartup-started>180){Finish(false,"timeout");return;}if(!EditorApplication.isPlaying||++frames<30)return;
        try
        {
            session=UnityEngine.Object.FindAnyObjectByType<GameSession>();flow=UnityEngine.Object.FindAnyObjectByType<GameFlowController>();hud=UnityEngine.Object.FindAnyObjectByType<GameHud>();safe=GameObject.Find("GameCanvas").transform.Find("SafeArea");
            int panels=0,huds=UnityEngine.Object.FindObjectsByType<GameHud>(FindObjectsInactive.Include).Length,sessions=UnityEngine.Object.FindObjectsByType<GameSession>(FindObjectsInactive.Include).Length;
            foreach(Transform child in safe)if(child.name=="GameOverPanel")panels++;
            Require(panels==1&&huds==1&&sessions==1,"G duplicate Game Over/session/controller objects");
            Require(Array.IndexOf(flow.GameRoots,hud.GameOverRoot)<0,"G GameOverPanel excluded from game roots");
            results.Add("PASS G: one GameOverPanel, GameHud and GameSession; panel excluded from flow roots.");

            flow.Play();AssertFresh("A cold HOME PLAY");results.Add("PASS A: cold HOME to PLAY starts score 0 with three pieces and no Game Over.");
            for(int i=0;i<10;i++){flow.ShowHome();flow.Play();AssertFresh("B loop "+i);}results.Add("PASS B: HOME to PLAY x10 without immediate Game Over.");

            session.DebugForceGameOver();AssertGameOver("C setup");flow.ShowHome();flow.Play();AssertFresh("C HOME PLAY");results.Add("PASS C: real Game Over to HOME to PLAY starts normally.");
            session.DebugForceGameOver();AssertGameOver("D setup");safe.Find("GameOverPanel/Card/RetryButton").GetComponent<Button>().onClick.Invoke();AssertFresh("D RETRY");results.Add("PASS D: real Game Over to RETRY starts normally.");

            flow.ShowHome();flow.Play();AssertFresh("E active Home Play");results.Add("PASS E: active-run HOME/PLAY path has no state leakage.");
            session.DebugForceGameOver();AssertGameOver("F actual condition");results.Add("PASS F: all remaining pieces unplaceable activates real Game Over.");

            flow.ShowHome();flow.Play();AssertFresh("H setup");session.DebugSetScore(1000);flow.ShowHome();flow.Play();Require(session.Score==1000&&session.State==GameState.Playing,"H resume score");
            Require(!hud.IsJourneyFeedbackVisible&&string.IsNullOrEmpty(hud.JourneyFeedbackText),"H stale Journey feedback");
            results.Add("PASS H: Journey feedback remains clean across resumed run.");
            Require(errors.Length==0,"runtime errors: "+errors);Finish(true,"");
        }catch(Exception ex){Finish(false,ex.ToString());}
    }
    static void AssertFresh(string label)
    {
        Require(session.State==GameState.Playing&&session.Score==0&&session.Combo==0&&session.BlockSetNumber==1,label+" state");
        Require(!hud.GameOverRoot.activeSelf,label+" panel");
        Require(session.Slots.Count==3,label+" slot count");
        foreach(var piece in session.Slots)Require(piece!=null&&!piece.IsConsumed&&session.Model.CanPlaceAnywhere(piece.Shape),label+" supplied piece");
    }
    static void AssertGameOver(string label){Require(session.State==GameState.GameOver&&hud.GameOverRoot.activeSelf,label);}
    static void Require(bool v,string m){if(!v)throw new Exception("New Run Game Over regression failed: "+m);}
    static void Finish(bool pass,string failure)
    {
        EditorApplication.update-=Check;Application.logMessageReceived-=Log;Directory.CreateDirectory("Validation");
        File.WriteAllText("Validation/sprint5_new_run_gameover.txt",string.Join("\n",results)+"\n"+(pass?"SPRINT5_NEW_RUN_GAMEOVER_PASS":failure));
        if(pass)Debug.Log("SPRINT5_NEW_RUN_GAMEOVER_PASS");else Debug.LogError(failure);EditorApplication.Exit(pass?0:1);
    }
}
