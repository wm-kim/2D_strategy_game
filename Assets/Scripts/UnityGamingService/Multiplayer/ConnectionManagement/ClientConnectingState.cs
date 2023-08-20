using System;
using Minimax.Utilities;
using Unity.Services.Authentication;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Minimax.UnityGamingService.Multiplayer.ConnectionManagement
{
    public class ClientConnectingState : OnlineState
    {
        public ClientConnectingState(ConnectionManager connectionManager) : base(connectionManager) { }

        public override void Enter()
        {
            ConnectClient();
        }

        public override void Exit() { }

        public override void OnClientConnected(ulong clientId)
        {
            m_connectionManager.ChangeState(m_connectionManager.ClientConnected);
        }
        
        public override void OnClientDisconnect(ulong clientId)
        {
            var disconnectReason = m_connectionManager.NetworkManager.DisconnectReason;
            DebugWrapper.Log("Client disconnected: " + disconnectReason);
            m_connectionManager.ChangeState(m_connectionManager.Offline);
        }

        protected void ConnectClient()
        {
            var payload = JsonUtility.ToJson(new ConnectionPayload()
            {
                playerId = AuthenticationService.Instance.PlayerId,
                playerName = "PlayerName_" + Random.Range(0, 1000)
            });
            var payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);
            m_connectionManager.NetworkManager.NetworkConfig.ConnectionData = payloadBytes;

            try
            {
                if (!m_connectionManager.NetworkManager.StartClient())
                {
                    DebugWrapper.LogError("NetworkManager Failed to start client");
                    m_connectionManager.ChangeState(m_connectionManager.Offline);
                }
            }
            catch (Exception e)
            {
                DebugWrapper.LogError("Error connecting client, see following exception");
                DebugWrapper.LogException(e);
                throw;
            }
        }
    }
    
}