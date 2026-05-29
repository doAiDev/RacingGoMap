using UnityEngine;

namespace RacingGoMap.Circuit
{
    [CreateAssetMenu(fileName = "NewCircuit", menuName = "RacingGoMap/Circuit Data")]
    public class CircuitData : ScriptableObject
    {
        public string circuitId;
        public string circuitName;
        public string sceneName;
        public Sprite previewImage;
        public int    totalLaps = 2;
    }
}
