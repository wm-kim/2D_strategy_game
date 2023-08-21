using Unity.Netcode;

namespace Minimax.UnityGamingService.Multiplayer.ConnectionManagement
{
    public abstract class ConnectionState
    {
        protected ConnectionManager m_connectionManager;
        protected ConnectionState(ConnectionManager connectionManager) => m_connectionManager = connectionManager;
        
        public abstract void Enter();

        public abstract void Exit();
        
        public virtual void OnClientConnected(ulong clientId) { }
        
        public virtual void OnClientDisconnect(ulong clientId) { }
        
        public virtual void OnServerStarted() { }
        
        public virtual void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) { }
        
        public virtual void OnTransportFailure() { }
        
        public virtual void OnUserRequestedShutdown() { }
        
        public virtual void StartClient() { }
        
        public virtual void StartHost() { }
        
        public virtual void StartServer() { }
        public virtual void OnServerStopped() { }
    }
}