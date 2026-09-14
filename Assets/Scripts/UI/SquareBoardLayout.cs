using UnityEngine;
namespace CosmicBlock.UI {
 [ExecuteAlways]
 public sealed class SquareBoardLayout:MonoBehaviour {
  private void LateUpdate() {
   var parent=transform.parent as RectTransform; if(parent==null)return;
   float side=Mathf.Max(0,Mathf.Min(parent.rect.width-48,parent.rect.height*.58f));
   ((RectTransform)transform).sizeDelta=new Vector2(side,side);
  }
 }
}
