using CosmicBlock.Core;
using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI {
 public sealed class PlanetDebugPanel:MonoBehaviour {
  [SerializeField] GameSession session;[SerializeField] GameFlowController flow;[SerializeField] Button launcher,closeButton;[SerializeField] GameObject panel;[SerializeField] Button[] presetButtons;
  const int PresetCount=5;
  public static bool ShouldShow(bool debugBuild)=>debugBuild;public bool IsAvailable=>ShouldShow(Debug.isDebugBuild);public bool IsPanelVisible=>panel!=null&&panel.activeSelf;
  public void Configure(GameSession owner,GameFlowController controller,Button devButton,GameObject panelRoot,Button close,Button[] buttons){RemoveListeners();session=owner;flow=controller;launcher=devButton;panel=panelRoot;closeButton=close;presetButtons=buttons;AddListeners();ApplyAvailability();}
  void Awake(){AddListeners();ApplyAvailability();}
  void ApplyAvailability(){bool available=ShouldShow(Debug.isDebugBuild);if(launcher!=null)launcher.gameObject.SetActive(available);if(panel!=null)panel.SetActive(false);}
  void AddListeners(){if(launcher!=null){launcher.onClick.RemoveListener(Toggle);launcher.onClick.AddListener(Toggle);}if(closeButton!=null){closeButton.onClick.RemoveListener(Close);closeButton.onClick.AddListener(Close);}if(presetButtons==null)return;for(int i=0;i<presetButtons.Length&&i<PresetCount;i++){int index=i;var button=presetButtons[i];if(button==null)continue;button.onClick.RemoveAllListeners();button.onClick.AddListener(()=>ApplyPreset(PresetEnergy(index)));}}
  void RemoveListeners(){if(launcher!=null)launcher.onClick.RemoveListener(Toggle);if(closeButton!=null)closeButton.onClick.RemoveListener(Close);if(presetButtons!=null)foreach(var button in presetButtons)if(button!=null)button.onClick.RemoveAllListeners();}
  int PresetEnergy(int index)=>index==0?0:index==4?session.Restoration.Total-10:PlanetRestoration.StartForStage(index+1,session.Restoration.PlanetId)-10;
  void RefreshLabels(){if(presetButtons==null||session==null||session.Restoration==null)return;for(int i=0;i<presetButtons.Length&&i<PresetCount;i++){if(presetButtons[i]==null)continue;var label=presetButtons[i].GetComponentInChildren<Text>(true);if(label!=null)label.text=i==0?"RESET PLANET":(i==4?"FINAL TEST":"STAGE "+i+" TEST")+" · "+PresetEnergy(i);}}
  public void Toggle(){if(!ShouldShow(Debug.isDebugBuild)||panel==null)return;RefreshLabels();panel.SetActive(!panel.activeSelf);}
  public void Close(){if(panel!=null)panel.SetActive(false);}
  public void ApplyPreset(int energy){
#if UNITY_EDITOR || DEVELOPMENT_BUILD
   if(!ShouldShow(Debug.isDebugBuild)||session==null)return;session.ResetTransientFeedback();session.DebugSetPlanetEnergy(Mathf.Clamp(energy,0,session.Restoration.Total));if(flow!=null)flow.RefreshHome();Close();
#endif
  }
  void OnDestroy()=>RemoveListeners();
 }
}
