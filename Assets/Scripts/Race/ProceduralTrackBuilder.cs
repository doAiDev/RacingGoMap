using UnityEngine;
using System.Collections.Generic;

namespace RacingGoMap.Race
{
    /// <summary>
    /// 런타임에 트랙 웨이포인트를 받아 도로·컵·체크포인트·시작 그리드를 생성합니다.
    /// </summary>
    public class ProceduralTrackBuilder : MonoBehaviour
    {
        public RaceStartGrid StartGrid    { get; private set; }
        public int           CheckpointCount { get; private set; }

        public void Build(TrackWaypointData data)
        {
            ClearChildren();
            SetBackground();
            BuildRoad(data);
            BuildCurbs(data);
            CheckpointCount = PlaceCheckpoints(data);
            StartGrid       = BuildStartGrid(data);
        }

        // ── 내부 단계 ───────────────────────────────────────

        void ClearChildren()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                Destroy(transform.GetChild(i).gameObject);
        }

        void SetBackground()
        {
            if (Camera.main != null)
                Camera.main.backgroundColor = new Color(0.54f, 0.80f, 0.46f);
        }

        void BuildRoad(TrackWaypointData data)
        {
            // 도로 면 (gray)
            MakeLine("Road", data.waypoints, data.trackWidth,
                     new Color(0.80f, 0.80f, 0.85f), sortOrder: 1, corners: 8);

            // 중앙 점선
            MakeLine("CenterDash", data.waypoints, 0.12f,
                     new Color(1f, 1f, 1f, 0.30f), sortOrder: 2, corners: 4);

            // 결승선 체커 패턴
            PlaceFinishLine(data);
        }

        void PlaceFinishLine(TrackWaypointData data)
        {
            var go = new GameObject("FinishVisual");
            go.transform.parent = transform;

            Vector2 pos  = data.waypoints[0];
            Vector2 next = data.waypoints[1];
            Vector2 dir  = (next - pos).normalized;
            float   angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            go.transform.position = new Vector3(pos.x, pos.y, 0);
            go.transform.rotation = Quaternion.Euler(0, 0, angle);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite       = MakeCheckerSprite(8, 2);
            sr.sortingOrder = 3;
            sr.drawMode     = SpriteDrawMode.Sliced;
            sr.size         = new Vector2(data.trackWidth, 0.9f);
        }

        void BuildCurbs(TrackWaypointData data)
        {
            float half = data.trackWidth * 0.5f;

            // 바깥쪽 컵
            MakeCurb("CurbOuter",  data.waypoints,  half + 0.1f, 3);
            // 안쪽 컵
            MakeCurb("CurbInner",  data.waypoints, -(half + 0.1f), 3);

            // 벽 충돌체
            MakeWall("WallOuter",  data.waypoints,  half + 0.55f);
            MakeWall("WallInner",  data.waypoints, -(half + 0.55f));
        }

        int PlaceCheckpoints(TrackWaypointData data)
        {
            var pts  = data.waypoints;
            int n    = pts.Length;
            int step = Mathf.Max(1, n / 8);
            int idx  = 0;

            for (int i = 0; i < n; i += step)
            {
                bool   finish = (idx == 0);
                Vector2 pos   = pts[i];
                Vector2 next  = pts[(i + 1) % n];
                Vector2 dir   = (next - pos).normalized;
                float   angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

                var go = new GameObject(finish ? "FinishLine" : $"CP_{idx}");
                go.transform.parent   = transform;
                go.transform.position = new Vector3(pos.x, pos.y, 0);
                go.transform.rotation = Quaternion.Euler(0, 0, angle);

                var col   = go.AddComponent<BoxCollider2D>();
                col.isTrigger = true;
                col.size      = new Vector2(data.trackWidth + 1f, 0.4f);

                var cp = go.AddComponent<Checkpoint>();
                cp.checkpointIndex = idx;
                cp.SetFinishLine(finish);

                idx++;
            }
            return idx;
        }

