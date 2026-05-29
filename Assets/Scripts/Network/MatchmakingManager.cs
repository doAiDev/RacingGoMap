using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using RacingGoMap.Core;
using RacingGoMap.Circuit;

namespace RacingGoMap.Network
{
    public class MatchmakingManager : Singleton<MatchmakingManager>
    {
        public const int   MinPlayers   = 2;
        public const int   MaxPlayers   = 5;
        public const float WaitSeconds  = 10f;

        public enum MatchState { Idle, Searching, Countdown, Starting }

        [Header("Debug")]
        [Tooltip("true일 때 Unity Services 없이 로친으로 매칭 시뮬레이션")]
        [SerializeField] bool _offlineMode = true;
        [Tooltip("오프라인 매칭 시 작동할 다른 플레이어 수 (1리드 + N)")]
        [SerializeField] int _simulatedOtherPlayers = 2;

        public MatchState State           { get; private set; } = MatchState.Idle;
        public int        PlayerCount     { get; private set; }
        public float      CountdownRemain { get; private set; }
        public string     SelectedCircuit { get; private set; }
        public bool       IsHost          { get; private set; }

        public event Action<int>    OnPlayerCountChanged;
        public event Action         OnCountdownStarted;
        public event Action<float>  OnCountdownTick;
        public event Action<string> OnGameReady;
        public event Action         OnSearchCancelled;
        public event Action<string> OnError;

        // Online
        Unity.Services.Lobbies.Models.Lobby _lobby;
        float _pollTimer;
        float _heartbeatTimer;
        float _countdownStartTime;
        bool  _launched;

        // -------------------------------------------------- Public API

        public async Task BeginSearch()
        {
            State    = MatchState.Searching;
            _launched = false;

            if (_offlineMode) { StartCoroutine(SimulateMatchmaking()); return; }

            try
            {
                var queryOpts = new Unity.Services.Lobbies.QueryLobbiesOptions
                {
                    Filters = new List<Unity.Services.Lobbies.Models.QueryFilter>
                    {
                        new Unity.Services.Lobbies.Models.QueryFilter(
                            Unity.Services.Lobbies.Models.QueryFilter.FieldOptions.AvailableSlots,
                            "0",
                            Unity.Services.Lobbies.Models.QueryFilter.OpOptions.GT)
                    }
                };
                var results = await Unity.Services.Lobbies.LobbyService.Instance.QueryLobbiesAsync(queryOpts);

                if (results.Results.Count > 0)
                {
                    _lobby  = await Unity.Services.Lobbies.LobbyService.Instance.JoinLobbyByIdAsync(results.Results[0].Id);
                    IsHost  = false;
                }
                else
                {
                    _lobby  = await Unity.Services.Lobbies.LobbyService.Instance.CreateLobbyAsync(
                        "GoMap_Room", MaxPlayers,
                        new Unity.Services.Lobbies.CreateLobbyOptions { IsPrivate = false });
                    IsHost = true;
                }

                PlayerCount = _lobby.Players.Count;
                OnPlayerCountChanged?.Invoke(PlayerCount);
                _pollTimer = _heartbeatTimer = 0f;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Online matchmaking failed, falling back to offline: {e.Message}");
                _offlineMode = true;
                StartCoroutine(SimulateMatchmaking());
            }
        }

        public async Task CancelSearch()
        {
            StopAllCoroutines();
            State   = MatchState.Idle;
            _launched = false;

            if (!_offlineMode && _lobby != null)
            {
                try
                {
                    if (IsHost)
                        await Unity.Services.Lobbies.LobbyService.Instance.DeleteLobbyAsync(_lobby.Id);
                    else
                    {
                        string pid = Unity.Services.Authentication.AuthenticationService.Instance.PlayerId;
                        await Unity.Services.Lobbies.LobbyService.Instance.RemovePlayerAsync(_lobby.Id, pid);
                    }
                }
                catch { }
                _lobby = null;
            }
            OnSearchCancelled?.Invoke();
        }

        // -------------------------------------------------- Offline simulation

        IEnumerator SimulateMatchmaking()
        {
            // 랜덤하게 2~3초 후 플레이어 등장
            yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 4f));

            int total = Mathf.Clamp(_simulatedOtherPlayers + 1, MinPlayers, MaxPlayers);
            PlayerCount = total;
            OnPlayerCountChanged?.Invoke(PlayerCount);

