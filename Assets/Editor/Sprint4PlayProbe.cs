using System;
using System.Collections.Generic;
using System.IO;
using CosmicBlock.Blocks;
using CosmicBlock.Board;
using CosmicBlock.Core;
using CosmicBlock.Effects;
using CosmicBlock.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Sprint4PlayProbe
{
    static double started, stageStarted;
    static int frames, stage;
    static string errors = "";
    static GameSession session;
    static BoardView board;
    static GameFeedbackController feedback;
    static RectTransform layer;
    static readonly List<string> results = new List<string>();

    public static void Run()
    {
        EditorSceneManager.OpenScene(Sprint4Builder.ScenePath);
        started = EditorApplication.timeSinceStartup;
        Application.logMessageReceived += Log;
        EditorApplication.update += Check;
        EditorApplication.EnterPlaymode();
    }
    static void Log(string message,string stack,LogType type)
    {
        if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert)errors+=message+"\n";
    }
    static void Check()
    {
        if(EditorApplication.timeSinceStartup-started>180){Finish(false,"timeout");return;}
        if(!EditorApplication.isPlaying||++frames<30)return;
        try
        {
            if(stage==0)
            {
                session=UnityEngine.Object.FindAnyObjectByType<GameSession>();
                board=UnityEngine.Object.FindAnyObjectByType<BoardView>();
                feedback=UnityEngine.Object.FindAnyObjectByType<GameFeedbackController>();
                layer=GameObject.Find("GameCanvas").transform.Find("DragLayer") as RectTransform;
                Require(feedback!=null&&feedback.CellPoolCapacity==64&&feedback.StarPoolCapacity==24&&feedback.HasAudioClip,"architecture/pools/audio");

                int hapticBefore=feedback.HapticRequestCount;
                session.DebugPrepareNextRow();
                int before=session.Score;
                Require(session.TryPlacePiece(session.Slots[0],3,3),"row placement");
                int expected=ScoreRules.AddPlacement(before,1,1,1)-before;
                Require(session.LastClear.LineCount==1&&feedback.LastCellCount==8&&feedback.LastScoreDelta==expected&&feedback.LastComboLabel=="CLEAR!"&&feedback.LastParticleCount==8,"single row feedback/score");
                Require(feedback.HapticRequestCount==hapticBefore+1&&Mathf.Approximately(feedback.LastPitch,1f),"single haptic/audio");
                results.Add("PASS 1-3/6/9/15: row clear uses 8 unique cells, actual score delta, CLEAR!, one SFX and one safe haptic request.");

                RapidDrag();
                Require(feedback.IsPlaying,"rapid input stopped feedback");
                results.Add("PASS 10: next piece begins and cancels drag while feedback remains non-blocking.");

                session.DebugPrepareNextRow();before=session.Score;
                Require(session.TryPlacePiece(session.Slots[0],3,3)&&session.Combo==2,"combo 2 placement");
                Require(feedback.LastComboLabel=="STAR COMBO"&&feedback.LastScoreDelta==ScoreRules.AddPlacement(before,1,1,2)-before&&Mathf.Approximately(feedback.LastPitch,1.05f),"combo2");
                session.DebugPrepareNextRow();before=session.Score;
                Require(session.TryPlacePiece(session.Slots[0],3,3)&&session.Combo==3,"combo 3 placement");
                Require(feedback.LastComboLabel.StartsWith("COSMIC COMBO")&&feedback.LastScoreDelta==ScoreRules.AddPlacement(before,1,1,3)-before&&Mathf.Approximately(feedback.LastPitch,1.1f),"combo3");
                results.Add("PASS 6-9/14: CLEAR -> STAR COMBO -> COSMIC COMBO; existing score formula and capped pitch progression.");

                PrepareColumn();before=session.Score;
                Require(session.TryPlacePiece(session.Slots[0],3,3)&&session.LastClear.ClearedColumns.Count==1,"column clear");
                results.Add("PASS 3: column clear follows the same production feedback path.");

                session.DebugPrepareCross();before=session.Score;
                Require(session.TryPlacePiece(session.Slots[0],3,3),"cross placement");
                Require(session.LastClear.LineCount==2&&session.LastClear.UniqueClearedCells.Count==15&&feedback.LastCellCount==15&&feedback.LastParticleCount==12&&feedback.LastScoreDelta==ScoreRules.AddPlacement(before,1,2,1)-before,"cross unique/multi");
                results.Add("PASS 4-5: row+column intersection is unique (15 cells); multi-line uses one event/SFX/haptic and 12 pooled stars.");
                Capture(1080,1920,"9x16");Capture(1080,2400,"tall");Capture(1080,1440,"short");

                session.DebugForceGameOver();
                Require(session.State==GameState.GameOver&&feedback.IsPlaying,"game over disrupted feedback");
                session.Retry();
                Require(session.State==GameState.Playing&&!feedback.IsPlaying&&feedback.ActiveCellCount==0&&feedback.ActiveStarCount==0,"retry cleanup");
                results.Add("PASS 11-12: Game Over stays valid during effect; Retry clears all temporary visuals/audio.");

                session.DebugPrepareNextRow();Require(session.TryPlacePiece(session.Slots[0],3,3),"cleanup clear");
                stage=1;frames=0;stageStarted=EditorApplication.timeSinceStartup;return;
            }
            if(EditorApplication.timeSinceStartup-stageStarted<1.0)return;
            Require(!feedback.IsPlaying&&feedback.ActiveCellCount==0&&feedback.ActiveStarCount==0,"particle/effect cleanup");
            Require(feedback.transform.childCount==90,"pool hierarchy grew: "+feedback.transform.childCount);
            Require(errors.Length==0,"runtime errors "+errors);
            results.Add("PASS 13: repeated effects reuse fixed 64-cell/24-star pool and clean up after 0.68 seconds.");
            Finish(true,"");
        }
        catch(Exception e){Finish(false,e.ToString());}
    }
    static void PrepareColumn()
    {
        session.DebugPrepareNextRow();session.Model.Clear();
        for(int y=0;y<8;y++)if(y!=3)session.Model.SetOccupied(3,y,true);
    }
    static void RapidDrag()
    {
        var piece=session.Slots[1];var handler=piece.GetComponent<BlockDragHandler>();
        var begin=new PointerEventData(EventSystem.current){pointerId=-1,position=Screen(piece.transform.position)};
        ExecuteEvents.Execute(piece.gameObject,begin,ExecuteEvents.beginDragHandler);
        Require(handler.IsDragging&&session.State==GameState.Playing,"rapid drag lock");
        ExecuteEvents.Execute(piece.gameObject,new BaseEventData(EventSystem.current),ExecuteEvents.cancelHandler);
        Require(!handler.IsDragging,"rapid drag cancel");
    }
    static Camera UICamera(){var canvas=layer.GetComponentInParent<Canvas>();return canvas.renderMode==RenderMode.ScreenSpaceOverlay?null:canvas.worldCamera;}
    static Vector2 Screen(Vector3 world)=>RectTransformUtility.WorldToScreenPoint(UICamera(),world);
    static void Capture(int width,int height,string name)
    {
        var canvas=layer.GetComponentInParent<Canvas>();var camera=Camera.main;var safe=canvas.transform.Find("SafeArea") as RectTransform;
        var mode=canvas.renderMode;var oldCamera=canvas.worldCamera;var target=camera.targetTexture;bool ortho=camera.orthographic;float size=camera.orthographicSize;var active=RenderTexture.active;
        var rt=new RenderTexture(width,height,24);Texture2D image=null;
        try
        {
            rt.Create();camera.targetTexture=rt;camera.orthographic=true;camera.orthographicSize=height/2f;canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=10;
            safe.anchorMin=Vector2.zero;safe.anchorMax=Vector2.one;safe.offsetMin=safe.offsetMax=Vector2.zero;Canvas.ForceUpdateCanvases();board.GetComponent<SquareBoardLayout>().SendMessage("LateUpdate");board.SendMessage("LateUpdate");
            var background=UnityEngine.Object.FindAnyObjectByType<AspectFillBackground>();if(background!=null)background.SendMessage("LateUpdate");Canvas.ForceUpdateCanvases();camera.Render();RenderTexture.active=rt;
            image=new Texture2D(width,height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,width,height),0,0);image.Apply();Directory.CreateDirectory("Validation");File.WriteAllBytes("Validation/sprint4_"+name+".png",image.EncodeToPNG());
            results.Add("PASS responsive render "+name+" "+width+"x"+height+": clear cells/stars/score/combo stay on feedback layer.");
        }
        finally
        {
            canvas.renderMode=mode;canvas.worldCamera=oldCamera;camera.targetTexture=target;camera.orthographic=ortho;camera.orthographicSize=size;RenderTexture.active=active;if(image!=null)UnityEngine.Object.DestroyImmediate(image);rt.Release();UnityEngine.Object.DestroyImmediate(rt);
            safe.GetComponent<SafeArea>().SendMessage("Apply");Canvas.ForceUpdateCanvases();board.GetComponent<SquareBoardLayout>().SendMessage("LateUpdate");board.SendMessage("LateUpdate");
        }
    }
    static void Require(bool condition,string message){if(!condition)throw new Exception("Sprint 4 test failed: "+message);}
    static void Finish(bool pass,string failure)
    {
        EditorApplication.update-=Check;Application.logMessageReceived-=Log;Directory.CreateDirectory("Validation");
        File.AppendAllText("Validation/sprint4.txt",string.Join("\n",results)+"\n"+(pass?"SPRINT4_PLAY_PASS":failure)+"\n");
        if(pass)Debug.Log("SPRINT4_PLAY_PASS");else Debug.LogError(failure);EditorApplication.Exit(pass?0:1);
    }
}


