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
        [SerializeField] Button back, previous, next, select;
        [SerializeField] Button[] slots;
        [SerializeField] Image[] thumbnails;
        [SerializeField] GameObject[] locks;
        [SerializeField] Text[] labels;
        PlanetRestoration restoration;Vector2 layoutSize;
        public int PlanetIndex {get;private set;}
        public int PreviewStage {get;private set;}
        public int CurrentStage => restoration==null?1:restoration.Stage;
        public Sprite HeroSprite => hero.sprite;
        public string Message => message.text;
        public string Status => status.text;
        public string Percentage => percentage.text;
        public float Progress => progress.rectTransform.anchorMax.x;
        public Button[] Slots => slots;
        public bool IsStageLocked(int stage) => !PlanetDefinitions.IsUnlocked(PlanetIndex+1)||!PlanetDefinitions.Get(PlanetIndex+1).ContentReady||stage>CurrentStage||(stage==5&&PlanetDefinitions.Get(PlanetIndex+1).CompletionSpriteOnly&&!restoration.IsRestored);
        public bool CanSelect=>PlanetDefinitions.IsUnlocked(PlanetIndex+1)&&PlanetDefinitions.Get(PlanetIndex+1).ContentReady;
        public bool Choose()=>flow.SelectCollectionPlanet(PlanetIndex+1);
        public void ConfigureSelection(Button button){select=button;}
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
            if(select!=null)select.onClick.AddListener(()=>Choose());back.onClick.AddListener(GoBack);previous.onClick.AddListener(Previous);next.onClick.AddListener(Next);
            for(int i=0;i<slots.Length;i++){int stage=i+1;slots[i].onClick.AddListener(()=>Preview(stage));}
        }
        public void Open(PlanetRestoration source){restoration=source;PlanetIndex=source.PlanetId-1;PreviewStage=CurrentStage;Refresh();}
        public void Previous()=>SelectPlanet(PlanetIndex-1);
        public void Next()=>SelectPlanet(PlanetIndex+1);
        public void SelectPlanet(int index){PlanetIndex=Mathf.Clamp(index,0,4);int id=PlanetIndex+1;restoration=new PlanetRestoration(PlanetDefinitions.Get(id).EnergyKey,PlanetRestoration.VersionKey,id);PreviewStage=CurrentStage;Refresh();}
        public void Preview(int stage){if(stage<1||stage>5||IsStageLocked(stage))return;PreviewStage=stage;Refresh();}
        public void GoBack()=>flow.ReturnFromCollection();
        void Refresh()
        {
            layoutSize=((RectTransform)transform).rect.size;int id=PlanetIndex+1;var definition=PlanetDefinitions.Get(id);bool known=CanSelect;
            planetNumber.text="행성 "+id.ToString("00");planetName.text=known?definition.Name:"???";
            hero.gameObject.SetActive(true);heroLock.SetActive(!PlanetDefinitions.IsUnlocked(id));
            if(PlanetArtCatalog.Current!=null){if(known)PlanetArtCatalog.Current.Apply(hero,id,definition.CompletionSpriteOnly&&PreviewStage==5&&!restoration.IsRestored?4:PreviewStage);else {hero.sprite=PlanetArtCatalog.Current.lockedPlanet;hero.transform.localScale=Vector3.one;hero.rectTransform.anchoredPosition=Vector2.zero;}}
            else if(known)hero.sprite=sprites[PreviewStage-1];
            if(known&&id!=1)FitHero(id);
            message.text=known?(id==1?(PreviewStage==5&&!restoration.IsRestored?PlanetCollectionData.FinalProgressMessage:PlanetCollectionData.StageMessages[PreviewStage-1]):definition.Name+" · "+(restoration.IsRestored?"복원 완료":definition.StageLabel(PreviewStage,restoration.CurrentEnergy))+"\n별빛 "+restoration.CurrentEnergy+" / "+restoration.Total):PlanetDefinitions.IsUnlocked(id)?"콘텐츠 준비 중입니다.":"이전 행성을 완성하면 해금됩니다.";
            status.text=known?(restoration.IsRestored?"복원 완료":CurrentStage+"단계 · "+restoration.StageName+" · 별빛 "+restoration.CurrentEnergy+" / "+restoration.Total):PlanetDefinitions.IsUnlocked(id)?"콘텐츠 준비 중":"아직 잠겨 있습니다.";status.gameObject.SetActive(true);
            if(select!=null){select.interactable=known;select.GetComponentInChildren<Text>().text=known?(flow.HasSavedRun?"이 행성에서 이어하기":PlanetSelection.Current==id?"선택됨 · 홈으로":"이 행성 선택"):"선택 불가";}
            percentage.text=restoration.Percent+"%";progressFrame.SetActive(known);
            progress.transform.parent.gameObject.SetActive(known);progress.rectTransform.anchorMax=new Vector2(known?restoration.OverallProgress:0,1);
            for(int i=0;i<5;i++)
            {
                bool unlocked=!IsStageLocked(i+1);slots[i].interactable=unlocked;
                thumbnails[i].gameObject.SetActive(known);if(known&&PlanetArtCatalog.Current!=null)PlanetArtCatalog.Current.Apply(thumbnails[i],id,i==4&&definition.CompletionSpriteOnly&&!restoration.IsRestored?4:i+1);else thumbnails[i].sprite=sprites[i];thumbnails[i].color=new Color(1,1,1,unlocked?1:.3f);
                locks[i].SetActive(!unlocked);var lockRect=(RectTransform)locks[i].transform;lockRect.anchorMin=known?new Vector2(.72f,.03f):new Vector2(.24f,.20f);lockRect.anchorMax=known?new Vector2(.98f,.29f):new Vector2(.76f,.76f);labels[i].text=known?definition.StageLabel(i+1,restoration.CurrentEnergy):"???";
                labels[i].color=known&&i+1==CurrentStage?new Color(.55f,.9f,1):unlocked?Color.white:new Color(.55f,.62f,.72f,.65f);slots[i].transform.parent.Find("StageNumber"+(i+1)).GetComponent<Text>().color=labels[i].color;
                slots[i].transform.Find("CurrentRing").gameObject.SetActive(known&&i+1==CurrentStage);slots[i].transform.localScale=Vector3.one;
            }
            previous.interactable=PlanetIndex>0;next.interactable=PlanetIndex<4;previous.transform.Find("Icon").GetComponent<Image>().color=new Color(1,1,1,previous.interactable?1:.35f);next.transform.Find("Icon").GetComponent<Image>().color=new Color(1,1,1,next.interactable?1:.35f);
        }
        void FitHero(int id)
        {
            var catalog=PlanetArtCatalog.Current;if(catalog==null)return;
            Canvas.ForceUpdateCanvases();var root=(RectTransform)transform;var r=hero.rectTransform;
            var top=(RectTransform)transform.Find("NameFrameArea/NameFrame");var bottom=status.rectTransform;
            var corners=new Vector3[4];top.GetWorldCorners(corners);float upper=root.InverseTransformPoint(corners[0]).y;bottom.GetWorldCorners(corners);float lower=root.InverseTransformPoint(corners[1]).y;
            float bound=catalog.Bound(id);
            float side=Mathf.Min(r.rect.width,r.rect.height);float fit=Mathf.Min(1,Mathf.Max(0,upper-lower-32)/(side*bound),root.rect.width*.70f/(side*bound));
            r.localScale*=fit;r.anchoredPosition*=fit;
            Vector2 anchor=root.rect.min+Vector2.Scale(root.rect.size,(r.anchorMin+r.anchorMax)*.5f);
            r.anchoredPosition+=new Vector2(root.rect.center.x,(upper+lower)*.5f)-anchor;
        }
        void Update(){if(restoration!=null&&((RectTransform)transform).rect.size!=layoutSize)Refresh();if(CanSelect&&restoration!=null)slots[CurrentStage-1].transform.localScale=Vector3.one*(1+.02f*(1+Mathf.Sin(Time.unscaledTime*2.5f)));}
        void OnDisable(){if(slots!=null)foreach(var slot in slots)if(slot!=null)slot.transform.localScale=Vector3.one;}
    }
}
