using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RacingGoMap.Art
{
    /// <summary>
    /// 런타임에 UI 컴포넌트에 파스텔 테마를 적용합니다.
    /// 외부 스킨 에셋 없이 코드만으로 힙한 UI.
    /// </summary>
    public static class UIStyleBuilder
    {
        // ── 팔레트 ──────────────────────────────────────────
        public static readonly Color AccentOrange = new(1f,    0.42f, 0.21f);
        public static readonly Color PanelBg      = new(0.12f, 0.12f, 0.18f, 0.95f);
        public static readonly Color ButtonBg     = new(1f,    0.42f, 0.21f);
        public static readonly Color ButtonText   = Color.white;
        public static readonly Color SubText      = new(0.75f, 0.75f, 0.75f);
        public static readonly Color PositiveGreen = new(0.30f, 0.85f, 0.50f);
        public static readonly Color DangerRed    = new(1f,    0.35f, 0.35f);

        // ── 버튼 스타일링 ────────────────────────────────────

        public static void StylePrimary(Button btn)
        {
            var img = btn.GetComponent<Image>();
            if (img) img.color = ButtonBg;

            var cb = btn.colors;
            cb.normalColor      = ButtonBg;
            cb.highlightedColor = Lighten(ButtonBg, 0.15f);
            cb.pressedColor     = Darken(ButtonBg, 0.20f);
            cb.selectedColor    = ButtonBg;
            cb.disabledColor    = new Color(0.5f, 0.5f, 0.5f);
            btn.colors = cb;

            var label = btn.GetComponentInChildren<TMP_Text>();
            if (label)
            {
                label.color     = ButtonText;
                label.fontStyle = FontStyles.Bold;
            }
        }

        public static void StyleSecondary(Button btn)
        {
            var cb = btn.colors;
            cb.normalColor      = new Color(0.25f, 0.25f, 0.32f);
            cb.highlightedColor = new Color(0.32f, 0.32f, 0.40f);
            cb.pressedColor     = new Color(0.18f, 0.18f, 0.24f);
            btn.colors = cb;
        }

        public static void StylePanel(Image panel)
        {
            panel.color  = PanelBg;
            panel.sprite = BuildRoundedRectSprite(12);
            panel.type   = Image.Type.Sliced;
        }

        // ── 동적 스프라이트 ──────────────────────────────────

        public static Sprite BuildRoundedRectSprite(int radius)
        {
            int size = 64;
            var tex  = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            var px = new Color[size * size];
            for (int i = 0; i < px.Length; i++) px[i] = Color.clear;
            tex.SetPixels(px);

            for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                if (InsideRounded(x, y, 0, 0, size - 1, size - 1, radius))
                    tex.SetPixel(x, y, Color.white);
            }
            tex.Apply();

            int b = radius;
            return Sprite.Create(tex,
                new Rect(0, 0, size, size),
                new Vector2(0.5f, 0.5f),
                1f,
                0,
                SpriteMeshType.FullRect,
                new Vector4(b, b, b, b));
        }

        public static Sprite BuildCircleSprite(int diameter = 64)
        {
            var tex = new Texture2D(diameter, diameter, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            var px = new Color[diameter * diameter];
            for (int i = 0; i < px.Length; i++) px[i] = Color.clear;
            tex.SetPixels(px);

            float cx = (diameter - 1) / 2f;
            float cy = (diameter - 1) / 2f;
            float r  = diameter / 2f - 0.5f;
            for (int x = 0; x < diameter; x++)
            for (int y = 0; y < diameter; y++)
            {
                float dx = x - cx, dy = y - cy;
                if (dx * dx + dy * dy <= r * r)
                    tex.SetPixel(x, y, Color.white);
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, diameter, diameter), new Vector2(0.5f, 0.5f), 1f);
        }

        // ── 조이스틱 스킨 ────────────────────────────────────

        public static void ApplyJoystickSkin(Image background, Image handle)
        {
            background.sprite = BuildCircleSprite(128);
            background.color  = new Color(1f, 1f, 1f, 0.18f);

            handle.sprite = BuildCircleSprite(64);
            handle.color  = new Color(1f, 1f, 1f, 0.55f);
        }

        // ── 공통 유틸 ────────────────────────────────────────

        static bool InsideRounded(int px, int py, int x0, int y0, int x1, int y1, int r)
        {
            if (px < x0 || px > x1 || py < y0 || py > y1) return false;
            if (px < x0 + r && py < y0 + r) return Dist(px, py, x0 + r, y0 + r) <= r;
            if (px > x1 - r && py < y0 + r) return Dist(px, py, x1 - r, y0 + r) <= r;
            if (px < x0 + r && py > y1 - r) return Dist(px, py, x0 + r, y1 - r) <= r;
            if (px > x1 - r && py > y1 - r) return Dist(px, py, x1 - r, y1 - r) <= r;
            return true;
        }

        static float Dist(int ax, int ay, int bx, int by)
            => Mathf.Sqrt((ax - bx) * (ax - bx) + (ay - by) * (ay - by));

        static Color Darken(Color c, float t)
            => new(c.r * (1 - t), c.g * (1 - t), c.b * (1 - t), c.a);

        static Color Lighten(Color c, float t)
            => new(Mathf.Clamp01(c.r + t), Mathf.Clamp01(c.g + t), Mathf.Clamp01(c.b + t), c.a);
    }
}
