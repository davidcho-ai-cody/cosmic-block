using CosmicBlock.Core;
using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI {
 public sealed class PlanetDebugPanel:MonoBehaviour {
  [SerializeField] GameSession session;[SerializeField] GameFlowController flow;[SerializeField] Button launcher,closeButton;[SerializeField] GameObject panel;[SerializeField] Button[] presetButtons;
  static readonly int[] Presets={0,90,190,290,390};
  public static bool ShouldShow(bool debugBuild)=>debugBuild;public bool IsAvailable=>ShouldShow(Debug.isDebugBuild);public bool IsPanelVisible=>panel!=null&&panel.activeSelf;
  public void Configure(GameSession owner,GameFlowController controller,Button devButton,GameObject panelRoot,Button close,Button[] buttons){RemoveListeners();session=owner;flow=controller;launcher=devButton;panel=panelRoot;closeButton=close;presetButtons=buttons;AddListeners();ApplyAvailability();}
  void Awake(){AddListeners();ApplyAvailability();}
  void ApplyAvailability(){bool available=ShouldShow(Debug.isDebugBuild);if(launcher!=null)launcher.gameObject.SetActive(available);if(panel!=null)panel.SetActive(false);}
  void AddListeners(){if(launcher!=null){launcher.onClick.RemoveListener(Toggle);launcher.onClick.AddListener(Toggle);}if(closeButton!=null){closeButton.onClick.RemoveListener(Close);closeButton.onClick.AddListener(Close);}if(presetButtons==null)return;for(int i=0;i<presetButtons.Length&&i<Presets.Length;i++){int value=Presets[i];var button=presetButtons[i];if(button==null)continue;button.onClick.RemoveAllListeners();button.onClick.AddListener(()=>ApplyPreset(value));}}
  void RemoveListeners(){if(launcher!=null)launcher.onClick.RemoveListener(Toggle);if(closeButton!=null)closeButton.onClick.RemoveListener(Close);if(presetButtons!=null)foreach(var button in presetButtons)if(button!=null)button.onClick.RemoveAllListeners();}
  public void Toggle(){if(!ShouldShow(Debug.isDebugBuild)||panel==null)return;panel.SetActive(!panel.activeSelf);}
  public void Close(){if(panel!=null)panel.SetActive(false);}
  public void ApplyPreset(int energy){
#if UNITY_EDITOR || DEVELOPMENT_BUILD
   if(!ShouldShow(Debug.isDebugBuild)||session==null)return;session.ResetTransientFeedback();session.DebugSetPlanetEnergy(Mathf.Clamp(energy,0,390));if(flow!=null)flow.RefreshHome();Close();
#endif
  }
  void OnDestroy()=>RemoveListeners();
 }
}
