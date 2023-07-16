using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using QFSW.QC;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace WMK
{
    public class Authentication : MonoBehaviour
    {
        private async void Start()
        {
            await UnityServices.InitializeAsync();
            DebugStatic.Log(UnityServices.State.ToString());
            
            SetupEvents();
            
            await SignInAnonymouslyAsync();
        }

        private void SetupEvents()
        {
            AuthenticationService.Instance.SignedIn += () =>
            {
                // Shows how to get the player ID
                DebugStatic.Log($"Player ID: {AuthenticationService.Instance.PlayerId}");

                // Shows how to get the access token
                DebugStatic.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
            };
            
            AuthenticationService.Instance.SignInFailed += (exception) =>
            {
                DebugStatic.LogError($"Sign in failed: {exception.Message}");
            };
        }
        
        private async UniTask SignInAnonymouslyAsync()
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Sign in anonymously succeeded!");
        
                // Shows how to get the playerID
                Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}"); 

            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                DebugStatic.LogException(ex);
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
