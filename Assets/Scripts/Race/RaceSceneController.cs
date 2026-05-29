using UnityEngine;
using RacingGoMap.Car;
using RacingGoMap.Core;
using RacingGoMap.Player;
using RacingGoMap.UI;

namespace RacingGoMap.Race
{
    public class RaceSceneController : MonoBehaviour
    {
        [SerializeField] RaceManager  _raceManager;
        [SerializeField] RaceHUD      _hud;
        [SerializeField] ResultUI     _resultUI;
        [SerializeField] CameraController _camera;
        [SerializeField] GameObject   _carPrefab;
        [SerializeField] CarDatabase  _carDatabase;

        void Start()
        {
            _resultUI.gameObject.SetActive(false);
            _raceManager.OnRaceFinished += ShowResults;

            SpawnLocalCar();
            _raceManager.BeginCountdown();
        }

        void SpawnLocalCar()
        {
            var profile = PlayerDataStore.Instance.Profile;
            var data    = _carDatabase.GetById(profile.equippedCarId) ?? _carDatabase.GetDefault();

            var carObj     = Instantiate(_carPrefab);
            carObj.name    = profile.playerName;

            var controller = carObj.GetComponent<CarController>();
            controller.SetCarData(data);
            controller.IsLocal = true;

            _raceManager.RegisterCar(controller, 0);

            var tracker = carObj.GetComponent<LapTracker>();
            _hud.SetPlayerTracker(tracker);
            _camera.SetTarget(carObj.transform);
        }

        void ShowResults(System.Collections.Generic.List<LapTracker> order)
        {
            _resultUI.gameObject.SetActive(true);
            var localTracker = FindObjectOfType<LapTracker>();
            _resultUI.ShowResults(order, localTracker);
        }
    }
}
