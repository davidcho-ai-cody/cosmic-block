using System;
using System.IO;
using System.Reflection;
using CosmicBlock.Core;
using CosmicBlock.Blocks;
using CosmicBlock.Board;
using CosmicBlock.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public static class RunSaveColdProbe {
 [Serializable] public class Proof {public bool hadBest;public int oldBest;public string snapshot;}
 static bool write;static double start;static int frames;static string errors="";
 public static void CreateSnapshot()=>Begin(true);
 public static void ResumeSnapshot()=>Begin(false);
 static void Begin(bool create){write=create;RunSaveStore.PathOverride=Path.GetFullPath("Validation/run-save-cold.json");if(create){RunSaveStore.Delete();var proof=new Proof{hadBest=PlayerPrefs.HasKey(GameSession.DefaultBestScoreKey),oldBest=PlayerPrefs.GetInt(GameSession.DefaultBestScoreKey)};File.WriteAllText("Validation/run-save-cold-proof.json",JsonUtility.ToJson(proof));}EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");start=EditorApplication.timeSinceStartup;Application.logMessageReceived+=Log;EditorApplication.update+=Check;EditorApplication.EnterPlaymode();}
 static void Check(){if(EditorApplication.timeSinceStartup-start>120){Finish(false,"timeout");return;}if(!EditorApplication.isPlaying||++frames<35)return;EditorApplication.update-=Check;RunSaveStore.PathOverride=Path.GetFullPath("Validation/run-save-cold.json");var session=UnityEngine.Object.FindAnyObjectByType<GameSession>();var flow=UnityEngine.Object.FindAnyObjectByType<GameFlowController>();try{Require(flow.Screen==FlowScreen.Home,"cold HOME");var proof=JsonUtility.FromJson<Proof>(File.ReadAllText("Validation/run-save-cold-proof.json"));if(write){flow.Play();typeof(GameSession).GetMethod("DebugSetShapes",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(session,new object[]{BlockCatalog.Shapes[0]});session.DebugSetScore(980);Require(session.TryPlacePiece(session.Slots[0],0,0)&&session.TryPlacePiece(session.Slots[1],2,0),"placements");session.SuspendAndSave();proof.snapshot=JsonUtility.ToJson(RunSaveStore.Load());File.WriteAllText("Validation/run-save-cold-proof.json",JsonUtility.ToJson(proof));Require(session.Score==1000,"write1000");}else {Require(session.Score==0&&!session.RunActive&&flow.HasSavedRun,"startup did not overwrite saved run");Require(flow.HomeRoot.transform.Find("MainActions/GameStartButton").GetComponent<Image>().sprite.name=="home_continue_button","cold continue PNG");var expected=JsonUtility.FromJson<RunSnapshot>(proof.snapshot);flow.Play();Require(session.Score==1000&&session.Combo==expected.combo&&session.Restoration.PlanetId==expected.planetId,"cold restore score/planet");var board=UnityEngine.Object.FindAnyObjectByType<BoardView>().CaptureCells();for(int i=0;i<64;i++)Require(board[i]==expected.cells[i],"cold cell"+i);for(int i=0;i<3;i++)Require(session.Slots[i].Shape==BlockCatalog.Shapes[expected.shapes[i]]&&session.Slots[i].AppearanceIndex==expected.appearances[i]&&session.Slots[i].IsConsumed==expected.consumed[i],"cold piece"+i);Require(session.Restoration.CurrentEnergy==expected.planetEnergy,"cold energy no award");}Require(errors.Length==0,errors);Finish(true,write?"COLD_WRITE_PASS":"COLD_RESTART_RESUME_PASS");}catch(Exception e){Finish(false,e.ToString());}}
 static void Require(bool value,string reason){if(!value)throw new Exception(reason);}
 static void Log(string m,string s,LogType t){if(t==LogType.Error||t==LogType.Exception||t==LogType.Assert)errors+=m+"\n";}
 static void Finish(bool pass,string text){EditorApplication.update-=Check;Application.logMessageReceived-=Log;UnityEngine.Object.FindAnyObjectByType<GameSession>()?.LeaveForHome();var p=JsonUtility.FromJson<Proof>(File.ReadAllText("Validation/run-save-cold-proof.json"));if(p.hadBest)PlayerPrefs.SetInt(GameSession.DefaultBestScoreKey,p.oldBest);else PlayerPrefs.DeleteKey(GameSession.DefaultBestScoreKey);PlayerPrefs.Save();File.WriteAllText(write?"Validation/run_save_cold_write.txt":"Validation/run_save_cold_read.txt",text);Debug.Log(text);EditorApplication.Exit(pass?0:1);}
}
