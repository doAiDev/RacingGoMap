using UnityEngine;
using RacingGoMap.Core;

namespace RacingGoMap.Player
{
    public class PlayerDataStore : Singleton<PlayerDataStore>
    {
        const string KEY = "player_profile";

        public PlayerProfile Profile { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Load();
        }

        void Load()
        {
            string json = PlayerPrefs.GetString(KEY, "");
            if (string.IsNullOrEmpty(json))
            {
                Profile = new PlayerProfile();
                Profile.playerId = System.Guid.NewGuid().ToString("N");
                Save();
            }
            else
            {
                Profile = JsonUtility.FromJson<PlayerProfile>(json);
            }
        }

        public void Save()
        {
            PlayerPrefs.SetString(KEY, JsonUtility.ToJson(Profile));
            PlayerPrefs.Save();
        }

        public void AddPoints(int amount) { Profile.totalPoints += amount; Save(); }

        public bool BuyCar(string carId, int cost)
        {
            if (Profile.totalPoints < cost)           return false;
            if (Profile.ownedCarIds.Contains(carId))  return false;
            Profile.totalPoints -= cost;
            Profile.ownedCarIds.Add(carId);
            Save();
            return true;
        }

        public void EquipCar(string carId)
        {
            if (Profile.ownedCarIds.Contains(carId))
            {
                Profile.equippedCarId = carId;
                Save();
            }
        }
    }
}
