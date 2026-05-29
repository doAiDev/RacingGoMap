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
