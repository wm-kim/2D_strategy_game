#if !DEDICATED_SERVER
using Cysharp.Threading.Tasks;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Minimax.PropertyDrawer;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using Utilities;

namespace Minimax.UnityGamingService.Authentication
{
    public class GooglePlayAuthentication : MonoBehaviour
    {
        [SerializeField] [ReadOnly] private string m_googlePlayToken;
        [SerializeField] [ReadOnly] private string m_googlePlayError;

        [SerializeField] private EnvironmentType m_environment = EnvironmentType.undefined;

        private async void Start()
        {
            await InitializeAuthentication();
            SetupEvents();
            await GooglePlayAuthenticate();
        }

        private async UniTask InitializeAuthentication()
        {
            //PlayGamesPlatform 활성화
            PlayGamesPlatform.Activate();
            //디버깅에 권장됨
            PlayGamesPlatform.DebugLogEnabled = true;

            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                var options = new InitializationOptions();

                // Set the environment
                switch (m_environment)
                {
                    case EnvironmentType.development:
                        options.SetEnvironmentName(EnvironmentType.development.ToString());
                        break;
                    case EnvironmentType.production:
                        options.SetEnvironmentName(EnvironmentType.production.ToString());
                        break;
                }

                await UnityServices.InitializeAsync(options);
                DebugWrapper.Log(UnityServices.State.ToString());
            }
        }

        private void SetupEvents()
        {
            AuthenticationService.Instance.SignedIn += () =>
            {
                // Shows how to get the player ID
                DebugWrapper.Log($"Player ID: {AuthenticationService.Instance.PlayerId}");

                // Shows how to get the access token
                DebugWrapper.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
            };

            AuthenticationService.Instance.SignInFailed += (exception) =>
            {
                DebugWrapper.LogError($"Sign in failed: {exception.Message}");
            };

            AuthenticationService.Instance.SignedOut += () => { DebugWrapper.Log("Player signed out."); };
        }


        private async UniTask GooglePlayAuthenticate()
        {
            // This connects to the Google Play Games platform
            PlayGamesPlatform.Instance.Authenticate((success) =>
            {
                if (success == SignInStatus.Success)
                {
                    DebugWrapper.Log("Login with Google Play games successful.");
                    PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                    {
                        DebugWrapper.Log("Authorization code: " + code);
                        // This token serves as an example to be used for SignInWithGooglePlayGames
                        m_googlePlayToken = code;
                        AuthenticateWithUnity(code).Forget();
                    });
                }
                else
                {
                    m_googlePlayError = "Failed to retrieve Google play games authorization code";
                    DebugWrapper.Log("Login Unsuccessful");
                }
            });
        }

        private async UniTask AuthenticateWithUnity(string authCode)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(authCode);
                DebugWrapper.Log("Login with Unity successful.");
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                DebugWrapper.LogException(ex);
                throw;
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                DebugWrapper.LogException(ex);
                throw;
            }
        }
    }
}
#endif