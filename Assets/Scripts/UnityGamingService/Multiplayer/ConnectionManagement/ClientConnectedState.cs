using Minimax.CoreSystems;
using Minimax.SceneManagement;
using Minimax.UI.View.Popups;
using UnityEngine;
using Utilities;
using Debug = Utilities.Debug;

namespace Minimax.UnityGamingService.Multiplayer.ConnectionManagement
{
    public class ClientConnectedState : OnlineState
    {
        public ClientConnectedState(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override void OnPlayerRequestedShutdown()
        {
            m_connectionManager.PlayerRequestShutdownServerRpc();
        }

        public override void OnClientDisconnect(ulong clientId)
        {
            var disconnectReason = m_connectionManager.NetworkManager.DisconnectReason;
            Debug.Log($"Client {clientId} disconnected");

            if (string.IsNullOrEmpty(disconnectReason))
            {
                m_connectionManager.ConnectStatusChannel.Publish(ConnectStatus.Reconnecting);
                m_connectionManager.ChangeState(m_connectionManager.ClientReconnecting);
            }
            else
            {
                var connectStatus = JsonUtility.FromJson<ConnectStatus>(disconnectReason);
                m_connectionManager.ConnectStatusChannel.Publish(connectStatus);

                Debug.Log($"Disconnected reason: {connectStatus.ToString()}");
                m_connectionManager.ChangeState(m_connectionManager.Offline);
            }
        }
    }
}