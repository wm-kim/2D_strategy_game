using Minimax.Utilities;
using UnityEngine;

namespace Minimax.UnityGamingService.Multiplayer.ConnectionManagement
{
    public class ClientConnectedState : OnlineState
    {
        public ClientConnectedState(ConnectionManager connectionManager) : base(connectionManager) { }
        
        public override void Enter() { }

        public override void Exit() { }
        
        public override void OnClientDisconnect(ulong _)
        {
            var disconnectReason = m_connectionManager.NetworkManager.DisconnectReason;
            DebugWrapper.Log("Client disconnected");
            
            if (string.IsNullOrEmpty(disconnectReason))
            {
                m_connectionManager.ConnectStatusChannel.Publish(ConnectStatus.Reconnecting);
                m_connectionManager.ChangeState(m_connectionManager.ClientReconnecting);
            }
            else
            {
                var connectStatus = JsonUtility.FromJson<ConnectStatus>(disconnectReason);
                m_connectionManager.ConnectStatusChannel.Publish(connectStatus);
                m_connectionManager.ChangeState(m_connectionManager.Offline);
            }
        }
    }
   
}