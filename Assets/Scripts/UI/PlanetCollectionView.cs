using CosmicBlock.Core;
using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI
{
    public sealed class PlanetCollectionView : MonoBehaviour
    {
        [SerializeField] GameFlowController flow;
        [SerializeField] Sprite[] sprites;
        [SerializeField] Image hero, progress;
        [SerializeField] GameObject heroLock, progressFrame;
        [SerializeField] Text percentage;
        [SerializeField] Text planetNumber, planetName, status, message;
        [SerializeField] Button back, previous, next;
        [SerializeField] Button[] slots;
        [SerializeField] Image[] thumbnails;
        [SerializeField] GameObject[] locks;
        [SerializeField] Text[] labels;
        PlanetRestoration restoration;
        public int PlanetIndex {get;private set;}
        public int PreviewStage {get;private set;}
        public int CurrentStage => restoration==null?1:restoration.Stage;
        public Sprite HeroSprite => hero.sprite;
        public string Message => message.text;
        public string Status => status.text;
        public string Percentage => percentage.text;
        public float Progress => progress.rectTransform.anchorMax.x;
        public Button[] Slots => slots;
        public bool IsStageLocked(int stage) => PlanetIndex!=0||stage>CurrentStage;
        public void Configure(GameFlowController owner,Sprite[] stages,Image planet,Image fill,GameObject lockedHero,
            Text number,Text name,Text state,Text lore,Button goBack,Button prev,Button forward,
            Button[] buttons,Image[] images,GameObject[] lockIcons,Text[] names)
        {
            flow=owner;sprites=stages;hero=planet;progress=fill;heroLock=lockedHero;
            planetNumber=number;planetName=name;status=state;message=lore;
            back=goBack;previous=prev;next=forward;slots=buttons;thumbnails=images;locks=lockIcons;labels=names;
        }
        public void ConfigureProgress(GameObject frame,Text value){progressFrame=frame;percentage=value;}
        void Awake()
        {
            back.onClick.AddListener(GoBack);previous.onClick.AddListener(Previous);next.onClick.AddListener(Next);
            for(int i=0;i<slots.Length;i++){int stage=i+1;slots[i].onClick.AddListener(()=>Preview(stage));}
        }
        public void Open(PlanetRestoration source){restoration=source;PlanetIndex=0;PreviewStage=CurrentStage;Refresh();}
        public void Previous()=>SelectPlanet(PlanetIndex-1);
        public void Next()=>SelectPlanet(PlanetIndex+1);
        public void SelectPlanet(int index){PlanetIndex=Mathf.Clamp(index,0,4);PreviewStage=CurrentStage;Refresh();}
        public void Preview(int stage){if(stage<1||stage>5||IsStageLocked(stage))return;PreviewStage=stage;Refresh();}
        public void GoBack()=>flow.ReturnFromCollection();
        void Refresh()
        {
            bool known=PlanetIndex==0;bool discovered=PlanetIndex==1&&restoration.IsPlanet02Unlocked;
            planetNumber.text="행성 "+(PlanetIndex+1).ToString("00");planetName.text=PlanetCollectionData.PlanetNames[PlanetIndex];
            hero.gameObject.SetActive(known);heroLock.SetActive(!known&&!discovered);if(known)hero.sprite=sprites[PreviewStage-1];
            message.text=known?(PreviewStage==5&&!restoration.IsRestored?PlanetCollectionData.FinalProgressMessage:PlanetCollectionData.StageMessages[PreviewStage-1]):discovered?PlanetCollectionData.DiscoveredMessage:PlanetCollectionData.LockedMessage;
            status.text=known?"":PlanetIndex==1&&restoration.IsPlanet02Unlocked?"발견 완료 · 탐험 준비 중":"아직 잠겨 있습니다.";status.gameObject.SetActive(!known);
            percentage.text=restoration.Percent+"%";progressFrame.SetActive(known);
            progress.transform.parent.gameObject.SetActive(known);progress.rectTransform.anchorMax=new Vector2(known?restoration.OverallProgress:0,1);
            for(int i=0;i<5;i++)
            {
                bool unlocked=!IsStageLocked(i+1);slots[i].interactable=unlocked;
                thumbnails[i].gameObject.SetActive(known);thumbnails[i].sprite=sprites[i];thumbnails[i].color=new Color(1,1,1,unlocked?1:.3f);
                locks[i].SetActive(!unlocked);var lockRect=(RectTransform)locks[i].transform;lockRect.anchorMin=known?new Vector2(.72f,.03f):new Vector2(.24f,.20f);lockRect.anchorMax=known?new Vector2(.98f,.29f):new Vector2(.76f,.76f);labels[i].text=known?PlanetCollectionData.StageNames[i]:"???";
                labels[i].color=known&&i+1==CurrentStage?new Color(.55f,.9f,1):unlocked?Color.white:new Color(.55f,.62f,.72f,.65f);slots[i].transform.parent.Find("StageNumber"+(i+1)).GetComponent<Text>().color=labels[i].color;
                slots[i].transform.Find("CurrentRing").gameObject.SetActive(known&&i+1==CurrentStage);slots[i].transform.localScale=Vector3.one;
            }
            previous.interactable=PlanetIndex>0;next.interactable=PlanetIndex<4;previous.transform.Find("Icon").GetComponent<Image>().color=new Color(1,1,1,previous.interactable?1:.35f);next.transform.Find("Icon").GetComponent<Image>().color=new Color(1,1,1,next.interactable?1:.35f);
        }
        void Update(){if(PlanetIndex==0&&restoration!=null)slots[CurrentStage-1].transform.localScale=Vector3.one*(1+.02f*(1+Mathf.Sin(Time.unscaledTime*2.5f)));}
        void OnDisable(){if(slots!=null)foreach(var slot in slots)if(slot!=null)slot.transform.localScale=Vector3.one;}
    }
}
