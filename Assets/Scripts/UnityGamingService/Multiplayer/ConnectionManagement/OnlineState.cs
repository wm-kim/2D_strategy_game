namespace Minimax.UnityGamingService.Multiplayer.ConnectionManagement
{
    public abstract class OnlineState : ConnectionState
    {
        protected OnlineState(ConnectionManager connectionManager) : base(connectionManager)
        { }
        
        public override void OnUserRequestedShutdown()
        {
            // This behaviour will be the same for every online state
            m_connectionManager.ChangeState(m_connectionManager.Offline);
        }
        
        public override void OnTransportFailure()
        {
            // This behaviour will be the same for every online state
            m_connectionManager.ChangeState(m_connectionManager.Offline);
        }
    }
}