using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI
{
    // HOME presentation only: sizes are measured from the preserved PNG aspect ratios.
    public sealed class HomeFinalLayout : MonoBehaviour
    {
        [SerializeField] RectTransform home, actions, play, collection, bestArea, bottom;
        [SerializeField] float playVisibleBottom,collectionVisibleTop;
        public void Configure(RectTransform h, RectTransform a, RectTransform p, RectTransform c, RectTransform b, RectTransform utility, float playBottom, float collectionTop)
        { home=h;actions=a;play=p;collection=c;bestArea=b;bottom=utility;playVisibleBottom=playBottom;collectionVisibleTop=collectionTop;Apply(); }
        void LateUpdate()=>Apply();
        public void Apply()
        {
            if(home==null||play==null||home.rect.width<=0)return;
            float unit=home.rect.width/1080f;
            var image=play.GetComponent<Image>();
            Vector2 shown=Fit(play.rect.size,image.sprite.rect.size);
            float playBottom=play.localPosition.y+play.rect.center.y+shown.y*(playVisibleBottom-.5f);
            float width=shown.x*.96f;
            float height=width*collection.GetComponent<Image>().sprite.rect.height/collection.GetComponent<Image>().sprite.rect.width;
            Place(collection,new Vector2(width,height),new Vector2(0,playBottom-32f*unit-height*(collectionVisibleTop-.5f)));
            var frame=bestArea.Find("Frame").GetComponent<Image>();
            float bestWidth=shown.x*.98f;
            float bestHeight=bestWidth*frame.sprite.rect.height/frame.sprite.rect.width;
            Place(bestArea,new Vector2(bestWidth,bestHeight),new Vector2(0,actions.localPosition.y+collection.localPosition.y-height*.5f-16f*unit-bestHeight*.5f));
            // Raise both utility actions by 100 reference pixels and enlarge their shared hit areas.
            float utilityHeight=home.rect.height*.085f;float utilityY=Mathf.Min(-home.rect.height*.5f+home.rect.height*.0575f+100f*unit,bestArea.localPosition.y-bestHeight*.5f-24f*unit-utilityHeight*.5f);Place(bottom,new Vector2(home.rect.width*.90f,utilityHeight),new Vector2(0,utilityY));
        }
        static Vector2 Fit(Vector2 box,Vector2 sprite){float scale=Mathf.Min(box.x/sprite.x,box.y/sprite.y);return sprite*scale;}
        static void Place(RectTransform r,Vector2 size,Vector2 position){r.anchorMin=r.anchorMax=r.pivot=new Vector2(.5f,.5f);r.sizeDelta=size;r.anchoredPosition=position;}
    }
}
