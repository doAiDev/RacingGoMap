using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RacingGoMap.Core;
using RacingGoMap.Player;
using RacingGoMap.Race;

namespace RacingGoMap.UI
{
    public class ResultUI : MonoBehaviour
    {
        [SerializeField] Transform  _listContent;
        [SerializeField] GameObject _resultItemPrefab;
        [SerializeField] TMP_Text   _pointsEarnedText;
        [SerializeField] Button     _playAgainButton;
        [SerializeField] Button     _mainMenuButton;

        static readonly int[] Rewards = { 100, 60, 40, 20, 10 };

        void Start()
        {
            _playAgainButton.onClick.AddListener(() => SceneLoader.Instance.Load(SceneLoader.Scenes.Matchmaking));
            _mainMenuButton.onClick.AddListener(()  => SceneLoader.Instance.Load(SceneLoader.Scenes.MainMenu));
        }

        public void ShowResults(List<LapTracker> order, LapTracker localPlayer)
        {
            foreach (Transform c in _listContent) Destroy(c.gameObject);

            int localPos = -1;
            for (int i = 0; i < order.Count; i++)
            {
                var item  = Instantiate(_resultItemPrefab, _listContent);
                var texts = item.GetComponentsInChildren<TMP_Text>();
                if (texts.Length >= 2)
                {
                    texts[0].text = $"{i + 1}위";
                    texts[1].text = order[i].gameObject.name;
                }
                if (order[i] == localPlayer) localPos = i;
            }

            if (localPos >= 0)
            {
                int reward = localPos < Rewards.Length ? Rewards[localPos] : 5;
                PlayerDataStore.Instance.AddPoints(reward);
                _pointsEarnedText.text = $"+{reward} P 획득!";
            }
        }
    }
}
