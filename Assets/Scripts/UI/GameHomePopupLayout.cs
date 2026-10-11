using UnityEngine;
namespace CosmicBlock.UI {
 public sealed class GameHomePopupLayout : MonoBehaviour {
  void LateUpdate(){Apply();}
  public void Apply(){var r=(RectTransform)transform;var area=(RectTransform)r.parent;float width=Mathf.Min(area.rect.width*.82f,area.rect.height*.60f/.64f);r.anchorMin=r.anchorMax=r.pivot=Vector2.one*.5f;r.anchoredPosition=Vector2.zero;r.sizeDelta=new Vector2(width,width*.64f);float q=width/440f;
   transform.Find("CosmicFrame").GetComponent<HomeQuitMeshGraphic>().Geometry(18*q,q);
   foreach(string name in new[]{"Continue","GoHome"})transform.Find(name).GetComponent<HomeQuitMeshGraphic>().Geometry(10*q,q);
   var dim=(RectTransform)area.Find("DimOverlay");var canvas=(RectTransform)GetComponentInParent<Canvas>().transform;if(dim!=null){var min=area.InverseTransformPoint(canvas.TransformPoint(canvas.rect.min));var max=area.InverseTransformPoint(canvas.TransformPoint(canvas.rect.max));dim.anchorMin=dim.anchorMax=dim.pivot=Vector2.one*.5f;dim.sizeDelta=new Vector2(max.x-min.x,max.y-min.y);dim.localPosition=(min+max)*.5f;}
  }
 }
}
