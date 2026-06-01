using UnityEngine;

namespace RacingGoMap.Race
{
    public class RaceStartGrid : MonoBehaviour
    {
        [SerializeField] Transform[] _slots;

        public Transform GetSlot(int index)
        {
            if (_slots == null || index < 0 || index >= _slots.Length) return transform;
            return _slots[index];
        }

        public int Count => _slots?.Length ?? 0;

        /// <summary>
        /// 런타임에 시작 그리드 슬롯을 동적 생성합니다.
        /// </summary>
        public void GenerateSlots(Vector2 startPos, Vector2 forward, int count, float rowSpacing)
        {
            var right = new Vector2(-forward.y, forward.x);
            var back  = -forward;

            _slots = new Transform[count];
            for (int i = 0; i < count; i++)
            {
                int   row   = i / 2;
                float side  = (i % 2 == 0) ? -1.4f : 1.4f;
                Vector2 p   = (Vector2)startPos
                            + right * side
                            + back  * (row * rowSpacing + 1f);

                var go = new GameObject($"Slot_{i + 1}");
                go.transform.parent   = transform;
                go.transform.position = new Vector3(p.x, p.y, 0);

                float angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg - 90f;
                go.transform.rotation = Quaternion.Euler(0, 0, angle);
                _slots[i] = go.transform;
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (_slots == null) return;
            Gizmos.color = Color.cyan;
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] == null) continue;
                Gizmos.DrawWireSphere(_slots[i].position, 0.4f);
                UnityEditor.Handles.Label(_slots[i].position + Vector3.right * 0.5f, $"P{i + 1}");
            }
        }
#endif
    }
}
