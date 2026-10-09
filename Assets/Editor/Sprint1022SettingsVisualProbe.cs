using System;
using System.IO;
using CosmicBlock.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public static class Sprint1022SettingsVisualProbe {
 static double start;static int frames;
 public static void Run(){EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");start=EditorApplication.timeSinceStartup;EditorApplication.update+=Check;EditorApplication.EnterPlaymode();}
 static void Check(){if(EditorApplication.timeSinceStartup-start>120){Debug.LogError("설정 렌더 시간 초과");EditorApplication.Exit(1);return;}if(!EditorApplication.isPlaying||++frames<35)return;EditorApplication.update-=Check;try{var flow=UnityEngine.Object.FindAnyObjectByType<GameFlowController>();flow.ShowHome();flow.HomeView.ShowSettings();foreach(int h in new[]{1920,2400,1440})Capture(h,flow);Debug.Log("SPRINT1022_VISUAL_PASS");EditorApplication.Exit(0);}catch(Exception ex){Debug.LogException(ex);EditorApplication.Exit(1);}}
 static void Capture(int h,GameFlowController flow){var canvas=GameObject.Find("GameCanvas").GetComponent<Canvas>();var safe=(RectTransform)canvas.transform.Find("SafeArea");safe.GetComponent<SafeArea>().enabled=false;var cam=Camera.main;var rt=new RenderTexture(1080,h,24);rt.Create();cam.targetTexture=rt;canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=cam;canvas.planeDistance=10;safe.anchorMin=h==2400?new Vector2(.02f,.04f):Vector2.zero;safe.anchorMax=h==2400?new Vector2(.98f,.94f):Vector2.one;safe.offsetMin=safe.offsetMax=Vector2.zero;Canvas.ForceUpdateCanvases();flow.HomeView.Refresh(UnityEngine.Object.FindAnyObjectByType<CosmicBlock.Core.GameSession>());Canvas.ForceUpdateCanvases();flow.HomeRoot.GetComponent<HomeFinalLayout>().Apply();Canvas.ForceUpdateCanvases();
  foreach(var t in flow.HomeRoot.transform.Find("SettingsPanel").GetComponentsInChildren<Text>()){if(t.cachedTextGenerator.characterCountVisible<t.text.Replace("\n","").Length)throw new Exception("설정 텍스트 잘림 "+h+" "+t.name);}
  var slider=flow.HomeRoot.GetComponentInChildren<AudioSettingsView>().VolumeSlider;var corners=new Vector3[4];slider.GetComponent<RectTransform>().GetWorldCorners(corners);foreach(var c in corners)if(!safe.rect.Contains(safe.InverseTransformPoint(c)))throw new Exception("Safe Area 밖 Slider "+h);
  cam.Render();RenderTexture.active=rt;var tex=new Texture2D(1080,h,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,1080,h),0,0);tex.Apply();File.WriteAllBytes("Validation/sprint1022_settings_"+h+".png",tex.EncodeToPNG());RenderTexture.active=null;cam.targetTexture=null;UnityEngine.Object.DestroyImmediate(tex);rt.Release();UnityEngine.Object.DestroyImmediate(rt);Debug.Log("설정 렌더 PASS "+h);
 }
}
