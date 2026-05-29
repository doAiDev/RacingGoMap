using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RacingGoMap.Auth;
using RacingGoMap.Core;

namespace RacingGoMap.UI
{
    public class LoginUI : MonoBehaviour
    {
        [SerializeField] Button      _guestButton;
        [SerializeField] Button      _memberButton;
        [SerializeField] TMP_Text    _statusText;
        [SerializeField] GameObject  _loadingSpinner;

        void Start()
        {
            _loadingSpinner.SetActive(false);
            _guestButton.onClick.AddListener(OnGuestClick);
            _memberButton.onClick.AddListener(OnMemberClick);
        }

        async void OnGuestClick()
        {
            SetInteractable(false);
            _loadingSpinner.SetActive(true);
            _statusText.text = "접속 중...";

            await AuthManager.Instance.LoginAsGuestAsync();
            SceneLoader.Instance.Load(SceneLoader.Scenes.MainMenu);
        }

        void OnMemberClick()
        {
            _statusText.text = "회원 로그인은 준비 중입니다 :)";
        }

        void SetInteractable(bool v)
        {
            _guestButton.interactable  = v;
            _memberButton.interactable = v;
        }
    }
}
