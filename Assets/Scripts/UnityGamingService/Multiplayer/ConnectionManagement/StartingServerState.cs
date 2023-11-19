using Utilities;
using Debug = Utilities.Debug;
#if DEDICATED_SERVER
using System;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Multiplay;
using UnityEngine;
#endif

namespace Minimax.UnityGamingService.Multiplayer.ConnectionManagement
{
    public class StartingServerState : OnlineState
    {
        public StartingServerState(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

        public override async void Enter()
        {
#if !DEDICATED_SERVER
            StartServer();
#else
            await UnityServices.InitializeAsync();
            m_connectionManager.DedicatedServer = new DedicatedServerManager();
            await m_connectionManager.DedicatedServer.ServerAuthentication();
            await m_connectionManager.DedicatedServer.StartGameServerAysnc();
#endif
        }

        public override void Exit()
        {
        }

        public override void OnServerStarted()
        {
            m_connectionManager.ChangeState(m_connectionManager.Server);
        }

        public override void OnServerStopped()
        {
            StartServerFailed();
        }

        public override void StartServer()
        {
            if (!m_connectionManager.NetworkManager.StartServer()) StartServerFailed();
        }

        private void StartServerFailed()
        {
            Debug.LogError("Failed to start server");
            m_connectionManager.ConnectStatusChannel.Publish(ConnectStatus.StartServerFailed);
            m_connectionManager.ChangeState(m_connectionManager.Offline);
            m_connectionManager.ShutDownApplication();
        }
    }
}