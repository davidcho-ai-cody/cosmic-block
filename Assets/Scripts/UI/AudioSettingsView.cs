using CosmicBlock.Audio;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace CosmicBlock.UI {
 public sealed class AudioSettingsView : MonoBehaviour {
  [SerializeField] BackgroundMusicController music;
  [SerializeField] Button bgmButton,sfxButton;
  [SerializeField] Slider volume;
  [SerializeField] Text bgmText,sfxText,volumeText;
  [SerializeField] TMP_Text bgmLabel,sfxLabel,volumeLabel;
  public Slider VolumeSlider=>volume;
  public Button BgmButton=>bgmButton;
  public Button SfxButton=>sfxButton;
  public void Configure(BackgroundMusicController owner,Button bgm,Button sfx,Slider slider,Text bgmLabel,Text sfxLabel,Text volumeLabel){music=owner;bgmButton=bgm;sfxButton=sfx;volume=slider;bgmText=bgmLabel;sfxText=sfxLabel;volumeText=volumeLabel;}
  public void Configure(BackgroundMusicController owner,Button bgm,Button sfx,Slider slider,TMP_Text bgmCaption,TMP_Text sfxCaption,TMP_Text volumeCaption){music=owner;bgmButton=bgm;sfxButton=sfx;volume=slider;bgmText=sfxText=volumeText=null;bgmLabel=bgmCaption;sfxLabel=sfxCaption;volumeLabel=volumeCaption;}
  static void Caption(Text legacy,TMP_Text label,string value){if(label!=null)label.text=value;else if(legacy!=null)legacy.text=value;}
  void OnEnable(){bgmButton.onClick.AddListener(ToggleMusic);sfxButton.onClick.AddListener(ToggleSfx);volume.onValueChanged.AddListener(ChangeVolume);Refresh();}
  void OnDisable(){bgmButton.onClick.RemoveListener(ToggleMusic);sfxButton.onClick.RemoveListener(ToggleSfx);volume.onValueChanged.RemoveListener(ChangeVolume);}
  void ToggleMusic(){music.SetMusicEnabled(!music.MusicEnabled);Refresh();}
  void ToggleSfx(){music.SetSfxEnabled(!music.SfxEnabled);Refresh();}
  void ChangeVolume(float value){music.SetMusicVolume(value);Refresh();}
  public void Refresh(){Caption(bgmText,bgmLabel,"배경음악   "+(music.MusicEnabled?"ON":"OFF"));Caption(sfxText,sfxLabel,"효과음   "+(music.SfxEnabled?"ON":"OFF"));Caption(volumeText,volumeLabel,"음악 볼륨   "+Mathf.RoundToInt(music.MusicVolume*100)+"%");volume.SetValueWithoutNotify(music.MusicVolume);}
 }
}
