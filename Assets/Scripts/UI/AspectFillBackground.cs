using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI {
 [ExecuteAlways,RequireComponent(typeof(RectTransform),typeof(Image))]
 public sealed class AspectFillBackground:MonoBehaviour {
  [SerializeField] Vector2 referenceSize=new Vector2(941,1672);
  [SerializeField] bool includeUnsafeArea;
  void OnEnable()=>Apply();
  void LateUpdate()=>Apply();
  void Apply(){
   var rect=(RectTransform)transform;var parent=rect.parent as RectTransform;if(parent==null||referenceSize.x<=0||referenceSize.y<=0)return;
   Vector2 targetSize=parent.rect.size;Vector2 targetPosition=Vector2.zero;
   if(includeUnsafeArea){
    var canvas=GetComponentInParent<Canvas>();float canvasScale=canvas==null?1:Mathf.Max(.0001f,canvas.scaleFactor);
    targetSize=new Vector2(Screen.width/canvasScale,Screen.height/canvasScale);
    Vector2 fullCenter=new Vector2(Screen.width*.5f,Screen.height*.5f);
    targetPosition=(fullCenter-Screen.safeArea.center)/canvasScale;
   }
   float scale=Mathf.Max(targetSize.x/referenceSize.x,targetSize.y/referenceSize.y);
   rect.anchorMin=rect.anchorMax=new Vector2(.5f,.5f);rect.pivot=new Vector2(.5f,.5f);
   rect.sizeDelta=referenceSize*scale;rect.anchoredPosition=targetPosition;
  }
 }
}
