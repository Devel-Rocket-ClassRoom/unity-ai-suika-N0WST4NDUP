using UnityEngine;

namespace Watermelon.Gameplay
{
    [ExecuteAlways]
    [RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
    public class WallVisual : MonoBehaviour
    {
        [SerializeField] private Color color = new Color(0.55f, 0.40f, 0.25f, 1f);

        private static Sprite _sharedSprite;

        private void OnEnable() => ApplyVisual();

        #if UNITY_EDITOR
        private void OnValidate()
        {
            UnityEditor.EditorApplication.delayCall -= ApplyVisual;
            UnityEditor.EditorApplication.delayCall += ApplyVisual;
        }
        #endif

        private void ApplyVisual()
        {
            if (this == null) return;

            if (_sharedSprite == null)
            {
                var tex = Texture2D.whiteTexture;
                _sharedSprite = Sprite.Create(
                    tex,
                    new Rect(0, 0, tex.width, tex.height),
                    Vector2.one * 0.5f,
                    1f);
            }

            var sr = GetComponent<SpriteRenderer>();
            var col = GetComponent<BoxCollider2D>();
            sr.sprite = _sharedSprite;
            sr.color = color;
            sr.drawMode = SpriteDrawMode.Sliced;
            sr.size = col.size;
        }
    }
}