        RaceStartGrid BuildStartGrid(TrackWaypointData data)
        {
            var go = new GameObject("StartGrid");
            go.transform.parent = transform;

            var grid = go.AddComponent<RaceStartGrid>();
            Vector2 pos  = data.waypoints[0];
            Vector2 fwd  = (data.waypoints[1] - pos).normalized;
            grid.GenerateSlots(pos, fwd, 5, 2.8f);
            return grid;
        }

        // ── 헬퍼 ─────────────────────────────────────────────

        void MakeLine(string name, Vector2[] pts, float width, Color col, int sortOrder, int corners)
        {
            var go = new GameObject(name);
            go.transform.parent = transform;
            var lr = go.AddComponent<LineRenderer>();
            lr.positionCount  = pts.Length + 1;
            lr.loop           = true;
            lr.startWidth     = lr.endWidth = width;
            lr.numCornerVertices = corners;
            lr.numCapVertices    = corners;
            lr.sortingOrder   = sortOrder;
            lr.material       = new Material(Shader.Find("Sprites/Default")) { color = col };

            for (int i = 0; i < pts.Length; i++)
                lr.SetPosition(i, new Vector3(pts[i].x, pts[i].y, 0));
            lr.SetPosition(pts.Length, new Vector3(pts[0].x, pts[0].y, 0));
        }

        void MakeCurb(string name, Vector2[] pts, float offset, int sortOrder)
        {
            int n  = pts.Length;
            var go = new GameObject(name);
            go.transform.parent = transform;

            var lr = go.AddComponent<LineRenderer>();
            lr.positionCount     = n + 1;
            lr.loop              = true;
            lr.startWidth        = lr.endWidth = 0.75f;
            lr.numCornerVertices = 6;
            lr.sortingOrder      = sortOrder;

            // 주황-흰 교대 그라디언트
            var grad = new Gradient();
            int segs = 8;
            var ck   = new GradientColorKey[segs * 2];
            var ak   = new GradientAlphaKey[] { new(1, 0), new(1, 1) };
            for (int i = 0; i < segs; i++)
            {
                float t0 = i / (float)segs;
                float t1 = (i + 0.5f) / segs;
                Color c = (i % 2 == 0) ? new Color(1f, 0.42f, 0.21f) : Color.white;
                ck[i * 2]     = new GradientColorKey(c, t0);
                ck[i * 2 + 1] = new GradientColorKey(c, t1);
            }
            grad.colorKeys = ck;
            grad.alphaKeys = ak;
            lr.colorGradient = grad;
            lr.material = new Material(Shader.Find("Sprites/Default"));

            for (int i = 0; i < n; i++)
            {
                Vector2 p = OffsetPoint(pts, i, offset);
                lr.SetPosition(i, new Vector3(p.x, p.y, 0));
            }
            lr.SetPosition(n, lr.GetPosition(0));
        }

        void MakeWall(string name, Vector2[] pts, float offset)
        {
            int n  = pts.Length;
            var go = new GameObject(name);
            go.transform.parent = transform;

            var ec  = go.AddComponent<EdgeCollider2D>();
            var wps = new List<Vector2>(n + 1);
            for (int i = 0; i < n; i++) wps.Add(OffsetPoint(pts, i, offset));
            wps.Add(wps[0]);
            ec.SetPoints(wps);
        }

        static Vector2 OffsetPoint(Vector2[] pts, int i, float offset)
        {
            int     n    = pts.Length;
            Vector2 cur  = pts[i];
            Vector2 next = pts[(i + 1) % n];
            Vector2 dir  = (next - cur).normalized;
            Vector2 perp = new(-dir.y, dir.x);
            return cur + perp * offset;
        }

        static Sprite MakeCheckerSprite(int cols, int rows)
        {
            int cw = 8, ch = 8;
            int w = cols * cw, h = rows * ch;
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
            {
                bool black = ((x / cw) + (y / ch)) % 2 == 0;
                tex.SetPixel(x, y, black ? Color.black : Color.white);
            }
            tex.Apply();
            return Sprite.Create(tex,
                new Rect(0, 0, w, h),
                new Vector2(0.5f, 0.5f),
                w,
                0,
                SpriteMeshType.FullRect,
                new Vector4(cw, ch, cw, ch));
        }
    }
}
