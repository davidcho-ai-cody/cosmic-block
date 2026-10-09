using CosmicBlock.Audio;
using CosmicBlock.UI;
using CosmicBlock.Effects;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public static class Sprint1022AudioSetup {
 public static void Run(){
  foreach(var name in new[]{"home","main"}){string path="Assets/Audio/BGM/cosmic_block_"+name+"_bgm.mp3";var importer=(AudioImporter)AssetImporter.GetAtPath(path);var settings=importer.defaultSampleSettings;settings.loadType=AudioClipLoadType.Streaming;settings.compressionFormat=AudioCompressionFormat.Vorbis;settings.quality=.7f;settings.preloadAudioData=false;importer.defaultSampleSettings=settings;importer.loadInBackground=false;importer.SaveAndReimport();}
  EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");var flow=Object.FindAnyObjectByType<GameFlowController>();var old=GameObject.Find("BackgroundMusic");if(old!=null)Object.DestroyImmediate(old);
  var root=new GameObject("BackgroundMusic");var music=root.AddComponent<BackgroundMusicController>();var a=Source(root,"HomeMusic");var b=Source(root,"GameMusic");music.Configure(flow,Object.FindAnyObjectByType<GameFeedbackController>(),AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/BGM/cosmic_block_home_bgm.mp3"),AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/BGM/cosmic_block_main_bgm.mp3"),a,b);
  var panel=flow.HomeRoot.transform.Find("SettingsPanel");var card=panel.Find("Card");panel.gameObject.SetActive(false);var oldControls=card.Find("AudioControls");if(oldControls!=null)Object.DestroyImmediate(oldControls.gameObject);
  card.Find("Sfx").gameObject.SetActive(false);var haptic=card.Find("Haptic");haptic.gameObject.SetActive(true);Rect((RectTransform)haptic,new Vector2(.1f,.23f),new Vector2(.9f,.32f));
  var controls=new GameObject("AudioControls",typeof(RectTransform));controls.transform.SetParent(card,false);Rect((RectTransform)controls.transform,Vector2.zero,Vector2.one);
  var title=card.Find("Title").GetComponent<Text>();var bgm=Button(controls.transform,"BgmToggle",title,.62f,.73f,out var bgmLabel);var sfx=Button(controls.transform,"SfxToggle",title,.34f,.44f,out var sfxLabel);var label=Label(controls.transform,"VolumeLabel",title);Rect(label.rectTransform,new Vector2(.1f,.53f),new Vector2(.9f,.61f));
  var sliderRoot=new GameObject("BgmVolume",typeof(RectTransform),typeof(Image),typeof(Slider));sliderRoot.transform.SetParent(controls.transform,false);Rect((RectTransform)sliderRoot.transform,new Vector2(.12f,.46f),new Vector2(.88f,.51f));sliderRoot.GetComponent<Image>().color=new Color(.04f,.12f,.23f,1);
  var fill=Image(sliderRoot.transform,"Fill",new Color(.2f,.8f,1,1));Rect(fill.rectTransform,Vector2.zero,Vector2.one);var handle=Image(sliderRoot.transform,"Handle",new Color(1,.8f,.3f,1));Rect(handle.rectTransform,Vector2.zero,Vector2.one);handle.rectTransform.sizeDelta=new Vector2(24,12);
  var slider=sliderRoot.GetComponent<Slider>();slider.minValue=0;slider.maxValue=1;slider.fillRect=fill.rectTransform;slider.handleRect=handle.rectTransform;slider.targetGraphic=handle;slider.direction=Slider.Direction.LeftToRight;
  controls.AddComponent<AudioSettingsView>().Configure(music,bgm,sfx,slider,bgmLabel,sfxLabel,label);
  EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();Debug.Log("SPRINT1022_SETUP_PASS");EditorApplication.Exit(0);
 }
 static AudioSource Source(GameObject root,string name){var child=new GameObject(name);child.transform.SetParent(root.transform);var s=child.AddComponent<AudioSource>();s.playOnAwake=false;s.loop=true;s.spatialBlend=0;s.volume=0;return s;}
 static void Rect(RectTransform r,Vector2 min,Vector2 max){r.anchorMin=min;r.anchorMax=max;r.offsetMin=r.offsetMax=Vector2.zero;}
 static Image Image(Transform parent,string name,Color color){var go=new GameObject(name,typeof(RectTransform),typeof(Image));go.transform.SetParent(parent,false);var i=go.GetComponent<Image>();i.color=color;return i;}
 static Text Label(Transform parent,string name,Text template){var go=new GameObject(name,typeof(RectTransform),typeof(Text));go.transform.SetParent(parent,false);var t=go.GetComponent<Text>();t.font=template.font;t.fontSize=23;t.color=Color.white;t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;t.text=name;return t;}
 static Button Button(Transform parent,string name,Text template,float min,float max,out Text label){var i=Image(parent,name,new Color(.08f,.18f,.3f,.95f));Rect(i.rectTransform,new Vector2(.1f,min),new Vector2(.9f,max));var b=i.gameObject.AddComponent<Button>();b.targetGraphic=i;label=Label(i.transform,"Label",template);Rect(label.rectTransform,Vector2.zero,Vector2.one);return b;}
}
