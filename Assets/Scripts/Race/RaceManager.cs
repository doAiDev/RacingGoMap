using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RacingGoMap.Car;
using RacingGoMap.Circuit;

namespace RacingGoMap.Race
{
    public class RaceManager : MonoBehaviour
    {
        public static RaceManager Instance { get; private set; }

        [SerializeField] RaceStartGrid      _grid;
        [SerializeField] CountdownController _countdown;
        [SerializeField] CircuitData         _circuitData;

        public enum State { Waiting, Countdown, Racing, Finished }
        public State CurrentState { get; private set; } = State.Waiting;

        readonly List<LapTracker>             _trackers    = new();
        readonly Dictionary<LapTracker, int>  _finishOrder = new();
        int _finishedCount;

        public event Action<State>            OnStateChanged;
        public event Action<List<LapTracker>> OnRaceFinished;

        void Awake()
        {
            Instance = this;
            _countdown.OnTick     += _ => { /* HUD 참조 */ };
            _countdown.OnFinished += StartRace;
        }

        public void RegisterCar(CarController car, int gridIndex)
        {
            var slot = _grid.GetSlot(gridIndex);
            car.transform.SetPositionAndRotation(slot.position, slot.rotation);
            car.SetCanMove(false);

            var tracker = car.GetComponent<LapTracker>();
            if (tracker == null) tracker = car.gameObject.AddComponent<LapTracker>();

            var checkpoints = FindObjectsOfType<Checkpoint>();
            int cpCount     = checkpoints.Count(c => !c.IsFinishLine);
            int laps        = _circuitData != null ? _circuitData.totalLaps : 2;
            tracker.Init(cpCount, laps);
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

        void HandleCarFinished(LapTracker tracker)
        {
            if (_finishOrder.ContainsKey(tracker)) return;
            _finishedCount++;
            _finishOrder[tracker] = _finishedCount;

            if (_finishedCount == 1)
            {
                // 첫 번째 완주자 나오자마자 종료
                CurrentState = State.Finished;
                var sorted = _trackers
                    .OrderBy(t => _finishOrder.ContainsKey(t) ? _finishOrder[t] : int.MaxValue)
                    .ToList();
                OnStateChanged?.Invoke(CurrentState);
                OnRaceFinished?.Invoke(sorted);
            }
        }

        public int GetLivePosition(LapTracker target)
        {
            if (_finishOrder.TryGetValue(target, out int pos)) return pos;
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
