using UnityEngine;

namespace RacingGoMap.Art
{
    /// <summary>
    /// 나무, 관중석 등 환경 요소를 코드로 생성합니다.
    /// </summary>
    public static class EnvironmentBuilder
    {
        // ── 나무 (둥근 실루엣) ────────────────────────────────

        public static Sprite BuildTree(TreeSize size = TreeSize.Medium)
        {
            int r = size == TreeSize.Small ? 20 : size == TreeSize.Medium ? 28 : 36;
            int s = r * 2 + 4;
            var tex = new Texture2D(s, s, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            Clear(tex);

            Color shadow  = new(0.22f, 0.50f, 0.22f);
            Color mid     = new(0.33f, 0.68f, 0.33f);
            Color light   = new(0.50f, 0.82f, 0.42f);
            Color shine   = new(0.70f, 0.92f, 0.60f);
            int cx = s / 2, cy = s / 2;

            // 그림자 원
            DrawCircle(tex, cx + 2, cy - 2, r,     shadow);
            // 본체
            DrawCircle(tex, cx,     cy,     r,     mid);
            // 밝은 영역
            DrawCircle(tex, cx - 3, cy + 3, r - 5, light);
            // 하이라이트
            DrawCircle(tex, cx - 5, cy + 5, r - 10, shine);

            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, s, s), new Vector2(0.5f, 0.5f), 32f);
        }

        // ── 관중석 블록 ──────────────────────────────────────

        public static Sprite BuildStand(int rows = 3, int cols = 5)
        {
            int cellW = 12, cellH = 10;
            int w = cols * cellW, h = rows * cellH;
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            Clear(tex);

            Color[] headColors =
            {
                new(1f,    0.55f, 0.55f),
                new(0.55f, 0.75f, 1f   ),
                new(1f,    0.90f, 0.45f),
                new(0.60f, 0.90f, 0.60f),
                new(0.85f, 0.65f, 1f   ),
            };
            Color seatColor = new(0.30f, 0.35f, 0.50f);
            Color outline   = new(0.15f, 0.15f, 0.20f);

            for (int row = 0; row < rows; row++)
            for (int col = 0; col < cols; col++)
            {
                int x0 = col * cellW, y0 = row * cellH;
                // 좌석
                FillRect(tex, x0 + 1, y0 + 1, x0 + cellW - 1, y0 + cellH - 3, seatColor);
                // 머리 (원형)
                Color hc = headColors[(row * cols + col) % headColors.Length];
                DrawCircleFill(tex, x0 + cellW / 2, y0 + cellH - 3, 3, hc);
                // 테두리
                FillRect(tex, x0, y0, x0 + cellW, y0 + 1, outline);
                FillRect(tex, x0, y0, x0 + 1, y0 + cellH, outline);
            }

            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 32f);
        }

        // ── 장애물 (배리어) ──────────────────────────────────

        public static Sprite BuildBarrier()
        {
            int w = 48, h = 16;
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            Clear(tex);

            // 주황-흰 대각 스트라이프
            for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
            {
                Color c = ((x + y) / 8 % 2 == 0)
                    ? new Color(1f, 0.42f, 0.21f)
                    : Color.white;
                tex.SetPixel(x, y, c);
            }
            // 위아래 회색 테두리
            for (int x = 0; x < w; x++)
            {
                Color border = new(0.55f, 0.55f, 0.60f);
                tex.SetPixel(x, 0,     border);
                tex.SetPixel(x, h - 1, border);
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 32f);
        }

        // ── 공통 유틸 ────────────────────────────────────────

        static void Clear(Texture2D tex)
        {
            var px = new Color[tex.width * tex.height];
            for (int i = 0; i < px.Length; i++) px[i] = Color.clear;
            tex.SetPixels(px);
        }

        static void DrawCircle(Texture2D tex, int cx, int cy, int r, Color c)
        {
            for (int x = cx - r; x <= cx + r; x++)
            for (int y = cy - r; y <= cy + r; y++)
            {
                float d = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));
                if (d <= r) SetSafe(tex, x, y, c);
            }
        }

        static void DrawCircleFill(Texture2D tex, int cx, int cy, int r, Color c)
            => DrawCircle(tex, cx, cy, r, c);

        static void FillRect(Texture2D tex, int x0, int y0, int x1, int y1, Color c)
        {
            for (int x = x0; x < x1; x++)
            for (int y = y0; y < y1; y++)
                SetSafe(tex, x, y, c);
        }

        static void SetSafe(Texture2D tex, int x, int y, Color c)
        {
            if (x >= 0 && x < tex.width && y >= 0 && y < tex.height)
                tex.SetPixel(x, y, c);
        }

        public enum TreeSize { Small, Medium, Large }
    }
}
