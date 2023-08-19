using Minimax.Utilities;

namespace Minimax.UnityGamingService.Multiplayer.ConnectionManagement
{
    public class StartingServerState : OnlineState
    {
        public StartingServerState(ConnectionManager connectionManager) : base(connectionManager)
        { }

        public override void Enter()
        {
            StartServer();
        }

        public override void Exit() { }
        
        public override void OnServerStarted()
        {
            m_connectionManager.ChangeState(m_connectionManager.Server);
        }

        public override void StartServer()
        {
            if (!m_connectionManager.NetworkManager.StartServer())
            {
                DebugWrapper.Instance.LogError("Failed to start server");
                m_connectionManager.ChangeState(m_connectionManager.Offline);
            }
        }
    }
}