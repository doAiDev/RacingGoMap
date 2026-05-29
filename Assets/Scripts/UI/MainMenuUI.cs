using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RacingGoMap.Core;
using RacingGoMap.Player;

namespace RacingGoMap.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] Button   _findRoomBtn;
        [SerializeField] Button   _garageBtn;
        [SerializeField] TMP_Text _playerNameText;
        [SerializeField] TMP_Text _pointsText;

        void Start()
        {
            _findRoomBtn.onClick.AddListener(() => SceneLoader.Instance.Load(SceneLoader.Scenes.Matchmaking));
            _garageBtn.onClick.AddListener(()   => SceneLoader.Instance.Load(SceneLoader.Scenes.Garage));

            var p = PlayerDataStore.Instance.Profile;
            _playerNameText.text = p.playerName;
            _pointsText.text     = $"{p.totalPoints:N0} P";
        }
    }
}
