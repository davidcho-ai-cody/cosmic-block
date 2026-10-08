using System.Collections;
using System.Globalization;
using CosmicBlock.Core;
using UnityEngine;
using UnityEngine.UI;

namespace CosmicBlock.UI
{
    public sealed class HomeViewController : MonoBehaviour
    {
        [SerializeField] GameFlowController flow;
        [SerializeField] Text bestScoreText;
        [SerializeField] Text selectedPlanetInfo;
        GameSession displayedSession;Vector2 layoutSize;
        [SerializeField] Image planetImage;
        [SerializeField] Text planetNameText;
        [SerializeField] Text stageText;
        [SerializeField] Text energyText;
        [SerializeField] Text energyPercentText;
        [SerializeField] Image progressFill;
        [SerializeField] Sprite[] planetStages;
        [SerializeField] Button collectionButton;
        [SerializeField] Button settingsButton;
        [SerializeField] Button settingsCloseButton;
        [SerializeField] Button quitButton;
        [SerializeField] Button quitCancelButton;
        [SerializeField] Button quitConfirmButton;
        [SerializeField] GameObject toastRoot;
        [SerializeField] GameObject settingsPanel;
        [SerializeField] GameObject quitPanel;
        Coroutine toastRoutine;

        public bool ToastVisible => toastRoot != null && toastRoot.activeSelf;
        public bool SettingsVisible => settingsPanel != null && settingsPanel.activeSelf;
        public bool QuitVisible => quitPanel != null && quitPanel.activeSelf;
        public float ProgressFill => progressFill == null ? 0 : progressFill.rectTransform.anchorMax.x;
        public Sprite PlanetSprite => planetImage == null ? null : planetImage.sprite;
        public string BestText => bestScoreText == null ? string.Empty : bestScoreText.text;
        public string StageText => stageText == null ? string.Empty : stageText.text;
        public string EnergyText => energyText == null ? string.Empty : energyText.text;
        public string EnergyPercentText => energyPercentText == null ? string.Empty : energyPercentText.text;

        public void Configure(GameFlowController controller, Text best, Image planet, Text planetName, Text stage, Text energy, Image fill, Sprite[] sprites,
            Button collection, Button settings, Button settingsClose, Button quit, Button quitCancel, Button quitConfirm,
            GameObject toast, GameObject settingsRoot, GameObject quitRoot)
        {
            RemoveListeners();
            flow=controller;bestScoreText=best;planetImage=planet;planetNameText=planetName;stageText=stage;energyText=energy;progressFill=fill;planetStages=sprites;
            collectionButton=collection;settingsButton=settings;settingsCloseButton=settingsClose;quitButton=quit;quitCancelButton=quitCancel;quitConfirmButton=quitConfirm;
            toastRoot=toast;settingsPanel=settingsRoot;quitPanel=quitRoot;
            AddListeners();HideTransient();
        }

        public void Configure(GameFlowController controller, Text best, Image planet, Text planetName, Text stage, Text energy, Text percent, Image fill, Sprite[] sprites,
            Button collection, Button settings, Button settingsClose, Button quit, Button quitCancel, Button quitConfirm,
            GameObject toast, GameObject settingsRoot, GameObject quitRoot)
        {
            Configure(controller,best,planet,planetName,stage,energy,fill,sprites,collection,settings,settingsClose,quit,quitCancel,quitConfirm,toast,settingsRoot,quitRoot);
            energyPercentText=percent;
        }

        void Awake(){AddListeners();}

