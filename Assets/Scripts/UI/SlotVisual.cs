using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI {
 [RequireComponent(typeof(Image),typeof(Outline))]
 public sealed class SlotVisual:MonoBehaviour {
  public static readonly Color NormalFill=new Color(.035f,.07f,.16f,.72f);
  public static readonly Color NormalBorder=new Color(.28f,.70f,.88f,.62f);
  public static readonly Color SelectedBorder=new Color(1,.76f,.28f,.96f);
  Image image;Outline outline;
  void Awake(){image=GetComponent<Image>();outline=GetComponent<Outline>();SetSelected(false);}
  public void SetSelected(bool selected){if(image==null)image=GetComponent<Image>();if(outline==null)outline=GetComponent<Outline>();image.color=NormalFill;image.raycastTarget=false;outline.effectColor=selected?SelectedBorder:NormalBorder;outline.effectDistance=selected?new Vector2(3,-3):new Vector2(2,-2);outline.useGraphicAlpha=false;}
  public bool IsSelected=>outline!=null&&outline.effectColor==SelectedBorder;
 }
}