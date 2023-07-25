using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

#if UNITY_EDITOR
using ParrelSync;
#endif

namespace Minimax
{
    public class Authentication : MonoBehaviour
    {
        private async void Start()
        {
            InitializeUnityAuthentication();
            SetupEvents();
            await SignInAnonymouslyAsync();
        }

        private async void InitializeUnityAuthentication()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                var options = new InitializationOptions();

#if !DEDICATED_SERVER
            // Set the profile to a random number to separate players
            options.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());
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
        }
        
        private async UniTask SignInAnonymouslyAsync()
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
