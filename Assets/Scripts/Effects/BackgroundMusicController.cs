using CosmicBlock.UI;
using CosmicBlock.Effects;
using UnityEngine;
namespace CosmicBlock.Audio {
 public sealed class BackgroundMusicController : MonoBehaviour {
  public const string EnabledKey="CosmicBlock.Audio.BgmEnabled", VolumeKey="CosmicBlock.Audio.BgmVolume", SfxKey="CosmicBlock.Audio.SfxEnabled";
  public static BackgroundMusicController Current {get;private set;}
  [SerializeField] GameFlowController flow;
  [SerializeField] GameFeedbackController feedback;
  [SerializeField] AudioClip homeClip,gameClip;
  [SerializeField] AudioSource homeSource,gameSource;
  public bool MusicEnabled {get;private set;}
  public bool SfxEnabled {get;private set;}
  public float MusicVolume {get;private set;}
  public float GameMix {get;private set;}
  public bool Suspended=>paused||unfocused;
  public AudioSource HomeSource=>homeSource;
  public AudioSource GameSource=>gameSource;
  bool paused,unfocused,ready; float target;
#if DEVELOPMENT_BUILD || UNITY_EDITOR
  string lastDiagnostic; float nextDiagnostic;
#endif
  public void Configure(GameFlowController owner,GameFeedbackController effects,AudioClip home,AudioClip game,AudioSource a,AudioSource b){flow=owner;feedback=effects;homeClip=home;gameClip=game;homeSource=a;gameSource=b;}
  void Awake(){if(Current!=null&&Current!=this){enabled=false;return;}Current=this;ReloadSettings();Setup(homeSource,homeClip);Setup(gameSource,gameClip);ready=true;}
  void Setup(AudioSource source,AudioClip clip){source.Stop();source.clip=clip;source.loop=true;source.playOnAwake=false;source.spatialBlend=0;source.volume=0;source.ignoreListenerPause=false;}
  public void ReloadSettings(){MusicEnabled=PlayerPrefs.GetInt(EnabledKey,1)!=0;MusicVolume=Mathf.Clamp01(PlayerPrefs.GetFloat(VolumeKey,.35f));SfxEnabled=PlayerPrefs.GetInt(SfxKey,1)!=0;if(feedback!=null)feedback.SoundEnabled=SfxEnabled;if(ready)ApplyVolumes();}
  public void SetMusicEnabled(bool value){MusicEnabled=value;PlayerPrefs.SetInt(EnabledKey,value?1:0);PlayerPrefs.Save();ApplyVolumes();}
  public void SetMusicVolume(float value){MusicVolume=Mathf.Clamp01(value);PlayerPrefs.SetFloat(VolumeKey,MusicVolume);PlayerPrefs.Save();ApplyVolumes();}
  public void SetSfxEnabled(bool value){SfxEnabled=value;if(feedback!=null)feedback.SoundEnabled=value;PlayerPrefs.SetInt(SfxKey,value?1:0);PlayerPrefs.Save();}
  void Update(){if(!ready||Suspended)return;target=flow!=null&&flow.Screen==FlowScreen.Game?1:0;
   GameMix=Mathf.MoveTowards(GameMix,target,Time.unscaledDeltaTime/1.5f);ApplyVolumes();
   PlayNeeded(homeSource,GameMix<1);PlayNeeded(gameSource,GameMix>0);ReportDiagnostic(false);
  }
  void PlayNeeded(AudioSource source,bool needed){if(!needed){if(source.isPlaying)source.Stop();return;}if(!source.isPlaying&&source.clip!=null)source.Play();}
  void ApplyVolumes(){if(!ready)return;float v=MusicEnabled?MusicVolume:0;homeSource.volume=(1-GameMix)*v;gameSource.volume=GameMix*v;}
  void OnApplicationPause(bool value){bool was=Suspended;paused=value;SetSuspended(was);}
  void OnApplicationFocus(bool value){bool was=Suspended;unfocused=!value;SetSuspended(was);}
  void SetSuspended(bool was){if(!ready||was==Suspended)return;foreach(var s in new[]{homeSource,gameSource}){if(Suspended)s.Pause();else s.UnPause();}ReportDiagnostic(true);}
  [System.Diagnostics.Conditional("DEVELOPMENT_BUILD"),System.Diagnostics.Conditional("UNITY_EDITOR")]
  void ReportDiagnostic(bool force){
#if DEVELOPMENT_BUILD || UNITY_EDITOR
   if(!ready)return;string state=(flow==null?"None":flow.Screen.ToString())+"/"+Mathf.RoundToInt(GameMix*4)+"/"+MusicEnabled+"/"+MusicVolume+"/"+SfxEnabled+"/"+Suspended;
   if(!force&&(state==lastDiagnostic||Time.unscaledTime<nextDiagnostic))return;lastDiagnostic=state;nextDiagnostic=Time.unscaledTime+.15f;
   Debug.Log("BGM_QA "+JsonUtility.ToJson(new Diagnostic{screen=flow==null?"None":flow.Screen.ToString(),mix=GameMix,enabled=MusicEnabled,volume=MusicVolume,sfx=SfxEnabled,suspended=Suspended,homePlaying=homeSource.isPlaying,gamePlaying=gameSource.isPlaying,homeVolume=homeSource.volume,gameVolume=gameSource.volume,homeTime=homeSource.time,gameTime=gameSource.time}));
#endif
  }
#if DEVELOPMENT_BUILD || UNITY_EDITOR
  [System.Serializable] sealed class Diagnostic {public string screen;public float mix,volume,homeVolume,gameVolume,homeTime,gameTime;public bool enabled,sfx,suspended,homePlaying,gamePlaying;}
#endif
  void OnDisable(){if(homeSource!=null)homeSource.Stop();if(gameSource!=null)gameSource.Stop();}
  void OnDestroy(){if(Current==this)Current=null;}
 }
}
