using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RacingGoMap.Car;

namespace RacingGoMap.Race
{
    public class RaceManager : MonoBehaviour
    {
        public static RaceManager Instance { get; private set; }

        [SerializeField] CountdownController _countdown;

        public enum State { Waiting, Countdown, Racing, Finished }
        public State CurrentState { get; private set; } = State.Waiting;

        readonly List<LapTracker>            _trackers    = new();
        readonly Dictionary<LapTracker, int> _finishOrder = new();
        int _finishedCount;

        public event Action<State>            OnStateChanged;
        public event Action<List<LapTracker>> OnRaceFinished;

        void Awake()
        {
            Instance = this;
            _countdown.OnFinished += StartRace;
        }

        /// <summary>
        /// 런타임 트랙에 맞게 차량을 등록합니다.
        /// </summary>
        public void RegisterCar(CarController car, RaceStartGrid grid,
                                int totalCheckpoints, int gridIndex)
        {
            var slot = grid != null ? grid.GetSlot(gridIndex) : null;
            if (slot != null)
                car.transform.SetPositionAndRotation(slot.position, slot.rotation);

            car.SetCanMove(false);

            var tracker = car.GetComponent<LapTracker>();
            if (tracker == null) tracker = car.gameObject.AddComponent<LapTracker>();

            int nonFinishCPs = Mathf.Max(0, totalCheckpoints - 1);
            tracker.Init(nonFinishCPs, 2);
            tracker.OnRaceFinished += () => HandleCarFinished(tracker);
            _trackers.Add(tracker);
        }

        public void BeginCountdown()
        {
            CurrentState = State.Countdown;
            OnStateChanged?.Invoke(CurrentState);
            _countdown.StartCountdown(3);
        }

        void StartRace()
        {
            CurrentState = State.Racing;
            foreach (var t in _trackers)
                t.GetComponent<CarController>()?.SetCanMove(true);
            OnStateChanged?.Invoke(CurrentState);
        }

        void HandleCarFinished(LapTracker t)
        {
            if (_finishOrder.ContainsKey(t)) return;
            _finishedCount++;
            _finishOrder[t] = _finishedCount;

            if (_finishedCount == 1)
            {
                CurrentState = State.Finished;
                var sorted = _trackers
                    .OrderBy(x => _finishOrder.ContainsKey(x) ? _finishOrder[x] : int.MaxValue)
                    .ToList();
                OnStateChanged?.Invoke(CurrentState);
                OnRaceFinished?.Invoke(sorted);
            }
        }

        public int GetLivePosition(LapTracker target)
        {
            if (_finishOrder.TryGetValue(target, out int p)) return p;
            int rank = 1;
            foreach (var t in _trackers)
            {
                if (t == target) continue;
                if (t.CurrentLap > target.CurrentLap ||
                   (t.CurrentLap == target.CurrentLap && t.NextCheckpoint > target.NextCheckpoint))
                    rank++;
            }
            return rank;
        }
    }
}
