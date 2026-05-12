using UnityEngine;
using UnityEngine.InputSystem;
using Watermelon.Data;

namespace Watermelon.Gameplay
{
    public class Dropper : MonoBehaviour
    {
        [SerializeField] private FruitStageTable stageTable;
        [SerializeField] private GameObject fruitPrefab;         // Task 1.4에서 연결
        [SerializeField] private SpriteRenderer previewRenderer; // 현재 과일 미리보기 (자식 오브젝트)
        [SerializeField] private float dropCooldown = 0.5f;
        [SerializeField] private float boardHalfWidth = 3.0f;

        public FruitStageData CurrentStage { get; private set; }
        public FruitStageData NextStage { get; private set; }

        private enum InputMode { Mouse, Keyboard }
        private InputMode _inputMode = InputMode.Mouse;
        private Vector2 _lastMousePosition;

        private bool _canDrop = true;
        private float _cooldownTimer;
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
            if (Mouse.current != null)
                _lastMousePosition = Mouse.current.position.ReadValue();
        }

        private void Start()
        {
            if (previewRenderer != null && previewRenderer.sprite == null)
                previewRenderer.sprite = CreateCircleSprite();

            CurrentStage = PickRandom();
            NextStage = PickRandom();
            RefreshPreview();
        }

        private void Update()
        {
            TickCooldown();
            MoveWithInput();
            if (_canDrop) CheckDropInput();
        }

        private void TickCooldown()
        {
            if (_canDrop) return;
            _cooldownTimer -= Time.deltaTime;
            if (_cooldownTimer <= 0f)
            {
                _canDrop = true;
                CurrentStage = NextStage;
                NextStage = PickRandom();
                RefreshPreview();
            }
        }

        private void MoveWithInput()
        {
            if (Keyboard.current != null &&
                (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.rightArrowKey.isPressed))
            {
                _inputMode = InputMode.Keyboard;
            }
            else if (Mouse.current != null)
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();
                if (Vector2.Distance(mousePos, _lastMousePosition) > 1f)
                    _inputMode = InputMode.Mouse;
                _lastMousePosition = mousePos;
            }

            float targetX = transform.position.x;

            if (_inputMode == InputMode.Mouse && Mouse.current != null)
            {
                Vector3 world = _mainCamera.ScreenToWorldPoint(
                    new Vector3(Mouse.current.position.x.ReadValue(),
                                Mouse.current.position.y.ReadValue(), 0f));
                targetX = world.x;
            }
            else if (_inputMode == InputMode.Keyboard && Keyboard.current != null)
            {
                if (Keyboard.current.leftArrowKey.isPressed)
                    targetX -= 5f * Time.deltaTime;
                else if (Keyboard.current.rightArrowKey.isPressed)
                    targetX += 5f * Time.deltaTime;
            }

            targetX = Mathf.Clamp(targetX, -boardHalfWidth, boardHalfWidth);
            transform.position = new Vector3(targetX, transform.position.y, 0f);
        }

        private void CheckDropInput()
        {
            bool drop = (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                     || (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame);
            if (drop) Drop();
        }

        public void Drop()
        {
            if (!_canDrop) return;

            SpawnAtCurrentPosition();

            _canDrop = false;
            _cooldownTimer = dropCooldown;
            if (previewRenderer != null) previewRenderer.enabled = false;
        }

        private void SpawnAtCurrentPosition()
        {
            if (fruitPrefab != null)
            {
                // Task 1.4 이후: Fruit 프리팹 스폰 + Kinematic → Dynamic
                var fruit = Instantiate(fruitPrefab, transform.position, Quaternion.identity);
                var rb = fruit.GetComponent<Rigidbody2D>();
                if (rb != null) rb.bodyType = RigidbodyType2D.Dynamic;
            }
            else
            {
                SpawnPlaceholderFruit();
            }
        }

        private void SpawnPlaceholderFruit()
        {
            var go = new GameObject($"Fruit_Stage{CurrentStage?.StageIndex ?? 0}");
            go.transform.position = transform.position;

            float diameter = (CurrentStage != null && CurrentStage.Diameter > 0f)
                             ? CurrentStage.Diameter : 0.6f;
            go.transform.localScale = Vector3.one * diameter;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CreateCircleSprite();
            sr.color = StageColor(CurrentStage?.StageIndex ?? 1);

            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 1f;

            var col = go.AddComponent<CircleCollider2D>();
            col.radius = 0.5f;
        }

        private void RefreshPreview()
        {
            if (previewRenderer == null) return;

            float diameter = (CurrentStage != null && CurrentStage.Diameter > 0f)
                             ? CurrentStage.Diameter : 0.6f;
            previewRenderer.transform.localScale = Vector3.one * diameter;
            previewRenderer.color = StageColor(CurrentStage?.StageIndex ?? 1);
            previewRenderer.enabled = true;
        }

        private FruitStageData PickRandom()
        {
            if (stageTable == null) return null;
            var spawnables = stageTable.GetSpawnables();
            if (spawnables == null || spawnables.Length == 0) return null;

            float total = 0f;
            foreach (var s in spawnables) total += s.SpawnWeight;

            float roll = Random.Range(0f, total);
            float cum = 0f;
            foreach (var s in spawnables)
            {
                cum += s.SpawnWeight;
                if (roll <= cum) return s;
            }
            return spawnables[spawnables.Length - 1];
        }

        // 단계별 식별 색상 (Task 2 스프라이트 교체 전 플레이스홀더)
        private static Color StageColor(int stage)
        {
            Color[] palette =
            {
                new Color(1f, 0.2f, 0.2f),   // 1 체리
                new Color(1f, 0.4f, 0.5f),   // 2 딸기
                new Color(0.5f, 0.2f, 0.8f), // 3 포도
                new Color(1f, 0.6f, 0.1f),   // 4 데코폰
                new Color(0.9f, 0.5f, 0.1f), // 5 감
                new Color(0.8f, 0.1f, 0.1f), // 6 사과
                new Color(0.9f, 0.9f, 0.5f), // 7 배
                new Color(1f, 0.6f, 0.6f),   // 8 복숭아
                new Color(1f, 0.8f, 0.1f),   // 9 파인애플
                new Color(0.4f, 0.8f, 0.3f), // 10 멜론
                new Color(0.1f, 0.6f, 0.1f), // 11 수박
            };
            int i = Mathf.Clamp(stage - 1, 0, palette.Length - 1);
            return palette[i];
        }

        private static Sprite CreateCircleSprite()
        {
            const int res = 64;
            var tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
            var center = new Vector2(res * 0.5f, res * 0.5f);
            float radius = res * 0.5f - 1f;
            for (int y = 0; y < res; y++)
                for (int x = 0; x < res; x++)
                    tex.SetPixel(x, y, Vector2.Distance(new Vector2(x, y), center) <= radius
                        ? Color.white : Color.clear);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, res, res), Vector2.one * 0.5f, res);
        }
    }
}
