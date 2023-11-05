using System;
using System.Collections.Generic;
using Minimax.CoreSystems;
using Minimax.Definitions;
using Minimax.GamePlay;
using Minimax.SceneManagement;
using Minimax.ScriptableObjects.Events;
using Minimax.UI.View.Popups;
using Unity.Netcode;
using Unity.Services.Matchmaker;
using UnityEngine;
using Utilities;
using Utilities.PubSub;
using Debug = Utilities.Debug;

namespace Minimax.UnityGamingService.Multiplayer.ConnectionManagement
{
    public enum ConnectStatus
    {
        Undefined,
        Success,
        ServerFull,              // can't join, server is already at capacity.
        LoggedInAgain,           // logged in on a separate client, causing this one to be kicked out.
        Reconnecting,            // client lost connection and is attempting to reconnect.
        UserRequestedDisconnect, // Intentional Disconnect triggered by the user.
        GenericDisconnect,       // server disconnected, but no specific reason given.
        StartHostFailed,         // host failed to bind
        StartServerFailed,       // server failed to bind
        StartClientFailed,       // failed to connect to server and/or invalid network endpoint
        ServerEndedSession       // server intentionally ended the session.
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
        public string                                           GeneratorName;
        public string                                           QueueName;
        public string                                           PoolName;
        public string                                           EnvironmentId;
        public string                                           BackfillTicketId;
        public string                                           MatchId;
        public string                                           PoolId;
    }

    public struct ConnectionEventMessage : INetworkSerializeByMemcpy
    {
        public ConnectStatus ConnectStatus;
        public NetworkString PlayerName;
    }

    public class ConnectionManager : NetworkBehaviour
    {
        public NetworkManager                          NetworkManager         => NetworkManager.Singleton;
        public NetworkDisconnectRelay                  DisconnectRelay        { get; private set; } = new();
        public IMessageChannel<ConnectStatus>          ConnectStatusChannel   { get; private set; }
        public IMessageChannel<ConnectionEventMessage> ConnectionEventChannel { get; private set; }

        private ConnectionState         m_currentState;
        public  OfflineState            Offline;
        public  StartingHostState       StartingHost;
        public  HostingState            Hosting;
        public  StartingServerState     StartingServer;
        public  ServerState             Server;
        public  GameStartedState        GameStarted;
        public  ClientConnectingState   ClientConnecting;
        public  ClientReconnectingState ClientReconnecting;
        public  ClientConnectedState    ClientConnected;

        public void Awake()
        {
            ConnectStatusChannel   = new BufferedMessageChannel<ConnectStatus>();
            ConnectionEventChannel = new NetworkedMessageChannel<ConnectionEventMessage>();

            Offline            = new OfflineState(this);
            StartingHost       = new StartingHostState(this);
            Hosting            = new HostingState(this);
            StartingServer     = new StartingServerState(this);
            Server             = new ServerState(this);
            GameStarted        = new GameStartedState(this);
            ClientConnecting   = new ClientConnectingState(this);
            ClientReconnecting = new ClientReconnectingState(this);
            ClientConnected    = new ClientConnectedState(this);
        }

        private void OnEnable()
        {
            m_currentState                            =  Offline;
            NetworkManager.OnClientConnectedCallback  += OnClientConnectedCallback;
            NetworkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
            NetworkManager.OnServerStarted            += OnServerStarted;
            NetworkManager.OnTransportFailure         += OnTransportFailure;
            NetworkManager.OnServerStopped            += OnServerStopped;
            NetworkManager.ConnectionApprovalCallback += ApprovalCheck;
        }

        public void OnDisable()
        {
            if (NetworkManager != null)
            {
                NetworkManager.OnClientConnectedCallback  -= OnClientConnectedCallback;
                NetworkManager.OnClientDisconnectCallback -= OnClientDisconnectCallback;
                NetworkManager.OnServerStarted            -= OnServerStarted;
                NetworkManager.OnTransportFailure         += OnTransportFailure;
                NetworkManager.ConnectionApprovalCallback -= ApprovalCheck;
            }
        }

#if DEDICATED_SERVER
        // automatically start the server if this is a dedicated server
        public DedicatedServerManager DedicatedServer { get; set; }
        private async void Start()
        {
            StartServer();
        }
#endif

        private void Update()
        {
            m_currentState.OnUpdate();
        }

        public void ChangeState(ConnectionState nextState)
        {
            Debug.Log(
                $"{name}: Changed connection state from {m_currentState.GetType().Name} to {nextState.GetType().Name}.");

            if (m_currentState != null) m_currentState.Exit();

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
            m_currentState.OnPlayerRequestedShutdown();
        }

        // we don't need this parameter as the ConnectionState already carries the relevant information
        private void OnServerStopped(bool _)
        {
            m_currentState.OnServerStopped();
        }


        /// <summary>
        /// Returns true if there is a player slot available for a new player to join the game.
        /// </summary>
        public bool HasAvailablePlayerSlot()
        {
            return NetworkManager.ConnectedClientsIds.Count < Define.MaxConnectedPlayers;
        }


        /// <summary>
        /// Returns the current connection status for a player attempting to join the game.
        /// </summary>
        public ConnectStatus GetConnectStatus(ConnectionPayload connectionPayload)
        {
            if (!HasAvailablePlayerSlot()) return ConnectStatus.ServerFull;

            return SessionPlayerManager.Instance.IsDuplicateConnection(connectionPayload.playerId)
                ? ConnectStatus.LoggedInAgain
                : ConnectStatus.Success;
        }

        /// <summary>
        /// Called when client wants to disconnect from the server.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        public void PlayerRequestShutdownServerRpc(ServerRpcParams serverRpcParams = default)
        {
            if (!IsServer) return;

            var senderClientId = serverRpcParams.Receive.SenderClientId;
            var playerNumber   = SessionPlayerManager.Instance.GetPlayerNumber(senderClientId);
            DisconnectAllAndShutdown(playerNumber, ConnectStatus.UserRequestedDisconnect);
        }

        public void DisconnectAllAndShutdown(int originPlayerNumber, ConnectStatus disconnectReason)
        {
            DisconnectRelay.DisconnectAll(originPlayerNumber, disconnectReason);
            ChangeState(Offline);
            ShutDownApplication();
        }

        public void ShutDownApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif DEDICATED_SERVER
            DedicatedServer.CloseServer();
#endif
        }
    }
}