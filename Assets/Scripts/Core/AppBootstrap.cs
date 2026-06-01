using UnityEngine;
using RacingGoMap.Auth;
using RacingGoMap.Player;
using RacingGoMap.Network;
using RacingGoMap.Core;

namespace RacingGoMap.Core
{
    [DefaultExecutionOrder(-100)]
    public class AppBootstrap : MonoBehaviour
    {
        void Awake()
        {
            Ensure<SceneLoader>("SceneLoader");
            Ensure<AuthManager>("AuthManager");
            Ensure<PlayerDataStore>("PlayerDataStore");
            Ensure<RuntimeGameData>("RuntimeGameData");  // 차량+서킷 데이터
            Ensure<MatchmakingManager>("MatchmakingManager");
        }

        static void Ensure<T>(string n) where T : MonoBehaviour
        {
            if (FindObjectOfType<T>() == null)
                new GameObject(n).AddComponent<T>();
        }
    }
}
