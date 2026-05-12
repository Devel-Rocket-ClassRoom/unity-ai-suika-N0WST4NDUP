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

        private LineRenderer _lineRenderer;
        private readonly Dictionary<Fruit, float> _lingerTimers = new();
        private bool _gameOver;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            SetupLine();
            SetupTrigger();
        }

        private void Update()
        {
            if (_gameOver) return;

            var toRemove = new List<Fruit>();
            foreach (var kv in _lingerTimers)
            {
                var fruit = kv.Key;
                if (fruit == null) { toRemove.Add(fruit); continue; }
                if (fruit.IsInGrace) continue;

                _lingerTimers[fruit] = kv.Value + Time.deltaTime;
                if (_lingerTimers[fruit] >= lingerThreshold)
                {
                    TriggerGameOver();
                    return;
                }
            }
            foreach (var f in toRemove) _lingerTimers.Remove(f);
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

            // 임시 UI — Task 1.7 GameManager로 교체 예정
            var canvas = new GameObject("GameOverCanvas").AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvas.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            var textGO = new GameObject("GameOverText");
            textGO.transform.SetParent(canvas.transform, false);
            var text = textGO.AddComponent<UnityEngine.UI.Text>();
            text.text      = "GAME OVER";
            text.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize  = 72;
            text.color     = Color.red;
            text.alignment = TextAnchor.MiddleCenter;
            var rt = textGO.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;

            // Dropper 입력 차단
            var dropper = FindFirstObjectByType<Dropper>();
            if (dropper != null) dropper.enabled = false;
        }

        private void SetupLine()
        {
            _lineRenderer.positionCount = 2;
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.SetPosition(0, new Vector3(-boardHalfWidth, transform.position.y, 0f));
            _lineRenderer.SetPosition(1, new Vector3( boardHalfWidth, transform.position.y, 0f));
        }

        private void SetupTrigger()
        {
            // 라인 위 공간 전체를 감지하는 BoxCollider2D (trigger)
            var col         = gameObject.AddComponent<BoxCollider2D>();
            col.isTrigger   = true;
            col.size        = new Vector2(boardHalfWidth * 2f, 3f);
            col.offset      = new Vector2(0f, 1.5f); // 라인 위쪽으로 3유닛
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.8f);
            Gizmos.DrawLine(
                new Vector3(-boardHalfWidth, transform.position.y, 0f),
                new Vector3( boardHalfWidth, transform.position.y, 0f)
            );
        }
    }
}
