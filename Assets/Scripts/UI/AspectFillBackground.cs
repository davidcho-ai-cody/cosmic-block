using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI {
 [ExecuteAlways,RequireComponent(typeof(RectTransform),typeof(Image))]
 public sealed class AspectFillBackground:MonoBehaviour {
  [SerializeField] Vector2 referenceSize=new Vector2(941,1672);
  void OnEnable()=>Apply();
  void LateUpdate()=>Apply();
  void Apply(){
   var rect=(RectTransform)transform;var parent=rect.parent as RectTransform;if(parent==null||referenceSize.x<=0||referenceSize.y<=0)return;
   float scale=Mathf.Max(parent.rect.width/referenceSize.x,parent.rect.height/referenceSize.y);
   rect.anchorMin=rect.anchorMax=new Vector2(.5f,.5f);rect.pivot=new Vector2(.5f,.5f);
   rect.sizeDelta=referenceSize*scale;rect.anchoredPosition=Vector2.zero;
  }
 }
}