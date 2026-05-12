using UnityEngine;

namespace Watermelon.Gameplay
{
    public static class PlaceholderSprite
    {
        private static Sprite _circle;

        public static Sprite Get()
        {
            if (_circle == null) _circle = CreateCircle();
            return _circle;
        }

        private static Sprite CreateCircle()
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
