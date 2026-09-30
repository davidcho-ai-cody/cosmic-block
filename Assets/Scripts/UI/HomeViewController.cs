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
            if(session==null)return;
            int stage=Mathf.Clamp(session.Restoration.Stage,1,5);
            int stageEnergy=session.Restoration.StageEnergy;
            bestScoreText.text=session.BestScore.ToString("N0",CultureInfo.InvariantCulture);
            planetNameText.text="행성 01";
            stageText.text=stage+"단계 · "+KoreanStageName(stage);
            energyText.text="별빛 에너지     "+stageEnergy+" / "+PlanetRestoration.StageEnergyRequired;
            if(energyPercentText!=null)energyPercentText.text=stageEnergy+"%";
            if(planetStages!=null&&planetStages.Length>=stage)planetImage.sprite=planetStages[stage-1];
            SetProgress(stageEnergy/(float)PlanetRestoration.StageEnergyRequired);
        }

        static string KoreanStageName(int stage)
        {
            switch(Mathf.Clamp(stage,1,5)){case 1:return "황폐함";case 2:return "깨어남";case 3:return "회복 중";case 4:return "번성";default:return "복원 완료";}
        }

        public void ShowCollectionNotice()
        {
            if(toastRoutine!=null)StopCoroutine(toastRoutine);
            toastRoot.SetActive(true);toastRoutine=StartCoroutine(HideToast());
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
        void SetProgress(float value){value=Mathf.Clamp01(value);progressFill.fillAmount=value;var rect=progressFill.rectTransform;rect.anchorMin=Vector2.zero;rect.anchorMax=new Vector2(value,1);rect.offsetMin=rect.offsetMax=Vector2.zero;}
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

