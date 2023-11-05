using Cysharp.Threading.Tasks;
using ParrelSync;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using Utilities;
using Debug = Utilities.Debug;
#if UNITY_EDITOR
#endif

namespace Minimax.UnityGamingService.Authentication
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
                Debug.Log(UnityServices.State.ToString());
            }
        }

        private void SetupEvents()
        {
            AuthenticationService.Instance.SignedIn += () =>
            {
                // Shows how to get the player ID
                Debug.Log($"Player ID: {AuthenticationService.Instance.PlayerId}");

                // Shows how to get the access token
                Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
            };

            AuthenticationService.Instance.SignInFailed += (exception) =>
            {
                Debug.LogError($"Sign in failed: {exception.Message}");
            };

            AuthenticationService.Instance.SignedOut += () => { Debug.Log("Player signed out."); };
        }

        private async UniTask SignInAnonymously()
        {
            if (AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("Already signed in!");
                return;
            }

            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Sign in anonymously succeeded!");
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
        }
    }
}