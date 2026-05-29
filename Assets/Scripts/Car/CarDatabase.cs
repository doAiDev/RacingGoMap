using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RacingGoMap.Car
{
    [CreateAssetMenu(fileName = "CarDatabase", menuName = "RacingGoMap/Car Database")]
    public class CarDatabase : ScriptableObject
    {
        public List<CarData> cars;

        public CarData GetById(string id)  => cars.FirstOrDefault(c => c.carId == id);
        public CarData GetDefault()        => cars.FirstOrDefault(c => c.isDefault) ?? cars[0];
    }
}
