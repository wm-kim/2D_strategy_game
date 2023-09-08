using System;
using System.Collections.Generic;
using Minimax.CoreSystems;
using Minimax.SceneManagement;
using Minimax.ScriptableObjects.Events;
using Minimax.Utilities;
using Minimax.Utilities.PubSub;
using Unity.Netcode;
using Unity.Services.Matchmaker;
using UnityEngine;

namespace Minimax.UnityGamingService.Multiplayer.ConnectionManagement
{
    public enum ConnectStatus
    {
        Undefined,
        Success,
        ServerFull,                 // can't join, server is already at capacity.
        LoggedInAgain,              // logged in on a separate client, causing this one to be kicked out.
        Reconnecting,               // client lost connection and is attempting to reconnect.
        UserRequestedDisconnect,    // Intentional Disconnect triggered by the user.
        GenericDisconnect,          // server disconnected, but no specific reason given.
        StartHostFailed,            // host failed to bind
        StartServerFailed,          // server failed to bind
        StartClientFailed,          // failed to connect to server and/or invalid network endpoint
        ServerEndedSession,         // server intentionally ended the session.
    }
    
    [Serializable]
    public class ConnectionPayload
    {
        public string playerId;
        public string playerName;
    }
    
    [Serializable]
    public class PayloadAllocation
    {
        public Unity.Services.Matchmaker.Models.MatchProperties MatchProperties;
        public string GeneratorName;
        public string QueueName;
        public string PoolName;
        public string EnvironmentId;
        public string BackfillTicketId;
        public string MatchId;
        public string PoolId;
    }
    
    public struct ConnectionEventMessage : INetworkSerializeByMemcpy
    {
        public ConnectStatus ConnectStatus;
        public FixedPlayerName PlayerName;
    }
    
    public class ConnectionManager : NetworkBehaviour
    {
        public NetworkManager NetworkManager => NetworkManager.Singleton;

        public IMessageChannel<ConnectStatus> ConnectStatusChannel { get; private set; } 
        public IMessageChannel<ConnectionEventMessage> ConnectionEventChannel { get; private set; }
        public ClientRpcParamManager ClientRpcParams { get; private set; } = new ClientRpcParamManager();
        
        private ConnectionState m_currentState;
        internal OfflineState Offline;
        internal StartingHostState StartingHost;
        internal HostingState Hosting;
        internal StartingServerState StartingServer;
        internal ServerState Server;
        internal ClientConnectingState ClientConnecting;
        internal ClientReconnectingState ClientReconnecting;
        internal ClientConnectedState ClientConnected;

#if DEDICATED_SERVER
        public string BackfillTicketId { get; set; }
        public PayloadAllocation PayloadAllocation { get; set; }
        
        
        public string ServerBearerToken { get; set; }
        
        public async void DeleteBackfillTicket()
        {
            if (BackfillTicketId != null)
                await MatchmakerService.Instance.DeleteBackfillTicketAsync(BackfillTicketId);
            else
            {
                DebugWrapper.LogError("BackfillTicketId is null");
            }
        }
#endif
        
        public void Awake()
        {
            ConnectStatusChannel = new BufferedMessageChannel<ConnectStatus>();
            ConnectionEventChannel = new NetworkedMessageChannel<ConnectionEventMessage>();
            
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
            NetworkManager.OnServerStopped += OnServerStopped;
            NetworkManager.ConnectionApprovalCallback += ApprovalCheck;
        }

        private void Update()
        {
            m_currentState.OnUpdate();
        }

        public override void OnDestroy()
        {
            if (NetworkManager != null)
            {
                NetworkManager.OnClientConnectedCallback -= OnClientConnectedCallback;
                NetworkManager.OnClientDisconnectCallback -= OnClientDisconnectCallback;
                NetworkManager.OnServerStarted -= OnServerStarted;
                NetworkManager.OnTransportFailure += OnTransportFailure;
                NetworkManager.ConnectionApprovalCallback -= ApprovalCheck;
            }
            
            base.OnDestroy();
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
        
        private void OnTransportFailure()
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
        
        // we don't need this parameter as the ConnectionState already carries the relevant information
        private void OnServerStopped(bool _)
        {
            m_currentState.OnServerStopped();
        }

        /// <summary>
        /// Returns true if there is a player slot available for a new player to join the game.
        /// </summary>
        public bool HasAvailablePlayerSlot() => NetworkManager.ConnectedClientsIds.Count < Define.MaxConnectedPlayers;
        
        
        /// <summary>
        /// Returns the current connection status for a player attempting to join the game.
        /// </summary>
        public ConnectStatus GetConnectStatus(ConnectionPayload connectionPayload)
        {
            if (!HasAvailablePlayerSlot())
            {
                return ConnectStatus.ServerFull;
            }
            
            return SessionManager<SessionPlayerData>.Instance.IsDuplicateConnection(connectionPayload.playerId) ?
                ConnectStatus.LoggedInAgain : ConnectStatus.Success;
        }

        public int GetAvailablePlayerNumber()
        {
            for (var i = 0; i < Define.MaxConnectedPlayers; i++)
            {
                if (IsPlayerNumberAvailable(i))
                {
                    return i;
                }
            }
            
            // Server is full
            return -1;
        }

        private bool IsPlayerNumberAvailable(int playerNumber)
        {
            var connectedClients = NetworkManager.ConnectedClientsIds;
            foreach (var clientId in connectedClients)
            {
                var playerData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(clientId);
                if (playerData.HasValue)
                {
                    if (playerData.Value.PlayerNumber == playerNumber)
                    {
                        return false;
                    }
                }
            }
            
            return true;
        }
        
        public int GetPlayerNumber(ulong clientId)
        {
            var playerData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(clientId);
            if (playerData.HasValue)
            {
                return playerData.Value.PlayerNumber;
            }
            
            // this should never happen, throw an exception
            throw new Exception($"Player Number data not found for client {clientId}");
        }

        public int GetOpponentPlayerNumber(ulong clientId)
        {
            var playerNumber = GetPlayerNumber(clientId);
            
            if (NetworkManager.ConnectedClientsIds.Count == 1)
            {
                // if there is only one player, throw an exception
                throw new Exception($"There is only one player in the game. Client {clientId} is the only player.");
            }
            
            var opponentPlayerNumber = playerNumber == 0 ? 1 : 0;
            return opponentPlayerNumber;
        }

        [ServerRpc(RequireOwnership = false)]
        public void RequestShutdownServerRpc(ServerRpcParams serverRpcParams = default)
        {
            if (!IsServer) return;
            
            var clientId = serverRpcParams.Receive.SenderClientId;
            var playerData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(clientId);
            if (playerData.HasValue)
            {
                DebugWrapper.Log($"{playerData.Value.PlayerName} requested shutdown."); 
            }
            
            // disconnect all clients
            var reason = JsonUtility.ToJson(ConnectStatus.UserRequestedDisconnect);
            for (var i = NetworkManager.ConnectedClientsIds.Count - 1; i >= 0; i--)
            {
                var id = NetworkManager.ConnectedClientsIds[i];
                NetworkManager.DisconnectClient(id, reason);
            }
            
            ChangeState(Offline);
            ShutDownApplication();
        }

        public void ShutDownApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else 
            Application.Quit();
#endif
        }
    }
}
