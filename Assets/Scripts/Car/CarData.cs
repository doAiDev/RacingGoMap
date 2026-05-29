using UnityEngine;

namespace RacingGoMap.Car
{
    [CreateAssetMenu(fileName = "NewCar", menuName = "RacingGoMap/Car Data")]
    public class CarData : ScriptableObject
    {
        public string  carId;
        public string  carName;
        [TextArea]
        public string  description;
        public Sprite  carSprite;
        public Color   primaryColor = Color.white;
        public int     purchaseCost;
        [Range(1f, 15f)]
        public float   bumpForce = 3f;
        [Range(1f, 5f)]
        public float   bumpResistance = 1f;
        public bool    isDefault;
    }
}
