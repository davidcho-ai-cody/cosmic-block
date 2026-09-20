using System.Globalization;
using CosmicBlock.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace CosmicBlock.UI
{
    public enum FlowScreen { Home, Game }

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

        public FlowScreen Screen { get; private set; }
        public bool QuitRequested { get; private set; }
        public GameObject HomeRoot => homeRoot;
        public GameObject[] GameRoots => gameRoots;

        public void Configure(GameObject home, GameObject[] game, GameSession owner, Text best, Text journey, Button play, Button gameOverHome)
        {
            RemoveListeners(); homeRoot=home; gameRoots=game; session=owner; homeBest=best; homeJourney=journey;
            playButton=play; gameOverHomeButton=gameOverHome; AddListeners();
        }

        void Awake() => AddListeners();
        void Start() { if (StartInGameForAutomation) { StartInGameForAutomation=false; Play(); } else ShowHome(); }
        void Update() { if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) HandleBack(); }

        public void Play()
        {
            if (session == null) return;
            session.ResetTransientFeedback(); session.Retry();
            homeRoot.SetActive(false); SetGameVisible(true); Screen=FlowScreen.Game; QuitRequested=false;
        }

        public void ShowHome()
        {
            if (session != null) { session.ResetTransientFeedback(); session.Retry(); }
            SetGameVisible(false); RefreshHome(); homeRoot.SetActive(true); Screen=FlowScreen.Home;
        }

        public void HandleBack()
        {
            if (Screen == FlowScreen.Home)
            {
                QuitRequested=true;
#if !UNITY_EDITOR
                Application.Quit();
#endif
            }
            else if (session != null && session.State == GameState.GameOver) ShowHome();
        }

        public void RefreshHome()
        {
            if(session==null)return; int best=session.BestScore;
            homeBest.text="BEST  "+best.ToString("N0",CultureInfo.InvariantCulture);
            homeJourney.text="BEST JOURNEY\n"+JourneyProgress.At(best).Current.Name;
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
        }
        void RemoveListeners()
        {
            if(playButton!=null)playButton.onClick.RemoveListener(Play);
            if(gameOverHomeButton!=null)gameOverHomeButton.onClick.RemoveListener(ShowHome);
        }
        void OnDestroy()=>RemoveListeners();
    }
}
