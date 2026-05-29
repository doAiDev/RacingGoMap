using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RacingGoMap.Core;
using RacingGoMap.Network;

namespace RacingGoMap.UI
{
    public class MatchmakingUI : MonoBehaviour
    {
        [SerializeField] TMP_Text   _statusText;
        [SerializeField] TMP_Text   _playerCountText;
        [SerializeField] GameObject _countdownPanel;
        [SerializeField] TMP_Text   _countdownText;
        [SerializeField] Button     _cancelButton;

        // 도트 애니메이션
        [SerializeField] Transform  _dotContainer;
        float _dotTimer;
        int   _dotCount;

        MatchmakingManager _mm;

        async void Start()
        {
            _mm = MatchmakingManager.Instance;
            _countdownPanel.SetActive(false);
            _cancelButton.onClick.AddListener(OnCancel);

            _mm.OnPlayerCountChanged += UpdateCount;
            _mm.OnCountdownStarted   += ShowCountdown;
            _mm.OnCountdownTick      += TickCountdown;
            _mm.OnGameReady          += GoToRace;
            _mm.OnError              += ShowError;

            _statusText.text      = "플레이어를 찾는 중";
            _playerCountText.text = $"0 / {MatchmakingManager.MaxPlayers}";

            await _mm.BeginSearch();
        }

        void OnDestroy()
        {
            if (_mm == null) return;
            _mm.OnPlayerCountChanged -= UpdateCount;
            _mm.OnCountdownStarted   -= ShowCountdown;
            _mm.OnCountdownTick      -= TickCountdown;
            _mm.OnGameReady          -= GoToRace;
            _mm.OnError              -= ShowError;
        }

        void Update()
        {
            // 검색 중 도트 애니메이션
            if (_mm.State != MatchmakingManager.MatchState.Searching) return;
            _dotTimer += Time.deltaTime;
            if (_dotTimer < 0.5f) return;
            _dotTimer = 0f;
            _dotCount = (_dotCount + 1) % 4;
            _statusText.text = "플레이어를 찾는 중" + new string('.', _dotCount);
        }

        void UpdateCount(int count)
        {
            _playerCountText.text = $"{count} / {MatchmakingManager.MaxPlayers}";
            if (count < MatchmakingManager.MinPlayers)
                _statusText.text = "플레이어 대기 중...";
            else
                _statusText.text = $"{count}명 모였습니다! 곳 시작";
        }

        void ShowCountdown() => _countdownPanel.SetActive(true);

        void TickCountdown(float remain)
        {
            int sec = Mathf.CeilToInt(remain);
            _countdownText.text = $"{sec}초 후 시작!";
        }

        void GoToRace(string _) => SceneLoader.Instance.Load(SceneLoader.Scenes.Race);

        void ShowError(string msg) => _statusText.text = $"오류: {msg}";

        async void OnCancel()
        {
            await _mm.CancelSearch();
            SceneLoader.Instance.Load(SceneLoader.Scenes.MainMenu);
        }
    }
}
