using System;
using Cysharp.Threading.Tasks;
using Minimax.CoreSystems;
using Minimax.Utilities;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Multiplay;
using UnityEngine;

namespace Minimax.UnityGamingService.Multiplayer
{
    public class DedicatedServerManager : IDisposable
    {
        public string ServerBearerToken { get; private set; }
        private string connectionString => $"{m_serverIP}:{m_serverPort}";
        private string m_serverIP = "0.0.0.0";
        private int m_serverPort = 7777;
        private int m_queryPort = 7787;
        private const int k_multiplayServiceTimeout = 20000;
        private MultiplayAllocationService m_multiplayAllocationService;
        private MultiplayServerQueryService m_multiplayServerQueryService;
        private MatchplayBackfiller m_backfiller;
        
        public DedicatedServerManager()
        {
            m_multiplayAllocationService = new MultiplayAllocationService();
            m_multiplayServerQueryService = new MultiplayServerQueryService();
        }
        
        /// <summary>
        /// Attempts to initialize the server with services
        /// </summary>
        public async UniTask StartGameServerAysnc()
        {
            DebugWrapper.Log("Starting Dedicated Server...");
            
            
            // The server should respond to query requests irrespective of the server being allocated.
            // Hence, start the handler as soon as we can.
            await m_multiplayServerQueryService.BeginServerQueryHandler();

            try
            {
                var matchmakerPayload = await GetMatchmakerPayload(k_multiplayServiceTimeout);
                if (matchmakerPayload != null)
                {
                    DebugWrapper.Log($"Got payload: {matchmakerPayload}.");
                    await StartBackfill(matchmakerPayload);
                }
                else
                {
                    DebugWrapper.Log($"Getting matchmaker payload timed out");
                }
            }
            catch (Exception ex)
            {
                DebugWrapper.LogWarning($"Something went wrong trying to set up the Services:\n{ex} ");
            }
            
            var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            NetworkManager.Singleton.NetworkConfig.NetworkTransport = unityTransport;
            var config = MultiplayService.Instance.ServerConfig; 
            m_serverPort = config.Port;
            m_queryPort = config.QueryPort;
            unityTransport.SetConnectionData(m_serverIP, (ushort)m_serverPort, "0.0.0.0");
            DebugWrapper.Log($"Starting server with: {m_serverIP}:{m_serverPort}.");
             
            GlobalManagers.Instance.Connection.StartServer();

            m_multiplayServerQueryService.SetPlayerCount((ushort)NetworkManager.Singleton.ConnectedClientsIds.Count);
        }

        private async UniTask<MatchmakingResults> GetMatchmakerPayload(int timeout)
        {
            if (m_multiplayAllocationService == null) return null;
            
            // Try to get the matchmaker allocation payload from the multiplay services, and init the services if we do.
            var matchmakerPayloadTask = m_multiplayAllocationService.SubscribeAndAwaitMatchmakerAllocation();
            (bool success, MatchmakingResults payload) = await UniTask.WhenAny( matchmakerPayloadTask, UniTask.Delay(timeout));
            if (success) return payload;
            return null;
        }

        private async UniTask StartBackfill(MatchmakingResults payload)
        {
            m_backfiller = new MatchplayBackfiller(connectionString, payload.QueueName, payload.MatchProperties);

            if (m_backfiller.NeedsPlayers())
            {
                await m_backfiller.BeginBackfilling();
            }
        }

        public void UserJoinedServer(string playerId)
        {
            m_backfiller.AddPlayerToMatch(playerId);
            m_multiplayServerQueryService.AddPlayer();
            if (!m_backfiller.NeedsPlayers() && m_backfiller.Backfilling)
            {
                m_backfiller.StopBackfill().Forget();
            }
        }

        public void UserLeft(string playerId)
        {
            var playerCount = m_backfiller.RemovePlayerFromMatch(playerId);
            m_multiplayServerQueryService.RemovePlayer();
            
            DebugWrapper.Log($"Player {playerId} left. {playerCount} players remaining.");
            // if (playerCount <= 0)
            // {
            //     CloseServer().Forget();
            //     return;
            // }
            
            if (m_backfiller.NeedsPlayers() && !m_backfiller.Backfilling)
            {
                m_backfiller.BeginBackfilling().Forget();
            }
        }

        public async UniTask CloseServer()
        {
            DebugWrapper.Log("Closing server...");
            await m_backfiller.StopBackfill();
            Dispose();
            Application.Quit();
        }
        
        public async UniTask ServerAuthentication()
        {
            var serverAuthResponse = await WebRequestManager.RequestAsync<ServerAuthResponse>(
                "http://localhost:8086/v4/token",
                SendType.GET);
            
            if (String.IsNullOrEmpty(serverAuthResponse.error))
            {
                DebugWrapper.Log("Server Authentication Success");
                ServerBearerToken = serverAuthResponse.token;
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

        public void Dispose()
        {
            m_backfiller?.Dispose();
            m_multiplayAllocationService?.Dispose();
            m_multiplayServerQueryService?.Dispose();
        }
    }
}