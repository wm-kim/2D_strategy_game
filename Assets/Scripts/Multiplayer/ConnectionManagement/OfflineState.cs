namespace Minimax.Multiplayer.ConnectionManagement
{
    public class OfflineState : ConnectionState
    {
        public OfflineState(ConnectionManager connectionManager) : base(connectionManager) { }
        
        public override void Enter()
        {
            m_connectionManager.NetworkManager.Shutdown();
        }

        public override void Exit() { }
        
        public override void StartClient()
        {
            m_connectionManager.ChangeState(m_connectionManager.ClientConnecting);
        }
        
        public override void StartHost()
        {
            m_connectionManager.ChangeState(m_connectionManager.StartingHost);
        }
    }
}