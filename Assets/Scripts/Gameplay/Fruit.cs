using System.Collections;
using UnityEngine;
using Watermelon.Data;

namespace Watermelon.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D), typeof(SpriteRenderer))]
    public class Fruit : MonoBehaviour
    {
        // GameManager(Task 1.7)에서 구독 — 머지 발생 시 결과 단계 전달
        public static event System.Action<FruitStageData> OnMerge;

        [SerializeField] private FruitStageData stageData;

        public FruitStageData StageData => stageData;

        private Rigidbody2D _rb;
        private CircleCollider2D _col;
        private SpriteRenderer _sr;
        private GameObject _fruitPrefab;
        private bool _isMerging;

        private void Awake()
        {
            _rb  = GetComponent<Rigidbody2D>();
            _col = GetComponent<CircleCollider2D>();
            _sr  = GetComponent<SpriteRenderer>();
        }

        public void Init(FruitStageData data, GameObject fruitPrefab)
        {
            stageData    = data;
            _fruitPrefab = fruitPrefab;
            ApplyStageVisuals();
        }

        public void SetSprite(Sprite sprite)
        {
            if (_sr == null) _sr = GetComponent<SpriteRenderer>();
            _sr.sprite = sprite;
        }

        public void SetKinematic(bool kinematic)
        {
            if (_rb == null) _rb = GetComponent<Rigidbody2D>();
            _rb.bodyType = kinematic ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_isMerging) return;
            if (stageData == null || stageData.NextStage == null) return;

            var other = collision.gameObject.GetComponent<Fruit>();
            if (other == null || other._isMerging) return;
            if (other.stageData == null) return;
            if (other.stageData.StageIndex != stageData.StageIndex) return;

            // 인스턴스 ID가 더 큰 쪽이 머지 처리 (중복 방지)
            if (other.GetInstanceID() > GetInstanceID()) return;

            _isMerging       = true;
            other._isMerging = true;

            // 운동량 보존: p = m*v 합산 후 합쳐진 질량으로 나눔
            float massA = _rb.mass;
            float massB = other._rb.mass;
            Vector2 momentum = _rb.linearVelocity * massA + other._rb.linearVelocity * massB;
            Vector2 mergedVelocity = momentum / (massA + massB);

            Vector2 mergePos = ((Vector2)transform.position + (Vector2)other.transform.position) * 0.5f;

            StartCoroutine(DoMerge(other, mergePos, mergedVelocity));
        }

        private IEnumerator DoMerge(Fruit other, Vector2 mergePos, Vector2 mergedVelocity)
        {
            // 콜라이더 비활성화로 이번 프레임 추가 충돌 방지
            _col.enabled       = false;
            other._col.enabled = false;

            yield return null; // 물리 스텝 이후 처리

            OnMerge?.Invoke(stageData.NextStage);

            if (_fruitPrefab != null)
            {
                var go      = Instantiate(_fruitPrefab, mergePos, Quaternion.identity);
                var fruit   = go.GetComponent<Fruit>();
                if (fruit != null) fruit.Init(stageData.NextStage, _fruitPrefab);
                var rb      = go.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.bodyType       = RigidbodyType2D.Dynamic;
                    rb.linearVelocity = mergedVelocity;
                }
            }

            Destroy(other.gameObject);
            Destroy(gameObject);
        }

        private void ApplyStageVisuals()
        {
            if (stageData == null) return;

            float diameter = stageData.Diameter > 0f ? stageData.Diameter : 0.6f;
            transform.localScale = Vector3.one * diameter;
            _col.radius = 0.5f;

            if (_sr.sprite == null)
                _sr.sprite = PlaceholderSprite.Get();

            _sr.color = StageColor(stageData.StageIndex);
        }

        private static Color StageColor(int stage)
        {
            Color[] palette =
            {
                new Color(1f,   0.2f,  0.2f),
                new Color(1f,   0.4f,  0.5f),
                new Color(0.5f, 0.2f,  0.8f),
                new Color(1f,   0.6f,  0.1f),
                new Color(0.9f, 0.5f,  0.1f),
                new Color(0.8f, 0.1f,  0.1f),
                new Color(0.9f, 0.9f,  0.5f),
                new Color(1f,   0.6f,  0.6f),
                new Color(1f,   0.8f,  0.1f),
                new Color(0.4f, 0.8f,  0.3f),
                new Color(0.1f, 0.6f,  0.1f),
            };
            int i = Mathf.Clamp(stage - 1, 0, palette.Length - 1);
            return palette[i];
        }
    }
}
