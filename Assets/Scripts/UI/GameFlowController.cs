using System.Globalization;
using CosmicBlock.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace CosmicBlock.UI
{
    public enum FlowScreen { Home, Game, Collection }

    public sealed class GameFlowController : MonoBehaviour
    {
        public static bool StartInGameForAutomation;

        [SerializeField] GameObject homeRoot;
        [SerializeField] GameObject[] gameRoots;
        [SerializeField] GameSession session;
        [SerializeField] Text homeBest;
        [SerializeField] Text homeJourney;
        [SerializeField] Button playButton;
        [SerializeField] Button gameOverHomeButton;
        [SerializeField] Button playingHomeButton;
        [SerializeField] Button homeExitButton;
        [SerializeField] GameObject homeConfirmPanel;
        [SerializeField] Button continueButton;
        [SerializeField] Button confirmHomeButton;
        [SerializeField] HomeViewController homeView;
        [SerializeField] PlanetCollectionView collectionView;
        public PlanetCollectionView CollectionView => collectionView;

        public FlowScreen Screen { get; private set; }
        public bool QuitRequested { get; private set; }
        public GameObject HomeRoot => homeRoot;
        public GameObject[] GameRoots => gameRoots;
        public bool HomeConfirmVisible => homeConfirmPanel != null && homeConfirmPanel.activeSelf;
        public HomeViewController HomeView => homeView;

        public void Configure(GameObject home, GameObject[] game, GameSession owner, Text best, Text journey, Button play, Button gameOverHome)
        {
            RemoveListeners(); homeRoot=home; gameRoots=game; session=owner; homeBest=best; homeJourney=journey;
            playButton=play; gameOverHomeButton=gameOverHome; AddListeners();
        }

        public void ConfigureNavigation(Button playingHome, Button exit, GameObject confirmPanel, Button keepPlaying, Button goHome)
        {
            RemoveListeners(); playingHomeButton=playingHome; homeExitButton=exit; homeConfirmPanel=confirmPanel;
            continueButton=keepPlaying; confirmHomeButton=goHome; AddListeners(); HideHomeConfirmation();
        }

        public void ConfigureHomeView(HomeViewController view){homeView=view;}

        public void ConfigureCollection(PlanetCollectionView view) => collectionView = view;
        public void ShowCollection()
        {
            if (Screen != FlowScreen.Home || collectionView == null || session == null) return;
            homeView?.HideTransient(); homeRoot.SetActive(false);
            collectionView.gameObject.SetActive(true); collectionView.Open(session.Restoration);
            Screen = FlowScreen.Collection;
        }
        public void ReturnFromCollection()
        {
            if (Screen != FlowScreen.Collection) return;
            collectionView.gameObject.SetActive(false); RefreshHome(); homeRoot.SetActive(true); Screen=FlowScreen.Home;
        }
        void Awake() => AddListeners();
        void Start() { if (StartInGameForAutomation) { StartInGameForAutomation=false; Play(); } else ShowHome(); }
        void Update() { if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) HandleBack(); }

        public void Play()
        {
            if (session == null) return;
            if (collectionView != null) collectionView.gameObject.SetActive(false);
            HideHomeConfirmation();
            session.ResetTransientFeedback();
            homeRoot.SetActive(false); SetGameVisible(true); session.Retry(); Screen=FlowScreen.Game; QuitRequested=false;
        }

        public void ShowHome()
        {
            if (collectionView != null) collectionView.gameObject.SetActive(false);
            HideHomeConfirmation(); homeView?.HideTransient();
            if (session != null) { session.ResetTransientFeedback(); session.Retry(); }
            SetGameVisible(false); RefreshHome(); homeRoot.SetActive(true); Screen=FlowScreen.Home;
        }

        public void RequestHome()
        {
            if (Screen != FlowScreen.Game || homeConfirmPanel == null) return;
            homeConfirmPanel.SetActive(true);
        }

        public void CancelHome() => HideHomeConfirmation();

        public void ConfirmHome()
        {
            if (!HomeConfirmVisible) return;
            HideHomeConfirmation(); ShowHome();
        }

        public void RequestQuit()
        {
            if (Screen != FlowScreen.Home) return;
            QuitRequested=true;
#if !UNITY_EDITOR
            Application.Quit();
#endif
        }

        public void HandleBack()
        {
            if (Screen == FlowScreen.Collection) { ReturnFromCollection(); return; }
            if (Screen == FlowScreen.Home)
            {
                RequestQuit();
            }
            else if (session != null && session.State == GameState.GameOver) ShowHome();
        }

        public void RefreshHome()
        {
            if(session==null)return; int best=session.BestScore;
            if(homeView!=null){homeView.Refresh(session);return;}
            if(homeBest!=null)homeBest.text="최고 점수  "+best.ToString("N0",CultureInfo.InvariantCulture);
            if(homeJourney!=null)homeJourney.text=session.Restoration.IsRestored?"푸른 별 · 복원 완료 ✓":"푸른 별 · 복원 단계 "+session.Restoration.Stage+" / 5";
        }

        void SetGameVisible(bool value)
        {
            if (gameRoots == null) return;
            foreach (var root in gameRoots) if (root != null)
            {
                root.SetActive(true);
                var group = root.GetComponent<CanvasGroup>();
                if (group == null) group = root.AddComponent<CanvasGroup>();
                group.alpha = value ? 1f : 0f;
                group.interactable = value;
                group.blocksRaycasts = value;
            }
        }
        void AddListeners()
        {
            if(playButton!=null){playButton.onClick.RemoveListener(Play);playButton.onClick.AddListener(Play);}
            if(gameOverHomeButton!=null){gameOverHomeButton.onClick.RemoveListener(ShowHome);gameOverHomeButton.onClick.AddListener(ShowHome);}
            if(playingHomeButton!=null){playingHomeButton.onClick.RemoveListener(RequestHome);playingHomeButton.onClick.AddListener(RequestHome);}
            if(homeExitButton!=null){homeExitButton.onClick.RemoveListener(RequestQuit);homeExitButton.onClick.AddListener(RequestQuit);}
            if(continueButton!=null){continueButton.onClick.RemoveListener(CancelHome);continueButton.onClick.AddListener(CancelHome);}
            if(confirmHomeButton!=null){confirmHomeButton.onClick.RemoveListener(ConfirmHome);confirmHomeButton.onClick.AddListener(ConfirmHome);}
        }
        void RemoveListeners()
        {
            if(playButton!=null)playButton.onClick.RemoveListener(Play);
            if(gameOverHomeButton!=null)gameOverHomeButton.onClick.RemoveListener(ShowHome);
            if(playingHomeButton!=null)playingHomeButton.onClick.RemoveListener(RequestHome);
            if(homeExitButton!=null)homeExitButton.onClick.RemoveListener(RequestQuit);
            if(continueButton!=null)continueButton.onClick.RemoveListener(CancelHome);
            if(confirmHomeButton!=null)confirmHomeButton.onClick.RemoveListener(ConfirmHome);
        }
        void HideHomeConfirmation(){if(homeConfirmPanel!=null)homeConfirmPanel.SetActive(false);}
        void OnDestroy()=>RemoveListeners();
    }
}
