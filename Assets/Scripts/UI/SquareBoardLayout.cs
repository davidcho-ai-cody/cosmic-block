using UnityEngine;
namespace CosmicBlock.UI {
 [ExecuteAlways]
 public sealed class SquareBoardLayout:MonoBehaviour {
  [SerializeField] bool gameVisualRebuild;
  [SerializeField] GameVisualPresentation matchedPresentation;
  public void ConfigureGameVisual(){gameVisualRebuild=true;matchedPresentation=null;}
  public void ConfigureLayoutMatch(GameVisualPresentation presentation)=>matchedPresentation=presentation;
  private void LateUpdate() {
   if(matchedPresentation!=null){matchedPresentation.Layout();return;}
   var parent=transform.parent as RectTransform; if(parent==null)return;
   float side=Mathf.Max(0,gameVisualRebuild?Mathf.Min(parent.rect.width*.66f,parent.rect.height*.36f):Mathf.Min(parent.rect.width-48,parent.rect.height*.58f));
   ((RectTransform)transform).sizeDelta=new Vector2(side,side);
  }
 }
}
