using UnityEngine;
namespace CosmicBlock.UI {
 public sealed class HomeQuitPopupLayout : MonoBehaviour {
  void LateUpdate(){Apply();}
  public void Apply(){var r=(RectTransform)transform;var area=(RectTransform)r.parent;float width=Mathf.Min(area.rect.width*.90f,area.rect.height*.72f*1.5f);r.anchorMin=r.anchorMax=r.pivot=Vector2.one*.5f;r.anchoredPosition=Vector2.zero;r.sizeDelta=new Vector2(width,width/1.5f);var dim=(RectTransform)area.Find("DimOverlay");var canvas=(RectTransform)GetComponentInParent<Canvas>().transform;if(dim!=null){var min=area.InverseTransformPoint(canvas.TransformPoint(canvas.rect.min));var max=area.InverseTransformPoint(canvas.TransformPoint(canvas.rect.max));dim.anchorMin=dim.anchorMax=dim.pivot=Vector2.one*.5f;dim.sizeDelta=new Vector2(max.x-min.x,max.y-min.y);dim.localPosition=(min+max)*.5f;}}
 }
}
