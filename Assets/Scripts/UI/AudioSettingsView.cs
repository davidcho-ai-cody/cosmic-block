using CosmicBlock.Audio;
using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI {
 public sealed class AudioSettingsView : MonoBehaviour {
  [SerializeField] BackgroundMusicController music;
  [SerializeField] Button bgmButton,sfxButton;
  [SerializeField] Slider volume;
  [SerializeField] Text bgmText,sfxText,volumeText;
  public Slider VolumeSlider=>volume;
  public Button BgmButton=>bgmButton;
  public Button SfxButton=>sfxButton;
  public void Configure(BackgroundMusicController owner,Button bgm,Button sfx,Slider slider,Text bgmLabel,Text sfxLabel,Text volumeLabel){music=owner;bgmButton=bgm;sfxButton=sfx;volume=slider;bgmText=bgmLabel;sfxText=sfxLabel;volumeText=volumeLabel;}
  void OnEnable(){bgmButton.onClick.AddListener(ToggleMusic);sfxButton.onClick.AddListener(ToggleSfx);volume.onValueChanged.AddListener(ChangeVolume);Refresh();}
  void OnDisable(){bgmButton.onClick.RemoveListener(ToggleMusic);sfxButton.onClick.RemoveListener(ToggleSfx);volume.onValueChanged.RemoveListener(ChangeVolume);}
  void ToggleMusic(){music.SetMusicEnabled(!music.MusicEnabled);Refresh();}
  void ToggleSfx(){music.SetSfxEnabled(!music.SfxEnabled);Refresh();}
  void ChangeVolume(float value){music.SetMusicVolume(value);Refresh();}
  public void Refresh(){bgmText.text="배경음악   "+(music.MusicEnabled?"ON":"OFF");sfxText.text="효과음   "+(music.SfxEnabled?"ON":"OFF");volumeText.text="음악 볼륨   "+Mathf.RoundToInt(music.MusicVolume*100)+"%";volume.SetValueWithoutNotify(music.MusicVolume);}
 }
}
