using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Minimax.Multiplayer.ConnectionManagement
{
    public class ClientConnectingState : ConnectionState
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

        private void ConnectClient()
        {
            var payload = JsonUtility.ToJson(new ConnectionPayload()
            {
                playerId = AuthenticationService.Instance.PlayerId,
                playerName = "PlayerName_" + Random.Range(0, 1000)
            });
            var payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);
            m_connectionManager.NetworkManager.NetworkConfig.ConnectionData = payloadBytes;
            
            if (!m_connectionManager.NetworkManager.StartClient())
            {
                DebugWrapper.LogError("Failed to start client");
                m_connectionManager.ChangeState(m_connectionManager.Offline);
            }
        }
    }
    
}