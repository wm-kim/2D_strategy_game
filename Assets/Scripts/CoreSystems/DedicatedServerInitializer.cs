using System;
using Cysharp.Threading.Tasks;
using Minimax.CoreSystems;
using Minimax.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace Minimax
{
    public class DedicatedServerInitializer : MonoBehaviour
    {
#if DEDICATED_SERVER
        private async void Start()
        {
            GlobalManagers.Instance.Connection.StartServer();
            await ServerAuthentication();
        }

        private async UniTask ServerAuthentication()
        {
            var serverAuthResponse = await WebRequestManager.RequestAsync<ServerAuthResponse>(
                "http://localhost:8086/v4/token",
                SendType.GET);
            
            if (String.IsNullOrEmpty(serverAuthResponse.error))
            {
                DebugWrapper.Log("Server Authentication Success");
                GlobalManagers.Instance.Connection.ServerBearerToken = serverAuthResponse.token;
            }
            else
            {
                DebugWrapper.LogError(serverAuthResponse.error);
            }
        }
        
        public class ServerAuthResponse
        {
            public string token;
            public string error;
        }
#endif
    }
}
