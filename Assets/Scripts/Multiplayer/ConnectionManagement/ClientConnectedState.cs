namespace Minimax.Multiplayer.ConnectionManagement
{
    public class ClientConnectedState : ConnectionState
    {
        public ClientConnectedState(ConnectionManager connectionManager) : base(connectionManager) { }
        
        public override void Enter() { }

        public override void Exit() { }
        
        public override void OnClientDisconnect(ulong clientId)
        {
            var disconnectReason = m_connectionManager.NetworkManager.DisconnectReason;
            DebugWrapper.Log("Client disconnected: " + disconnectReason);
            m_connectionManager.ChangeState(m_connectionManager.Offline);
        }
    }
   
}