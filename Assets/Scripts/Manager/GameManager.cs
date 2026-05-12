using UnityEngine;
using UnityEngine.UI;
using Watermelon.Data;
using Watermelon.Gameplay;

namespace Watermelon.Manager
{
    public class GameManager : MonoBehaviour
    {
        public enum GameState { Ready, Playing, GameOver }

        public static GameManager Instance { get; private set; }
        public GameState State { get; private set; } = GameState.Ready;
        public int Score { get; private set; }

        [Header("UI")]
        [SerializeField] private Text scoreText;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Text gameOverScoreText;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void OnEnable()
        {
            Fruit.OnMerge      += HandleMerge;
            GameOverLine.OnGameOver += HandleGameOver;
        }

        private void OnDisable()
        {
            Fruit.OnMerge      -= HandleMerge;
            GameOverLine.OnGameOver -= HandleGameOver;
        }

        private void Start()
        {
            StartGame();
        }

        public void StartGame()
        {
            Score = 0;
            State = GameState.Playing;
            RefreshScoreUI();

            if (gameOverPanel != null) gameOverPanel.SetActive(false);
        }

        private void HandleMerge(FruitStageData resultStage)
        {
            if (State != GameState.Playing) return;
            if (resultStage == null) return;

            Score += resultStage.MergeScore;
            RefreshScoreUI();
        }

        private void HandleGameOver()
        {
            if (State == GameState.GameOver) return;
            State = GameState.GameOver;

            var dropper = FindFirstObjectByType<Dropper>();
            if (dropper != null) dropper.enabled = false;

            ShowGameOverUI();
        }

        private void RefreshScoreUI()
        {
            if (scoreText != null)
                scoreText.text = Score.ToString("N0");
        }

        private void ShowGameOverUI()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                if (gameOverScoreText != null)
                    gameOverScoreText.text = $"Score: {Score:N0}";
            }
        }
    }
}
