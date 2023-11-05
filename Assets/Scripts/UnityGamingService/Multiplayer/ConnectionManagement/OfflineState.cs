namespace Minimax.UnityGamingService.Multiplayer.ConnectionManagement
{
    public class OfflineState : ConnectionState
    {
        public OfflineState(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

        public override void Enter()
        {
            if (m_connectionManager != null && m_connectionManager.NetworkManager != null)
                m_connectionManager.NetworkManager.Shutdown();
        }

        public override void Exit()
        {
        }

        public override void StartClient()
        {
            m_connectionManager.ChangeState(m_connectionManager.ClientConnecting);
        }

        public override void StartHost()
        {
            m_connectionManager.ChangeState(m_connectionManager.StartingHost);
        }

        public override void StartServer()
        {
            m_connectionManager.ChangeState(m_connectionManager.StartingServer);
        }
    }
}