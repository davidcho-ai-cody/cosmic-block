using UnityEngine;
namespace CosmicBlock.UI
{
    public sealed class HomeAmbientMotion : MonoBehaviour
    {
        [SerializeField] RectTransform planet;
        [SerializeField] RectTransform title;
        Vector3 planetBase,titleBase;
        public void Configure(RectTransform planetTarget,RectTransform titleTarget){planet=planetTarget;title=titleTarget;Cache();}
        public void SetPlanetBaseScale(Vector3 scale){planetBase=scale;}
        void Awake()=>Cache();
        void OnEnable()=>Cache();
        void Cache(){if(planet!=null)planetBase=planet.localScale;if(title!=null)titleBase=title.localScale;}
        void Update(){float t=Time.unscaledTime;if(planet!=null)planet.localScale=planetBase*(1f+Mathf.Sin(t*.85f)*.012f);if(title!=null)title.localScale=titleBase*(1f+Mathf.Sin(t*.65f+.8f)*.006f);}
        void OnDisable(){if(planet!=null)planet.localScale=planetBase;if(title!=null)title.localScale=titleBase;}
    }
}
