using System;
using System.Collections.Generic;
using System.IO;
using CosmicBlock.Core;
using CosmicBlock.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public static class Sprint3PlayProbe {
 const string Key="CosmicBlock.Tests.Sprint3.BestScore";static bool had;static int old,frames,stage;static double started;static string errors="";static GameSession session;static GameHud hud;static readonly List<string> results=new List<string>();
 public static void Run(){Sprint3Builder.Validate();EditorSceneManager.OpenScene(Sprint3Builder.ScenePath);var s=UnityEngine.Object.FindAnyObjectByType<GameSession>();var so=new SerializedObject(s);so.FindProperty("bestScoreKey").stringValue=Key;so.ApplyModifiedPropertiesWithoutUndo();had=PlayerPrefs.HasKey(Key);old=PlayerPrefs.GetInt(Key,0);PlayerPrefs.SetInt(Key,8520);PlayerPrefs.Save();started=EditorApplication.timeSinceStartup;frames=stage=0;Application.logMessageReceived+=Log;EditorApplication.update+=Check;EditorApplication.EnterPlaymode();}
 static void Log(string m,string st,LogType t){if(t==LogType.Error||t==LogType.Exception||t==LogType.Assert)errors+=m+"\n";}
 static void Check(){if(EditorApplication.timeSinceStartup-started>120){Finish(false,"timeout");return;}if(!EditorApplication.isPlaying||++frames<30)return;try{
  if(stage==0){session=UnityEngine.Object.FindAnyObjectByType<GameSession>();hud=UnityEngine.Object.FindAnyObjectByType<GameHud>();Require(session.Score==0&&session.Journey.Current.Name=="START"&&session.BestJourney.Current.Name=="SATURN","Test 1/9 initial and best");results.Add("PASS Test 1/9: Score 0 START; Best 8,520 -> SATURN.");
   TestScore(1000,"STAR FIELD",1);TestScore(3000,"MOON",2);TestScore(6000,"SATURN",3);TestScore(10000,"DEEP SPACE",4);Require(!session.Journey.Next.HasValue&&session.State==GameState.Playing,"Test 5 continues");results.Add("PASS Tests 2-5: all destinations; DEEP SPACE remains Playing.");
   session.DebugPrepareCross();session.DebugSetScore(2950);Require(session.TryPlacePiece(session.Slots[0],3,3)&&session.Score==3160&&session.Journey.Current.Name=="MOON"&&hud.ReachedCount==2,"Test 6 placement crossing");int count=hud.ReachedCount;session.DebugSetScore(3200);Require(hud.ReachedCount==count,"Test 7 duplicate");results.Add("PASS Tests 6-7: crossing feedback once per destination.");
   session.Retry();Require(session.Score==0&&session.Journey.Current.Name=="START"&&session.BestScore==8520&&hud.ReachedCount==0,"Test 8 retry");results.Add("PASS Test 8: Retry starts Journey at START, preserves Best, resets run feedback.");
   session.DebugSetScore(5420);session.DebugForceGameOver();var final=GameObject.Find("GameCanvas").transform.Find("SafeArea/GameOverPanel/Card/FinalScore").GetComponent<Text>();Require(session.State==GameState.GameOver&&final.text.Contains("MOON  >  SATURN")&&final.text.Contains("BEST JOURNEY  SATURN"),"Test 10 gameover");results.Add("PASS Test 10: Game Over shows current route/progress and Best Journey.");
   Require(GameObject.Find("GameCanvas").transform.Find("SafeArea/Journey")!=null&&GameObject.Find("GameCanvas").transform.Find("SafeArea/BlockArea")!=null,"Test 12 UI");results.Add("PASS Test 12: Journey/Board/slots remain in SafeArea hierarchy.");stage=1;frames=0;return;}
  Require(errors.Length==0,"runtime errors "+errors);Finish(true,"");
 }catch(Exception e){Finish(false,e.ToString());}}
 static void TestScore(int score,string name,int count){session.DebugSetScore(score);Require(session.Journey.Current.Name==name&&hud.ReachedCount==count,"milestone "+score);}
 static void Require(bool c,string m){if(!c)throw new Exception("Sprint 3 test failed: "+m);}
 static void Finish(bool pass,string failure){EditorApplication.update-=Check;Application.logMessageReceived-=Log;if(had)PlayerPrefs.SetInt(Key,old);else PlayerPrefs.DeleteKey(Key);PlayerPrefs.Save();Directory.CreateDirectory("Validation");File.AppendAllText("Validation/sprint3.txt",string.Join("\n",results)+"\n"+(pass?"SPRINT3_PLAY_PASS":failure));if(pass)Debug.Log("SPRINT3_PLAY_PASS");else Debug.LogError(failure);EditorApplication.Exit(pass?0:1);}
}

