using UnityEngine;

namespace RacingGoMap.Race
{
    [System.Serializable]
    public class TrackWaypointData
    {
        public string   circuitId;
        public string   circuitName;
        public float    trackWidth = 6f;
        public int      totalLaps  = 2;
        public Vector2[] waypoints;
    }
}
