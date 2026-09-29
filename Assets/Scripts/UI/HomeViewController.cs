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

        void Awake(){AddListeners();}

        public void Refresh(GameSession session)
        {
            if(session==null)return;
            int stage=Mathf.Clamp(session.Restoration.Stage,1,5);
            bestScoreText.text="최고 점수  "+session.BestScore.ToString("N0",CultureInfo.InvariantCulture);
            planetNameText.text="푸른 별";
            stageText.text=session.Restoration.IsRestored?"복원 완료 ✓":"복원 단계 "+stage+" / 5";
            energyText.text=session.Restoration.IsRestored?"100%":session.Restoration.StageEnergy+"%";
            if(planetStages!=null&&planetStages.Length>=stage)planetImage.sprite=planetStages[stage-1];
            SetProgress(session.Restoration.IsRestored?1f:session.Restoration.StageEnergy/100f);
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

