using System.Threading.Tasks;
using UnityEngine;
using RacingGoMap.Core;

namespace RacingGoMap.Auth
{
    public class AuthManager : Singleton<AuthManager>
    {
        public bool IsAuthenticated { get; private set; }
        public bool IsGuest { get; private set; }
        public string UserId { get; private set; }

        public async Task LoginAsGuestAsync()
        {
            try
            {
                var initOptions = new Unity.Services.Core.InitializationOptions();
                await Unity.Services.Core.UnityServices.InitializeAsync(initOptions);

                if (!Unity.Services.Authentication.AuthenticationService.Instance.IsSignedIn)
                    await Unity.Services.Authentication.AuthenticationService.Instance.SignInAnonymouslyAsync();

                UserId = Unity.Services.Authentication.AuthenticationService.Instance.PlayerId;
            }
            catch
            {
                // 오프라인 폴백
                UserId = "offline_" + SystemInfo.deviceUniqueIdentifier.Substring(0, 8);
            }

            IsGuest = true;
            IsAuthenticated = true;
        }

        public void Logout()
        {
            try { Unity.Services.Authentication.AuthenticationService.Instance.SignOut(); } catch { }
            IsAuthenticated = false;
            IsGuest = false;
            UserId = null;
        }
    }
}
