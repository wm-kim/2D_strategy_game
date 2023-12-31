using System;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using Utilities;
using Debug = Utilities.Debug;
using Random = UnityEngine.Random;

namespace Minimax.UnityGamingService.Multiplayer.ConnectionManagement
{
    public class ClientConnectingState : OnlineState
    {
        public ClientConnectingState(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

        public override void Enter()
        {
            ConnectClient();
        }

        public override void Exit()
        {
        }

        public override void OnClientConnected(ulong _)
        {
            m_connectionManager.ConnectStatusChannel.Publish(ConnectStatus.Success);
            m_connectionManager.ChangeState(m_connectionManager.ClientConnected);
        }

        public override void OnClientDisconnect(ulong clientId)
        {
            StartingClientFailed();
        }

        protected void ConnectClient()
        {
            Debug.Log("Connecting client...");

            var payload = JsonUtility.ToJson(new ConnectionPayload()
            {
                playerId   = AuthenticationService.Instance.PlayerId,
                playerName = "PlayerName_" + Random.Range(0, 1000)
            });
            var payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);
            m_connectionManager.NetworkManager.NetworkConfig.ConnectionData = payloadBytes;

            try
            {
                if (!m_connectionManager.NetworkManager.StartClient()) StartingClientFailed();
            }
            catch (Exception e)
            {
                Debug.LogError("Error connecting client, see following exception");
                Debug.LogException(e);
                throw;
            }
        }

        private void StartingClientFailed()
        {
            var disconnectReason = m_connectionManager.NetworkManager.DisconnectReason;
            Debug.Log("Client disconnected");

            if (string.IsNullOrEmpty(disconnectReason))
            {
                m_connectionManager.ConnectStatusChannel.Publish(ConnectStatus.StartClientFailed);
            }
            else
            {
                var connectStatus = JsonUtility.FromJson<ConnectStatus>(disconnectReason);
                m_connectionManager.ConnectStatusChannel.Publish(connectStatus);
            }

            m_connectionManager.ChangeState(m_connectionManager.Offline);
        }
    }
}