            State = MatchState.Countdown;
            _countdownStartTime = Time.time;
            CountdownRemain = WaitSeconds;
            SelectedCircuit = GetRandomCircuitId();
            OnCountdownStarted?.Invoke();
        }

        // -------------------------------------------------- Online polling

        void Update()
        {
            if (_offlineMode || _lobby == null || State == MatchState.Idle || _launched) return;

            if (IsHost)
            {
                _heartbeatTimer += Time.deltaTime;
                if (_heartbeatTimer >= 15f)
                {
                    _heartbeatTimer = 0f;
                    Unity.Services.Lobbies.LobbyService.Instance.SendHeartbeatPingAsync(_lobby.Id);
                }
            }

            _pollTimer += Time.deltaTime;
            if (_pollTimer >= 1.5f) { _pollTimer = 0f; PollOnline(); }

            if (State == MatchState.Countdown) TickCountdown();
        }

        async void PollOnline()
        {
            try
            {
                _lobby = await Unity.Services.Lobbies.LobbyService.Instance.GetLobbyAsync(_lobby.Id);

                if (!IsHost && _lobby.Data != null && _lobby.Data.ContainsKey("relayCode"))
                {
                    _launched = true;
                    State = MatchState.Starting;
                    SelectedCircuit = _lobby.Data.ContainsKey("circuitId") ? _lobby.Data["circuitId"].Value : "circuit_01";
                    string code = _lobby.Data["relayCode"].Value;
                    var joinAlloc = await Unity.Services.Relay.RelayService.Instance.JoinAllocationAsync(code);
                    // transport setup 다른 스크립트에서 처리
                    OnGameReady?.Invoke(SelectedCircuit);
                    return;
                }

                int newCount = _lobby.Players.Count;
                if (newCount != PlayerCount) { PlayerCount = newCount; OnPlayerCountChanged?.Invoke(PlayerCount); }

                if (State == MatchState.Searching && PlayerCount >= MinPlayers)
                {
                    State = MatchState.Countdown;
                    _countdownStartTime = Time.time;
                    OnCountdownStarted?.Invoke();
                }

                if (State == MatchState.Countdown) TickCountdown();
            }
            catch (Exception e) { Debug.LogWarning($"Lobby poll: {e.Message}"); }
        }

        void TickCountdown()
        {
            CountdownRemain = WaitSeconds - (Time.time - _countdownStartTime);
            OnCountdownTick?.Invoke(CountdownRemain);

            if (CountdownRemain <= 0f && !_launched)
            {
                _launched = true;
                State = MatchState.Starting;
                if (_offlineMode)
                    OnGameReady?.Invoke(SelectedCircuit);
                else
                    LaunchOnlineGame();
            }
        }

        async void LaunchOnlineGame()
        {
            if (!IsHost) return;
            try
            {
                SelectedCircuit = GetRandomCircuitId();
                var alloc = await Unity.Services.Relay.RelayService.Instance.CreateAllocationAsync(MaxPlayers - 1);
                string code = await Unity.Services.Relay.RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);

                await Unity.Services.Lobbies.LobbyService.Instance.UpdateLobbyAsync(_lobby.Id,
                    new Unity.Services.Lobbies.UpdateLobbyOptions
                    {
                        Data = new Dictionary<string, Unity.Services.Lobbies.Models.DataObject>
                        {
                            { "relayCode", new Unity.Services.Lobbies.Models.DataObject(
                                Unity.Services.Lobbies.Models.DataObject.VisibilityOptions.Public, code) },
                            { "circuitId", new Unity.Services.Lobbies.Models.DataObject(
                                Unity.Services.Lobbies.Models.DataObject.VisibilityOptions.Public, SelectedCircuit) }
                        }
                    });
                OnGameReady?.Invoke(SelectedCircuit);
            }
            catch (Exception e) { OnError?.Invoke($"게임 시작 오류: {e.Message}"); }
        }

        string GetRandomCircuitId()
        {
            var cm = Resources.Load<CircuitManager>("CircuitManager");
            if (cm != null && cm.availableCircuits != null && cm.availableCircuits.Count > 0)
                return cm.GetRandom().circuitId;
            return "circuit_01";
        }
    }
}
