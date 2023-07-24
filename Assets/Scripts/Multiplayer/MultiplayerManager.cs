using System;
using System.Collections;
using System.Collections.Generic;
using Minimax.ScriptableObjects.Events;
using Mono.CSharp;
using QFSW.QC;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

namespace Minimax
{
    public class MultiplayerManager : NetworkBehaviour
    {
        public static MultiplayerManager Instance { get; private set; }
        
        // light protection against DOS attacks
        private const int MAX_CONNECTION_PAYLOAD = 1024;
        private const int MAX_PLAYER_AMOUNT = 2;
        private string m_playerName;
        
        private NetworkList<SessionPlayerData> playerDataNetworkList;
        
        [Header("Listening To")]
        [SerializeField] private VoidEventSO m_startHostEvent = default;
        [SerializeField] private VoidEventSO m_startServerEvent = default;
        [SerializeField] private VoidEventSO m_startClientEvent = default;

        [Header("Broadcasting on")]
        [SerializeField] private VoidEventSO OnPlayerDataNetworkListChanged = default;
        
        private void OnEnable()
        {
            m_startHostEvent.OnEventRaised.AddListener(StartHost);
            m_startServerEvent.OnEventRaised.AddListener(StartServer);
            m_startClientEvent.OnEventRaised.AddListener(StartClient);
        }
        
        private void OnDisable()
        {
            m_startHostEvent.OnEventRaised.RemoveListener(StartHost);
            m_startServerEvent.OnEventRaised.RemoveListener(StartServer);
            m_startClientEvent.OnEventRaised.RemoveListener(StartClient);
        }

        private void Awake()
        {
            Instance = this;
            UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
            m_playerName = "PlayerName" + UnityEngine.Random.Range(0, 1000);
            playerDataNetworkList = new NetworkList<SessionPlayerData>();
            playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
        }
        
        private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<SessionPlayerData> changeEvent) {
            OnPlayerDataNetworkListChanged.RaiseEvent();
        }

        public void StartHost()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
            NetworkManager.Singleton.StartHost();
        }
        
        public void StartServer()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
            NetworkManager.Singleton.StartServer();
        }

        public void StartClient()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
            NetworkManager.Singleton.StartClient();
        }

        private void NetworkManager_ConnectionApprovalCallback(
            NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response)
        {
            var connectionData = request.Payload;
            if (connectionData.Length > MAX_CONNECTION_PAYLOAD)
            {
                response.Approved = false;
                return;
            }
            
            if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT) 
            {
                response.Approved = false;
                response.Reason = "Game is full";
                return;
            }
            response.Approved = true;
        }
        
        private void NetworkManager_OnClientConnectedCallback(ulong clientId)
        {
            playerDataNetworkList.Add(new SessionPlayerData {
                ClientId = clientId,
            });
            SetPlayerNameServerRpc(m_playerName);
            SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
            
            DebugWrapper.Instance.LogClientRpc($"Client {clientId} connected");
        }
        
        private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId) 
        {
            for (int i = 0; i < playerDataNetworkList.Count; i++) {
                SessionPlayerData sessionPlayerData = playerDataNetworkList[i];
                if (sessionPlayerData.ClientId == clientId) {
                    playerDataNetworkList.RemoveAt(i);
                }
            }
            DebugWrapper.Instance.LogClientRpc($"Client {clientId} disconnected");
            
            #if DEDICATED_SERVER
            Debug.Log("playerDataNetworkList.Count " + playerDataNetworkList.Count);
            if (playerDataNetworkList.Count == 0) {
                Debug.Log("All players left the game");
                Debug.Log("Shutting Down Network Manager");
                NetworkManager.Singleton.Shutdown();
                Application.Quit();
            }
            #endif
        }
        
        private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId) 
        {
            SetPlayerNameServerRpc(m_playerName);
            SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
        }
        
        private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
        {
            // TODO: Handle client disconnect
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
        {
            int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
            SessionPlayerData sessionPlayerData = playerDataNetworkList[playerDataIndex];
            sessionPlayerData.PlayerName = playerName;
            playerDataNetworkList[playerDataIndex] = sessionPlayerData;
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
        {
            int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
            SessionPlayerData sessionPlayerData = playerDataNetworkList[playerDataIndex];
            sessionPlayerData.PlayerId = playerId;
            playerDataNetworkList[playerDataIndex] = sessionPlayerData;
        }
        
        public bool IsClientConnected(ulong clientId) 
        {
            foreach (SessionPlayerData playerData in playerDataNetworkList) {
                if (playerData.ClientId == clientId) {
                    return true;
                }
            }
            return false;
        }

        public bool IsConnected() => IsClientConnected(NetworkManager.Singleton.LocalClientId);

        private int GetPlayerDataIndexFromClientId(ulong clientId) 
        {
            for (int i = 0; i< playerDataNetworkList.Count; i++) 
            {
                if (playerDataNetworkList[i].ClientId == clientId) 
                {
                    return i;
                }
            }
            return -1;
        }
        
        public NetworkList<SessionPlayerData> GetPlayerDataNetworkList() {
            return playerDataNetworkList;
        }
        
        public SessionPlayerData GetPlayerDataFromClientId(ulong clientId) {
            foreach (SessionPlayerData playerData in playerDataNetworkList) {
                if (playerData.ClientId == clientId) {
                    return playerData;
                }
            }
            return default;
        }
    }
}
