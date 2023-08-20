using Minimax.Utilities;

namespace Minimax.UnityGamingService.Multiplayer.ConnectionManagement
{
    public class ClientConnectedState : OnlineState
    {
        public ClientConnectedState(ConnectionManager connectionManager) : base(connectionManager) { }
        
        public override void Enter() { }

        public override void Exit() { }
        
        public override void OnClientDisconnect(ulong clientId)
        {
            var disconnectReason = m_connectionManager.NetworkManager.DisconnectReason;
            DebugWrapper.Log("Client disconnected: " + disconnectReason);
            if (string.IsNullOrEmpty(disconnectReason))
            {
                m_connectionManager.ChangeState(m_connectionManager.ClientReconnecting);
            }
            else
            {
                m_connectionManager.ChangeState(m_connectionManager.Offline);
            }
        }
    }
   
}