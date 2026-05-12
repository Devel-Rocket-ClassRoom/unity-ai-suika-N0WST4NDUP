using UnityEngine;
using Watermelon.Data;

namespace Watermelon.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D), typeof(SpriteRenderer))]
    public class Fruit : MonoBehaviour
    {
        [SerializeField] private FruitStageData stageData;

        public FruitStageData StageData => stageData;

        private Rigidbody2D _rb;
        private CircleCollider2D _col;
        private SpriteRenderer _sr;

        private void Awake()
        {
            _rb  = GetComponent<Rigidbody2D>();
            _col = GetComponent<CircleCollider2D>();
            _sr  = GetComponent<SpriteRenderer>();
        }

        public void Init(FruitStageData data)
        {
            stageData = data;
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
                new Color(1f,    0.2f,  0.2f),
                new Color(1f,    0.4f,  0.5f),
                new Color(0.5f,  0.2f,  0.8f),
                new Color(1f,    0.6f,  0.1f),
                new Color(0.9f,  0.5f,  0.1f),
                new Color(0.8f,  0.1f,  0.1f),
                new Color(0.9f,  0.9f,  0.5f),
                new Color(1f,    0.6f,  0.6f),
                new Color(1f,    0.8f,  0.1f),
                new Color(0.4f,  0.8f,  0.3f),
                new Color(0.1f,  0.6f,  0.1f),
            };
            int i = Mathf.Clamp(stage - 1, 0, palette.Length - 1);
            return palette[i];
        }
    }
}
