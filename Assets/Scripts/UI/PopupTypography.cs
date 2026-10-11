using TMPro;
using UnityEngine;
namespace CosmicBlock.UI {
 public sealed class PopupTypography : MonoBehaviour {
  [SerializeField] TMP_FontAsset bodyFont,emphasisFont;
  [SerializeField] TextMeshProUGUI[] bodyLabels=new TextMeshProUGUI[0];
  public void Configure(TMP_FontAsset body,TMP_FontAsset emphasis,TextMeshProUGUI[] labels=null){bodyFont=body;emphasisFont=emphasis;bodyLabels=labels??new TextMeshProUGUI[0];}
  public void Apply(float scale){foreach(var text in GetComponentsInChildren<TextMeshProUGUI>(true)){bool title=text.name=="TitleText",body=text.name=="DescriptionText"||System.Array.IndexOf(bodyLabels,text)>=0;text.font=body?bodyFont:emphasisFont;text.enableAutoSizing=false;text.fontStyle=FontStyles.Normal;text.fontSize=(title?26:body?18:21)*scale;text.characterSpacing=0;text.lineSpacing=4;text.alignment=TextAlignmentOptions.Center;}}
 }
}
