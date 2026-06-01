using UnityEngine;

namespace RacingGoMap.Art
{
    /// <summary>
    /// 외부 이미지 없이 코드로 탑뷰 카트 스프라이트를 생성합니다.
    /// 64x96 px 기준, 파스텔 쿼터뷰 스타일.
    /// </summary>
    public static class CarSpriteBuilder
    {
        const int W = 64;
        const int H = 96;

        public static Sprite Build(Color bodyColor)
        {
            var tex = new Texture2D(W, H, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;

            // 투명 배경
            Fill(tex, Color.clear);

            Color shadow    = Darken(bodyColor, 0.35f);
            Color highlight = Lighten(bodyColor, 0.45f);
            Color wheelCol  = new Color(0.18f, 0.18f, 0.20f);
            Color windshield = new Color(0.72f, 0.90f, 1f, 0.92f);
            Color outline   = new Color(0.10f, 0.10f, 0.12f, 1f);

            // --- 바퀴 그림자 (차체 아래)
            DrawEllipse(tex, 10, 14, 9, 7, Darken(wheelCol, 0.5f));
            DrawEllipse(tex, 54, 14, 9, 7, Darken(wheelCol, 0.5f));
            DrawEllipse(tex, 10, 80, 9, 7, Darken(wheelCol, 0.5f));
            DrawEllipse(tex, 54, 80, 9, 7, Darken(wheelCol, 0.5f));

            // --- 차체 본체 (둥근 사각형)
            DrawRoundRect(tex, 10, 10, W - 10, H - 10, 10, shadow);
            DrawRoundRect(tex, 8,  12, W - 8,  H - 8,  10, bodyColor);

            // --- 차체 하이라이트 (왼쪽 상단 광택)
            DrawRoundRect(tex, 10, H - 36, 28, H - 16, 6, highlight);

            // --- 앞 유리
            DrawRoundRect(tex, 16, H - 44, W - 16, H - 18, 5, windshield);

            // --- 뒷 유리 (작게)
            DrawRoundRect(tex, 18, 16, W - 18, 30, 4,
                new Color(windshield.r, windshield.g, windshield.b, 0.6f));

            // --- 차체 아웃라인
            DrawRoundRectOutline(tex, 8, 12, W - 8, H - 8, 10, outline, 2);

            // --- 바퀴
            DrawWheel(tex, 10, 18, wheelCol);
            DrawWheel(tex, 54, 18, wheelCol);
            DrawWheel(tex, 10, 76, wheelCol);
            DrawWheel(tex, 54, 76, wheelCol);

            // --- 번호판 (뒤쪽)
            DrawRoundRect(tex, 22, 10, W - 22, 20, 2, new Color(1f, 1f, 0.8f));

            tex.Apply();
            return Sprite.Create(tex,
                new Rect(0, 0, W, H),
                new Vector2(0.5f, 0.5f),
                32f);
        }

        // ────────────────── 기본 도형 유틸 ──────────────────

        static void Fill(Texture2D tex, Color c)
        {
            var pixels = new Color[tex.width * tex.height];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = c;
            tex.SetPixels(pixels);
        }

        static void DrawRoundRect(Texture2D tex, int x0, int y0, int x1, int y1, int r, Color c)
        {
            for (int x = x0; x <= x1; x++)
            for (int y = y0; y <= y1; y++)
            {
                if (InsideRoundRect(x, y, x0, y0, x1, y1, r))
                    BlendPixel(tex, x, y, c);
            }
        }

        static void DrawRoundRectOutline(Texture2D tex, int x0, int y0, int x1, int y1, int r, Color c, int thickness)
        {
            for (int t = 0; t < thickness; t++)
                DrawRoundRectRing(tex, x0 + t, y0 + t, x1 - t, y1 - t, r - t, c);
        }

        static void DrawRoundRectRing(Texture2D tex, int x0, int y0, int x1, int y1, int r, Color c)
        {
            for (int x = x0; x <= x1; x++)
            for (int y = y0; y <= y1; y++)
            {
                bool outer = InsideRoundRect(x, y, x0, y0, x1, y1, r);
                bool inner = InsideRoundRect(x, y, x0 + 1, y0 + 1, x1 - 1, y1 - 1, Mathf.Max(0, r - 1));
                if (outer && !inner) BlendPixel(tex, x, y, c);
            }
        }

        static bool InsideRoundRect(int px, int py, int x0, int y0, int x1, int y1, int r)
        {
            if (px < x0 || px > x1 || py < y0 || py > y1) return false;
            // 네 모서리 원 체크
            if (px < x0 + r && py < y0 + r) return Dist(px, py, x0 + r, y0 + r) <= r;
            if (px > x1 - r && py < y0 + r) return Dist(px, py, x1 - r, y0 + r) <= r;
            if (px < x0 + r && py > y1 - r) return Dist(px, py, x0 + r, y1 - r) <= r;
            if (px > x1 - r && py > y1 - r) return Dist(px, py, x1 - r, y1 - r) <= r;
            return true;
        }

        static void DrawEllipse(Texture2D tex, int cx, int cy, int rx, int ry, Color c)
        {
            for (int x = cx - rx; x <= cx + rx; x++)
            for (int y = cy - ry; y <= cy + ry; y++)
            {
                float dx = (float)(x - cx) / rx;
                float dy = (float)(y - cy) / ry;
                if (dx * dx + dy * dy <= 1f)
                    BlendPixel(tex, x, y, c);
            }
        }

        static void DrawWheel(Texture2D tex, int cx, int cy, Color c)
        {
            DrawEllipse(tex, cx, cy, 7, 7, c);
            // 휠 하이라이트
            DrawEllipse(tex, cx - 1, cy + 1, 2, 2, new Color(0.55f, 0.55f, 0.60f));
        }

        static void BlendPixel(Texture2D tex, int x, int y, Color src)
        {
            if (x < 0 || x >= tex.width || y < 0 || y >= tex.height) return;
            if (src.a <= 0f) return;
            if (src.a >= 1f) { tex.SetPixel(x, y, src); return; }
            Color dst = tex.GetPixel(x, y);
            float a   = src.a + dst.a * (1f - src.a);
            Color res = a > 0f
                ? new Color(
                    (src.r * src.a + dst.r * dst.a * (1f - src.a)) / a,
                    (src.g * src.a + dst.g * dst.a * (1f - src.a)) / a,
                    (src.b * src.a + dst.b * dst.a * (1f - src.a)) / a,
                    a)
                : Color.clear;
            tex.SetPixel(x, y, res);
        }

        static float Dist(int ax, int ay, int bx, int by)
            => Mathf.Sqrt((ax - bx) * (ax - bx) + (ay - by) * (ay - by));

        static Color Darken(Color c, float amount)
            => new Color(c.r * (1 - amount), c.g * (1 - amount), c.b * (1 - amount), c.a);

        static Color Lighten(Color c, float amount)
            => new Color(
                Mathf.Clamp01(c.r + (1f - c.r) * amount),
                Mathf.Clamp01(c.g + (1f - c.g) * amount),
                Mathf.Clamp01(c.b + (1f - c.b) * amount),
                c.a);
    }
}
