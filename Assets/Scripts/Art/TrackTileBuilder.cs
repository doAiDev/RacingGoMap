using UnityEngine;

namespace RacingGoMap.Art
{
    /// <summary>
    /// 코드로 트랙 타일 텍스처를 생성합니다.
    /// - 도로 타일 (64x64)
    /// - 커브 타일
    /// - 엣지/컵 타일 (주황-흰 체크)
    /// - 잔디 타일
    /// </summary>
    public static class TrackTileBuilder
    {
        const int T = 64; // 타일 크기

        // ── 팔레트 ──────────────────────────────────────────
        static readonly Color RoadColor   = new(0.80f, 0.80f, 0.85f);
        static readonly Color RoadLine    = new(1f,    1f,    1f,    0.85f);
        static readonly Color GrassLight  = new(0.56f, 0.82f, 0.48f);
        static readonly Color GrassDark   = new(0.44f, 0.72f, 0.36f);
        static readonly Color CurbOrange  = new(1f,    0.42f, 0.21f);
        static readonly Color CurbWhite   = Color.white;
        static readonly Color RoadShadow  = new(0.68f, 0.68f, 0.72f);

        // ── 공개 팩토리 ──────────────────────────────────────

        public static Sprite BuildRoadStraight()
        {
            var tex = NewTex();
            Fill(tex, RoadColor);
            // 가운데 점선
            for (int y = 4; y < T; y += 12)
                DrawRect(tex, T / 2 - 2, y, T / 2 + 2, y + 6, RoadLine);
            // 양쪽 엣지 컵 패턴
            DrawCurb(tex, 0, 0, 6, T);
            DrawCurb(tex, T - 6, 0, T, T);
            tex.Apply();
            return ToSprite(tex);
        }

        public static Sprite BuildRoadCurve()
        {
            var tex = NewTex();
            Fill(tex, GrassLight);
            // 부채꼴 도로 영역
            int cx = 0, cy = 0, r = T;
            for (int x = 0; x < T; x++)
            for (int y = 0; y < T; y++)
            {
                float d = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));
                if (d < r && d > r - 20) SetPixelSafe(tex, x, y, RoadColor);
            }
            DrawCurbArc(tex, cx, cy, r,      r - 4,  6);
            DrawCurbArc(tex, cx, cy, r - 16, r - 20, 6);
            tex.Apply();
            return ToSprite(tex);
        }

        public static Sprite BuildGrass()
        {
            var tex = NewTex();
            // 밝은/어두운 격자 패턴으로 잔디 표현
            for (int x = 0; x < T; x++)
            for (int y = 0; y < T; y++)
            {
                bool checker = ((x / 8) + (y / 8)) % 2 == 0;
                SetPixelSafe(tex, x, y, checker ? GrassLight : GrassDark);
            }
            tex.Apply();
            return ToSprite(tex);
        }

        public static Sprite BuildFinishLine()
        {
            var tex = NewTex();
            Fill(tex, RoadColor);
            // 흑백 체커 결승선
            int stripeW = 8;
            for (int x = 0; x < T; x++)
            for (int y = 20; y < 44; y++)
            {
                bool isBlack = ((x / stripeW) + (y / stripeW)) % 2 == 0;
                SetPixelSafe(tex, x, y, isBlack ? Color.black : Color.white);
            }
            DrawCurb(tex, 0, 0, 6, T);
            DrawCurb(tex, T - 6, 0, T, T);
            tex.Apply();
            return ToSprite(tex);
        }

        // ── 컵(curb) 헬퍼 ────────────────────────────────────

        static void DrawCurb(Texture2D tex, int x0, int y0, int x1, int y1)
        {
            int segH = 8;
            for (int y = y0; y < y1; y++)
            {
                Color c = ((y / segH) % 2 == 0) ? CurbOrange : CurbWhite;
                for (int x = x0; x < x1; x++) SetPixelSafe(tex, x, y, c);
            }
        }

        static void DrawCurbArc(Texture2D tex, int cx, int cy, int rOuter, int rInner, int segDeg)
        {
            for (int x = 0; x < T; x++)
            for (int y = 0; y < T; y++)
            {
                float d = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));
                if (d <= rOuter && d >= rInner)
                {
                    float angle = Mathf.Atan2(y - cy, x - cx) * Mathf.Rad2Deg;
                    if (angle < 0) angle += 360f;
                    Color c = ((int)(angle / segDeg) % 2 == 0) ? CurbOrange : CurbWhite;
                    SetPixelSafe(tex, x, y, c);
                }
            }
        }

        // ── 공통 유틸 ────────────────────────────────────────

        static Texture2D NewTex()
        {
            var t = new Texture2D(T, T, TextureFormat.RGBA32, false);
            t.filterMode = FilterMode.Bilinear;
            return t;
        }

        static void Fill(Texture2D tex, Color c)
        {
            var px = new Color[T * T];
            for (int i = 0; i < px.Length; i++) px[i] = c;
            tex.SetPixels(px);
        }

        static void DrawRect(Texture2D tex, int x0, int y0, int x1, int y1, Color c)
        {
            for (int x = x0; x < x1; x++)
            for (int y = y0; y < y1; y++)
                SetPixelSafe(tex, x, y, c);
        }

        static void SetPixelSafe(Texture2D tex, int x, int y, Color c)
        {
            if (x >= 0 && x < tex.width && y >= 0 && y < tex.height)
                tex.SetPixel(x, y, c);
        }

        static Sprite ToSprite(Texture2D tex)
            => Sprite.Create(tex, new Rect(0, 0, T, T), new Vector2(0.5f, 0.5f), T);
    }
}
