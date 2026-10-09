using System;
using System.IO;
using CosmicBlock.Core;
using CosmicBlock.UI;
using UnityEditor;
using UnityEngine;
[InitializeOnLoad] public static class CompletionNoticeProbeScope {
 [Serializable] sealed class Data {public bool[] had=new bool[5];public int[] values=new int[5];}
 static readonly string path;static readonly Data backup;
 static CompletionNoticeProbeScope(){path=Environment.GetEnvironmentVariable("COSMIC_NOTICE_SCOPE_PATH");if(string.IsNullOrEmpty(path))return;if(File.Exists(path))backup=JsonUtility.FromJson<Data>(File.ReadAllText(path));else{backup=new Data();for(int i=0;i<5;i++){string k=PlanetCompletionNotice.Key(i+1);backup.had[i]=PlayerPrefs.HasKey(k);backup.values[i]=PlayerPrefs.GetInt(k);}File.WriteAllText(path,JsonUtility.ToJson(backup));}EditorApplication.quitting+=Restore;if(Environment.GetEnvironmentVariable("COSMIC_NOTICE_LEGACY_PROBE")=="1")EditorApplication.update+=Legacy;}
 static void Legacy(){if(!EditorApplication.isPlaying)return;var popup=UnityEngine.Object.FindAnyObjectByType<PlanetCompletionPopup>();if(popup!=null&&popup.Visible)popup.Continue();for(int id=1;id<=5;id++)if(PlanetCompletionNotice.Pending(id))PlanetCompletionNotice.Acknowledge(id);}
 static void Restore(){for(int i=0;i<5;i++){string k=PlanetCompletionNotice.Key(i+1);if(backup.had[i])PlayerPrefs.SetInt(k,backup.values[i]);else PlayerPrefs.DeleteKey(k);}PlayerPrefs.Save();}
}
