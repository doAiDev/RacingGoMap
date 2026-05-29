using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RacingGoMap.Core
{
    public class SceneLoader : Singleton<SceneLoader>
    {
        public static class Scenes
        {
            public const string Login      = "LoginScene";
            public const string MainMenu   = "MainMenuScene";
            public const string Matchmaking = "MatchmakingScene";
            public const string Garage     = "GarageScene";
            public const string Race       = "RaceScene";
            public const string Result     = "ResultScene";
        }

        public void Load(string sceneName) => StartCoroutine(LoadAsync(sceneName));

        IEnumerator LoadAsync(string sceneName)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            while (!op.isDone) yield return null;
        }
    }
}
