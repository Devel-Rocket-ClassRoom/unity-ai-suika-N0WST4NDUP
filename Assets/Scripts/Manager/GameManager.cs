using UnityEngine;
using UnityEngine.UI;
using Watermelon.Data;
using Watermelon.Gameplay;

namespace Watermelon.Manager
{
    public class GameManager : MonoBehaviour
    {
        public enum GameState { Ready, Playing, GameOver }

        private const string BestScoreKey = "BestScore";

        public static GameManager Instance { get; private set; }
        public GameState State { get; private set; } = GameState.Ready;
        public int Score { get; private set; }
        public int BestScore { get; private set; }

        [Header("HUD")]
        [SerializeField] private Text scoreText;
        [SerializeField] private Text bestScoreText;

        [Header("Game Over Panel")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Text gameOverScoreText;
        [SerializeField] private Text gameOverBestText;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            BestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
        }

        private void OnEnable()
        {
            Fruit.OnMerge           += HandleMerge;
            GameOverLine.OnGameOver += HandleGameOver;
        }

        private void OnDisable()
        {
            Fruit.OnMerge           -= HandleMerge;
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
            RefreshHUD();

            if (gameOverPanel != null) gameOverPanel.SetActive(false);
        }

        private void HandleMerge(FruitStageData resultStage)
        {
            if (State != GameState.Playing || resultStage == null) return;

            Score += resultStage.MergeScore;
            RefreshHUD();
        }

        private void HandleGameOver()
        {
            if (State == GameState.GameOver) return;
            State = GameState.GameOver;

            if (Score > BestScore)
            {
                BestScore = Score;
                PlayerPrefs.SetInt(BestScoreKey, BestScore);
                PlayerPrefs.Save();
            }

            var dropper = FindFirstObjectByType<Dropper>();
            if (dropper != null) dropper.enabled = false;

            ShowGameOverUI();
        }

        private void RefreshHUD()
        {
            if (scoreText    != null) scoreText.text    = Score.ToString("N0");
            if (bestScoreText != null) bestScoreText.text = BestScore.ToString("N0");
        }

        private void ShowGameOverUI()
        {
            if (gameOverPanel == null) return;
            gameOverPanel.SetActive(true);
            if (gameOverScoreText != null) gameOverScoreText.text = $"점수  {Score:N0}";
            if (gameOverBestText  != null) gameOverBestText.text  = $"최고  {BestScore:N0}";
        }
    }
}
