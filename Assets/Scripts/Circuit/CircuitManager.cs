using System.Collections.Generic;
using UnityEngine;

namespace RacingGoMap.Circuit
{
    [CreateAssetMenu(fileName = "CircuitManager", menuName = "RacingGoMap/Circuit Manager")]
    public class CircuitManager : ScriptableObject
    {
        public List<CircuitData> availableCircuits;

        public CircuitData GetRandom()
        {
            if (availableCircuits == null || availableCircuits.Count == 0) return null;
            return availableCircuits[Random.Range(0, availableCircuits.Count)];
        }

        public CircuitData GetById(string id)
        {
            foreach (var c in availableCircuits)
                if (c.circuitId == id) return c;
            return null;
        }
    }
}
