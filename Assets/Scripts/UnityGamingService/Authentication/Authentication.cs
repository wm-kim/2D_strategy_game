using Cysharp.Threading.Tasks;
using Minimax.Utilities;
#if UNITY_EDITOR
using ParrelSync;
#endif
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

namespace Minimax.UnityGamingService.Multiplayer
{
    public enum EnvironmentType
    {
        // lower case to match Unity's environment names
        undefined,
        development,
        production
    }

    public class Authentication : MonoBehaviour
    {
        [SerializeField] private EnvironmentType m_environment = EnvironmentType.undefined;

#if !DEDICATED_SERVER
        private async void Start()
        {
            await InitializeUnityAuthentication();
            SetupEvents();
            await SignInAnonymously();
        }
#endif

        private async UniTask InitializeUnityAuthentication()
        {
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

#if UNITY_EDITOR
                if (ClonesManager.IsClone())
                {
                    // Get the custom argument for this clone project.  
                    var customArgument = ClonesManager.GetArgument();
                    options.SetProfile(customArgument);
                }
#endif
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

        private async UniTask SignInAnonymously()
        {
            if (AuthenticationService.Instance.IsSignedIn)
            {
                DebugWrapper.Log("Already signed in!");
                return;
            }

            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                DebugWrapper.Log("Sign in anonymously succeeded!");
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                DebugWrapper.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                DebugWrapper.LogException(ex);
            }
        }
    }
}