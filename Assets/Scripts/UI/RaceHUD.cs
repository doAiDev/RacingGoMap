using UnityEngine;
using TMPro;
using RacingGoMap.Race;

namespace RacingGoMap.UI
{
    public class RaceHUD : MonoBehaviour
    {
        [SerializeField] GameObject _countdownPanel;
        [SerializeField] TMP_Text   _countdownText;
        [SerializeField] TMP_Text   _lapText;
        [SerializeField] TMP_Text   _positionText;
        [SerializeField] TMP_Text   _lapTimeText;

        LapTracker   _tracker;
        RaceManager  _race;

        void Start()
        {
            _race = RaceManager.Instance;
            _race.OnStateChanged += OnStateChanged;

            var cd = FindObjectOfType<CountdownController>();
            if (cd != null)
            {
                cd.OnTick     += n => ShowCountdown(n.ToString());
                cd.OnFinished += () => { ShowCountdown("GO!"); Invoke(nameof(HideCountdown), 0.7f); };
            }

            _countdownPanel.SetActive(false);
        }

        public void SetPlayerTracker(LapTracker t) => _tracker = t;

        void OnStateChanged(RaceManager.State s)
        {
            if (s == RaceManager.State.Racing) HideCountdown();
        }

        void ShowCountdown(string s)
        {
            _countdownPanel.SetActive(true);
            _countdownText.text = s;
        }

        void HideCountdown() => _countdownPanel.SetActive(false);

        void Update()
        {
            if (_tracker == null || _race == null) return;
            if (_race.CurrentState != RaceManager.State.Racing) return;

            _lapText.text      = $"LAP {_tracker.CurrentLap} / {_tracker.TotalLaps}";
            _positionText.text = $"P{_race.GetLivePosition(_tracker)}";
            _lapTimeText.text  = FormatTime(_tracker.CurrentLapTime);
        }

        static string FormatTime(float t) => $"{(int)t / 60:00}:{t % 60:00.00}";
    }
}
