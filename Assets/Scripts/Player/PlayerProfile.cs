using System.Collections.Generic;

namespace RacingGoMap.Player
{
    [System.Serializable]
    public class PlayerProfile
    {
        public string playerId   = "";
        public string playerName = "Racer";
        public int    totalPoints = 0;
        public List<string> ownedCarIds    = new List<string> { "car_basic" };
        public string          equippedCarId = "car_basic";
    }
}
