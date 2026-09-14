using System.Globalization;
using CosmicBlock.Core;
using UnityEngine;
using UnityEngine.UI;

namespace CosmicBlock.UI
{
    public sealed class GameHud : MonoBehaviour
    {
        [SerializeField] private Text scoreText;
        [SerializeField] private Text comboText;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Text finalScoreText;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button adPlaceholderButton;
        private GameSession session;

        public void Configure(Text score, Text combo, GameObject panel, Text finalScore, Button retry, Button ad)
        { scoreText = score; comboText = combo; gameOverPanel = panel; finalScoreText = finalScore; retryButton = retry; adPlaceholderButton = ad; }
        public void Connect(GameSession owner)
        {
            if (session != null) retryButton.onClick.RemoveListener(session.Retry);
            session = owner;
            retryButton.onClick.AddListener(session.Retry);
            adPlaceholderButton.interactable = false;
        }
        public void Render(GameSession owner)
        {
            string score = owner.Score.ToString("N0", CultureInfo.InvariantCulture);
            string best = owner.BestScore.ToString("N0", CultureInfo.InvariantCulture);
            scoreText.text = "BEST  " + best + "     SCORE  " + score;
            comboText.text = owner.Combo == 0 ? "" : owner.Combo < 3 ? "COMBO " + owner.Combo :
                owner.Combo == 3 ? "STAR COMBO 3" : "COSMIC COMBO " + owner.Combo;
            finalScoreText.text = "SCORE\n" + score + "\n\nBEST\n" + best;
            gameOverPanel.SetActive(owner.State == GameState.GameOver);
        }
        private void OnDestroy()
        { if (session != null && retryButton != null) retryButton.onClick.RemoveListener(session.Retry); }
    }
}
