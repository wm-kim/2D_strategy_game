using Cysharp.Threading.Tasks;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Minimax.UnityGamingService.Multiplayer
{
    public class GooglePlayAuthentication : MonoBehaviour
    {
        public string GooglePlayToken;
        public string GooglePlayError;

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
                await UnityServices.InitializeAsync();
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
            
            AuthenticationService.Instance.SignedOut += () =>
            {
                DebugWrapper.Log("Player signed out.");
            };
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
                        GooglePlayToken = code;
                        AuthenticateWithUnity(code).Forget();
                    });
                }
                else
                {
                    GooglePlayError = "Failed to retrieve Google play games authorization code";
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
