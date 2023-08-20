using System;
using System.Collections.Generic;
using Minimax.CoreSystems;
using Minimax.ScriptableObjects.Events;
using Minimax.Utilities;
using Minimax.Utilities.PubSub;
using Unity.Multiplayer.Samples.BossRoom;
using Unity.Netcode;
using UnityEngine;

namespace Minimax.UnityGamingService.Multiplayer.ConnectionManagement
{
    public enum ConnectStatus
    {
        Undefined,
        Success,
        ServerFull,                 // can't join, server is already at capacity.
        LoggedInAgain,              // logged in on a separate client, causing this one to be kicked out.
        UserRequestedDisconnect,    // Intentional Disconnect triggered by the user.
    }
    
    [Serializable]
    public class ConnectionPayload
    {
        public string playerId;
        public string playerName;
    }
    
    public class ConnectionManager : MonoBehaviour
    {
        public NetworkManager NetworkManager => NetworkManager.Singleton;
        public IMessageChannel<ConnectStatus> ConnectStatusChannel { get; private set; } = new MessageChannel<ConnectStatus>();
        
        private ConnectionState m_currentState;
        internal OfflineState Offline;
        internal StartingHostState StartingHost;
        internal HostingState Hosting;
        internal StartingServerState StartingServer;
        internal ServerState Server;
        internal ClientConnectingState ClientConnecting;
        internal ClientReconnectingState ClientReconnecting;
        internal ClientConnectedState ClientConnected;
        private Dictionary<ulong, ClientRpcParams> m_clientRpcParams = new Dictionary<ulong, ClientRpcParams>();
        
        private void Awake()
        {
            Offline = new OfflineState(this);
            StartingHost = new StartingHostState(this);
            Hosting = new HostingState(this);
            StartingServer = new StartingServerState(this);
            Server = new ServerState(this);
            ClientConnecting = new ClientConnectingState(this);
            ClientReconnecting = new ClientReconnectingState(this);
            ClientConnected = new ClientConnectedState(this);
        }

        private void Start()
        {
            m_currentState = Offline;
            NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
            NetworkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
            NetworkManager.OnServerStarted += OnServerStarted;
            NetworkManager.OnTransportFailure += OnTransportFailure;
            NetworkManager.ConnectionApprovalCallback += ApprovalCheck;
        }

        private void OnDestroy()
        {
            if (NetworkManager != null)
            {
                NetworkManager.OnClientConnectedCallback -= OnClientConnectedCallback;
                NetworkManager.OnClientDisconnectCallback -= OnClientDisconnectCallback;
                NetworkManager.OnServerStarted -= OnServerStarted;
                NetworkManager.OnTransportFailure += OnTransportFailure;
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
        
        void OnTransportFailure()
        {
            m_currentState.OnTransportFailure();
        }
        
        public void StartClient()
        {
            m_currentState.StartClient();
        }
        
        public void StartHost()
        {
            m_currentState.StartHost();
        }
        
        public void StartServer()
        {
            m_currentState.StartServer();
        }
        
        public void RequestShutdown()
        {
            m_currentState.OnUserRequestedShutdown();
        }

        /// <summary>
        /// Returns true if there is a player slot available for a new player to join the game.
        /// </summary>
        public bool HasAvailablePlayerSlot() => NetworkManager.ConnectedClientsIds.Count < Define.MaxConnectedPlayers;
        
        /// <summary>
        /// Returns true if there are no players connected to the game.
        /// </summary>
        public bool HasNoPlayerConnected() => NetworkManager.ConnectedClientsIds.Count <= 0;
        
        public ConnectStatus GetConnectStatus(ConnectionPayload connectionPayload)
        {
            if (!HasAvailablePlayerSlot())
            {
                return ConnectStatus.ServerFull;
            }
            
            return SessionManager<SessionPlayerData>.Instance.IsDuplicateConnection(connectionPayload.playerId) ?
                ConnectStatus.LoggedInAgain : ConnectStatus.Success;
        }
        
        /// <summary>
        /// Cache client rpc params for all connected clients, to avoid creating new params every time.
        /// </summary>
        public void CacheClientRpcParams()
        {
            var connectionIds = NetworkManager.ConnectedClientsIds;
            foreach (var clientId in connectionIds)
            {
                m_clientRpcParams.Add(clientId, new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { clientId } 
                    }
                });
            }
        }
        
        public ClientRpcParams GetClientRpcParams(ulong clientId)
        {
            return m_clientRpcParams[clientId];
        }
    }
}
