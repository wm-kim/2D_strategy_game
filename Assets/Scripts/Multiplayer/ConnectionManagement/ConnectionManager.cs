using System;
using Unity.Netcode;
using UnityEngine;

namespace Minimax.Multiplayer.ConnectionManagement
{
    public enum ConnectStatus
    {
        Undefined,
        Success,
        ServerFull,     
        LoggedInAgain,
    }
    
    [Serializable]
    public class ConnectionPayload
    {
        public string playerId;
        public string playerName;
    }
    
    public class ConnectionManager : MonoBehaviour
    {
        private ConnectionState m_currentState;
        public NetworkManager NetworkManager => NetworkManager.Singleton;

        public int MaxConnectedPlayers { get; private set; } = 2;

        internal OfflineState Offline;
        internal StartingHostState StartingHost;
        internal HostingState Hosting;
        internal ClientConnectingState ClientConnecting;
        internal ClientConnectedState ClientConnected;

        private void Awake()
        {
            Offline = new OfflineState(this);
            StartingHost = new StartingHostState(this);
            Hosting = new HostingState(this);
            ClientConnecting = new ClientConnectingState(this);
            ClientConnected = new ClientConnectedState(this);
        }

        private void Start()
        {
            NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
            NetworkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
            NetworkManager.OnServerStarted += OnServerStarted;
            NetworkManager.ConnectionApprovalCallback += ApprovalCheck;
        }

        private void OnDestroy()
        {
            if (NetworkManager != null)
            {
                NetworkManager.OnClientConnectedCallback -= OnClientConnectedCallback;
                NetworkManager.OnClientDisconnectCallback -= OnClientDisconnectCallback;
                NetworkManager.OnServerStarted -= OnServerStarted;
                NetworkManager.ConnectionApprovalCallback -= ApprovalCheck;
            }
        }

        public void ChangeState(ConnectionState nextState)
        {
            DebugWrapper.Log(
                $"{name}: Changed connection state from {m_currentState.GetType().Name} to {nextState.GetType().Name}.");

            if (m_currentState != null)
            {
                m_currentState.Exit();
            }

            m_currentState = nextState;
            m_currentState.Enter();
        }

        private void OnClientConnectedCallback(ulong clientId)
        {
            m_currentState.OnClientConnected(clientId);
        }

        private void OnClientDisconnectCallback(ulong clientId)
        {
            m_currentState.OnClientDisconnect(clientId);
        }

        private void OnServerStarted()
        {
            m_currentState.OnServerStarted();
        }

        private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response)
        {
            m_currentState.ApprovalCheck(request, response);
        }
        
        public void StartClient()
        {
            m_currentState.StartClient();
        }
        
        public void StartHost()
        {
            m_currentState.StartHost();
        }
    }
}
