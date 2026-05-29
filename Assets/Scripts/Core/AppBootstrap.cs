using UnityEngine;
using RacingGoMap.Auth;
using RacingGoMap.Player;
using RacingGoMap.Network;

namespace RacingGoMap.Core
{
    [DefaultExecutionOrder(-100)]
    public class AppBootstrap : MonoBehaviour
    {
        void Awake()
        {
            EnsureExists<SceneLoader>("SceneLoader");
            EnsureExists<AuthManager>("AuthManager");
            EnsureExists<PlayerDataStore>("PlayerDataStore");
            EnsureExists<MatchmakingManager>("MatchmakingManager");
        }

        static void EnsureExists<T>(string objName) where T : MonoBehaviour
        {
            if (FindObjectOfType<T>() == null)
                new GameObject(objName).AddComponent<T>();
        }
    }
}
