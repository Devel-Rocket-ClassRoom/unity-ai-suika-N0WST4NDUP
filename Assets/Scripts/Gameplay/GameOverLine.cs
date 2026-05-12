using System.Collections.Generic;
using UnityEngine;

namespace Watermelon.Gameplay
{
    [RequireComponent(typeof(LineRenderer))]
    public class GameOverLine : MonoBehaviour
    {
        public static event System.Action OnGameOver;

        [SerializeField] private float boardHalfWidth  = 2.4f;
        [SerializeField] private float lingerThreshold = 2f;

        [Header("Wave")]
        [SerializeField] private int   waveResolution = 60;
        [SerializeField] private float waveAmplitude  = 0.08f;
        [SerializeField] private float waveFrequency  = 3.5f;
        [SerializeField] private float waveSpeed      = 1.8f;

        private LineRenderer _lineRenderer;
        private readonly Dictionary<Fruit, float> _lingerTimers = new();
        private bool _gameOver;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            SetupLineRenderer();
            SetupTrigger();
        }

        private void Update()
        {
            UpdateWave();

            if (_gameOver) return;

            // 키 스냅샷으로 순회 — 루프 중 딕셔너리 수정 예외 방지
            var keys = new List<Fruit>(_lingerTimers.Keys);
            foreach (var fruit in keys)
            {
                if (fruit == null) { _lingerTimers.Remove(fruit); continue; }
                if (fruit.IsInGrace) continue;

                _lingerTimers[fruit] += Time.deltaTime;
                if (_lingerTimers[fruit] >= lingerThreshold)
                {
                    TriggerGameOver();
                    return;
                }
            }
        }

        private void UpdateWave()
        {
            float baseY = transform.position.y;
            float step  = (boardHalfWidth * 2f) / (waveResolution - 1);

            for (int i = 0; i < waveResolution; i++)
            {
                float x = -boardHalfWidth + step * i;
                float y = baseY + waveAmplitude * Mathf.Sin(waveFrequency * x + Time.time * waveSpeed);
                _lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var fruit = other.GetComponent<Fruit>();
            if (fruit != null && !_lingerTimers.ContainsKey(fruit))
                _lingerTimers[fruit] = 0f;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var fruit = other.GetComponent<Fruit>();
            if (fruit != null) _lingerTimers.Remove(fruit);
        }

        private void TriggerGameOver()
        {
            _gameOver = true;
            _lingerTimers.Clear();
            OnGameOver?.Invoke();
        }

        private void SetupLineRenderer()
        {
            _lineRenderer.positionCount = waveResolution;
            _lineRenderer.useWorldSpace = true;
            UpdateWave();
        }

        private void SetupTrigger()
        {
            var col       = gameObject.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size      = new Vector2(boardHalfWidth * 2f, 3f);
            col.offset    = new Vector2(0f, 1.5f);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.8f);
            float baseY = transform.position.y;
            float step  = (boardHalfWidth * 2f) / (waveResolution - 1);
            for (int i = 0; i < waveResolution - 1; i++)
            {
                float xA = -boardHalfWidth + step * i;
                float xB = -boardHalfWidth + step * (i + 1);
                float yA = baseY + waveAmplitude * Mathf.Sin(waveFrequency * xA);
                float yB = baseY + waveAmplitude * Mathf.Sin(waveFrequency * xB);
                Gizmos.DrawLine(new Vector3(xA, yA, 0f), new Vector3(xB, yB, 0f));
            }
        }
    }
}
