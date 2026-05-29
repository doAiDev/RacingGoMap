using UnityEngine;

namespace RacingGoMap.Race
{
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] public int  checkpointIndex;
        [SerializeField] bool        _isFinishLine;
        public bool IsFinishLine => _isFinishLine;

        void OnTriggerEnter2D(Collider2D other)
        {
            var tracker = other.GetComponent<LapTracker>();
            if (tracker != null)
                tracker.OnCheckpointReached(checkpointIndex, _isFinishLine);
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = _isFinishLine ? Color.green : Color.yellow;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
#endif
    }
}
