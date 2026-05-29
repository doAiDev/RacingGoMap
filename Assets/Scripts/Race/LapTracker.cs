using System;
using UnityEngine;

namespace RacingGoMap.Race
{
    public class LapTracker : MonoBehaviour
    {
        public int   TotalLaps        { get; private set; } = 2;
        public int   CurrentLap       { get; private set; } = 1;
        public int   NextCheckpoint   { get; private set; } = 0;
        public bool  Finished         { get; private set; }
        public float CurrentLapTime   { get; private set; }
        public float BestLapTime      { get; private set; } = float.MaxValue;
        public float TotalRaceTime    { get; private set; }

        int   _totalCheckpoints;
        float _lapStartTime;
        float _raceStartTime;

        public event Action<int> OnLapCompleted;
        public event Action      OnRaceFinished;

        public void Init(int checkpoints, int laps)
        {
            _totalCheckpoints = checkpoints;
            TotalLaps         = laps;
            _lapStartTime     = Time.time;
            _raceStartTime    = Time.time;
        }

        void Update()
        {
            if (Finished) return;
            CurrentLapTime = Time.time - _lapStartTime;
            TotalRaceTime  = Time.time - _raceStartTime;
        }

        public void OnCheckpointReached(int index, bool isFinish)
        {
            if (Finished) return;

            if (isFinish)
            {
                // 시작 전 통과 무시 (0번 체크포인트를 알아야 라프 완성)
                if (_totalCheckpoints > 0 && NextCheckpoint < _totalCheckpoints) return;

                float lapTime = Time.time - _lapStartTime;
                if (lapTime < BestLapTime) BestLapTime = lapTime;
                _lapStartTime   = Time.time;
                NextCheckpoint  = 0;

                OnLapCompleted?.Invoke(CurrentLap);

                if (CurrentLap >= TotalLaps)
                {
                    Finished = true;
                    OnRaceFinished?.Invoke();
                }
                else
                {
                    CurrentLap++;
                }
            }
            else
            {
                if (index == NextCheckpoint)
                    NextCheckpoint = index + 1;
            }
        }
    }
}