        public void Refresh(GameSession session)
        {
            if(session==null)return;displayedSession=session;layoutSize=((RectTransform)transform).rect.size;
            int stage=Mathf.Clamp(session.Restoration.Stage,1,5);
            int stageEnergy=session.Restoration.StageEnergy;
            if(bestScoreText!=null)bestScoreText.text=session.BestScore.ToString("N0",CultureInfo.InvariantCulture);
            if(planetNameText!=null)planetNameText.text="행성 "+session.Restoration.PlanetId.ToString("00")+" · "+session.Restoration.Definition.Name;
            if(stageText!=null)stageText.text=stage+"단계 · "+session.Restoration.StageName;
            if(energyText!=null)energyText.text="별빛 에너지     "+stageEnergy+" / "+session.Restoration.StageRequired;
            if(energyPercentText!=null)energyPercentText.text=session.Restoration.Percent+"%";
            var homeHero=planetImage!=null?planetImage:transform.Find("HeroArea/HeroPlanet")?.GetComponent<Image>();if(homeHero!=null&&PlanetArtCatalog.Current!=null){if(session.Restoration.PlanetId==1){homeHero.sprite=PlanetArtCatalog.Current.homeBrand;homeHero.transform.localScale=Vector3.one;}else {PlanetArtCatalog.Current.Apply(homeHero,session.Restoration.PlanetId,session.Restoration.SpriteStage);float bound=PlanetArtCatalog.Current.Bound(session.Restoration.PlanetId)/.75f;homeHero.transform.localScale/=bound;homeHero.rectTransform.anchoredPosition/=bound;}transform.GetComponent<HomeAmbientMotion>()?.SetPlanetBaseScale(homeHero.transform.localScale);}else if(planetImage!=null&&planetStages!=null&&planetStages.Length>=stage)planetImage.sprite=planetStages[stage-1];
            if(selectedPlanetInfo!=null){selectedPlanetInfo.gameObject.SetActive(session.Restoration.PlanetId!=1);selectedPlanetInfo.text="행성 "+session.Restoration.PlanetId.ToString("00")+" · "+session.Restoration.Definition.Name+"\n별빛 "+session.Restoration.CurrentEnergy+" / "+session.Restoration.Total+" · 복원도 "+session.Restoration.Percent+"%";}
            SetProgress(session.Restoration.OverallProgress);
        }

        void LateUpdate(){if(displayedSession!=null&&((RectTransform)transform).rect.size!=layoutSize)Refresh(displayedSession);}
        static string KoreanStageName(int stage)
        {
            return PlanetRestoration.NameForStage(stage);
        }

        public void ShowCollectionNotice()
        {
            flow?.ShowCollection();
        }

        public void ShowSettings(){settingsPanel.SetActive(true);}
        public void CloseSettings(){settingsPanel.SetActive(false);}
        public void ShowQuit(){quitPanel.SetActive(true);}
        public void CancelQuit(){quitPanel.SetActive(false);}
        public void ConfirmQuit(){quitPanel.SetActive(false);flow?.RequestQuit();}

        public void HideTransient()
        {
            if(toastRoutine!=null)StopCoroutine(toastRoutine);toastRoutine=null;
            if(toastRoot!=null)toastRoot.SetActive(false);
            if(settingsPanel!=null)settingsPanel.SetActive(false);
            if(quitPanel!=null)quitPanel.SetActive(false);
        }

        IEnumerator HideToast(){yield return new WaitForSecondsRealtime(1.2f);toastRoot.SetActive(false);toastRoutine=null;}
        void SetProgress(float value){if(progressFill==null)return;value=Mathf.Clamp01(value);progressFill.fillAmount=value;var rect=progressFill.rectTransform;rect.anchorMin=Vector2.zero;rect.anchorMax=new Vector2(value,1);rect.offsetMin=rect.offsetMax=Vector2.zero;}
        void AddListeners()
        {
            if(collectionButton!=null)collectionButton.onClick.AddListener(ShowCollectionNotice);
            if(settingsButton!=null)settingsButton.onClick.AddListener(ShowSettings);
            if(settingsCloseButton!=null)settingsCloseButton.onClick.AddListener(CloseSettings);
            if(quitButton!=null)quitButton.onClick.AddListener(ShowQuit);
            if(quitCancelButton!=null)quitCancelButton.onClick.AddListener(CancelQuit);
            if(quitConfirmButton!=null)quitConfirmButton.onClick.AddListener(ConfirmQuit);
        }
        void RemoveListeners()
        {
            if(collectionButton!=null)collectionButton.onClick.RemoveListener(ShowCollectionNotice);
            if(settingsButton!=null)settingsButton.onClick.RemoveListener(ShowSettings);
            if(settingsCloseButton!=null)settingsCloseButton.onClick.RemoveListener(CloseSettings);
            if(quitButton!=null)quitButton.onClick.RemoveListener(ShowQuit);
            if(quitCancelButton!=null)quitCancelButton.onClick.RemoveListener(CancelQuit);
            if(quitConfirmButton!=null)quitConfirmButton.onClick.RemoveListener(ConfirmQuit);
        }
        void OnDisable()=>HideTransient();
        void OnDestroy()=>RemoveListeners();
    }
}

