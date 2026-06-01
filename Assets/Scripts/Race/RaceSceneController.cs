using UnityEngine;
using RacingGoMap.Car;
using RacingGoMap.Core;
using RacingGoMap.Player;
using RacingGoMap.Network;
using RacingGoMap.UI;

namespace RacingGoMap.Race
{
    public class RaceSceneController : MonoBehaviour
    {
        [Header("씨 구성요소")]
        [SerializeField] RaceManager           _raceManager;
        [SerializeField] ProceduralTrackBuilder _trackBuilder;
        [SerializeField] RaceHUD               _hud;
        [SerializeField] ResultUI              _resultUI;
        [SerializeField] CameraController      _camera;

        [Header("차량")]
        [SerializeField] GameObject _carPrefab;

        void Start()
        {
            _resultUI.gameObject.SetActive(false);

            // 1) 매칭에서 선택된 서킷 로드
            var data = GetSelectedCircuit();

            // 2) 트랙 생성
            _trackBuilder.Build(data);

            // 3) 플레이어 차량 스폰
            SpawnLocalCar();

            // 4) 결승 콜백
            _raceManager.OnRaceFinished += ShowResults;

            // 5) 카운트다운 시작
            _raceManager.BeginCountdown();
        }

        TrackWaypointData GetSelectedCircuit()
        {
            string id = MatchmakingManager.Instance?.SelectedCircuit;
            var    gd = RuntimeGameData.Instance;

            if (!string.IsNullOrEmpty(id) && gd != null)
            {
                var found = gd.GetCircuitById(id);
                if (found != null) return found;
            }

            // 폴백: 랜덤 서킷
            return gd != null ? gd.GetRandomCircuit() : RuntimeGameData.Instance.GetRandomCircuit();
        }

        void SpawnLocalCar()
        {
            var profile = PlayerDataStore.Instance.Profile;
            var gd      = RuntimeGameData.Instance;
            var carData = gd?.GetCarById(profile.equippedCarId) ?? gd?.GetDefaultCar();

            var carObj     = Instantiate(_carPrefab);
            carObj.name    = profile.playerName;

            var controller = carObj.GetComponent<CarController>();
            if (carData != null) controller.SetCarData(carData);
            controller.IsLocal = true;

            // 시작 그리드 위치에 시작
            _raceManager.RegisterCar(controller, _trackBuilder.StartGrid, _trackBuilder.CheckpointCount, 0);

            var tracker = carObj.GetComponent<LapTracker>();
            if (tracker != null) _hud.SetPlayerTracker(tracker);
            _camera.SetTarget(carObj.transform);
        }

        void ShowResults(System.Collections.Generic.List<LapTracker> order)
        {
            _resultUI.gameObject.SetActive(true);
            var local = FindObjectOfType<LapTracker>();
            _resultUI.ShowResults(order, local);
        }
    }
}
