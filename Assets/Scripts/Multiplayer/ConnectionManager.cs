using System;
using System.Collections;
using System.Collections.Generic;
using Minimax.ScriptableObjects.Events;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Minimax
{
    public class ConnectionManager : NetworkBehaviour
    {
        public static ConnectionManager Instance { get; private set; }
        
        // light protection against DOS attacks
        private const int MAX_CONNECTION_PAYLOAD = 1024;
        private const int MAX_PLAYER_AMOUNT = 2;
        private string m_playerName;
        
        private NetworkList<SessionPlayerData> m_playerDataNetworkList;
        
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
            m_playerName = "PlayerName" + UnityEngine.Random.Range(0, 1000);
            m_playerDataNetworkList = new NetworkList<SessionPlayerData>();
            m_playerDataNetworkList.OnListChanged += MPlayerDataNetworkListOnListChanged;
        }
        
        private void MPlayerDataNetworkListOnListChanged(NetworkListEvent<SessionPlayerData> changeEvent) {
            OnPlayerDataNetworkListChanged.RaiseEvent();
        }

        public void StartHost()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.SetClientSynchronizationMode(LoadSceneMode.Additive);
        }
        
        public void StartServer()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
            NetworkManager.Singleton.StartServer();
            NetworkManager.Singleton.SceneManager.SetClientSynchronizationMode(LoadSceneMode.Additive);
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
            m_playerDataNetworkList.Add(new SessionPlayerData {
                ClientId = clientId,
            });
            SetPlayerNameServerRpc(m_playerName);
            SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
            
            DebugWrapper.Log($"Client {clientId} connected");
        }
        
        private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId) 
        {
            for (int i = 0; i < m_playerDataNetworkList.Count; i++) {
                SessionPlayerData sessionPlayerData = m_playerDataNetworkList[i];
                if (sessionPlayerData.ClientId == clientId) {
                    m_playerDataNetworkList.RemoveAt(i);
                }
            }
            DebugWrapper.Log($"Client {clientId} disconnected");
            
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
            SessionPlayerData sessionPlayerData = m_playerDataNetworkList[playerDataIndex];
            sessionPlayerData.PlayerName = playerName;
            m_playerDataNetworkList[playerDataIndex] = sessionPlayerData;
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
        {
            int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
            SessionPlayerData sessionPlayerData = m_playerDataNetworkList[playerDataIndex];
            sessionPlayerData.PlayerId = playerId;
            m_playerDataNetworkList[playerDataIndex] = sessionPlayerData;
        }
        
        public bool HasAvailablePlayerSlots() {
            return NetworkManager.Singleton.ConnectedClientsIds.Count < MAX_PLAYER_AMOUNT;
        }
        
        public bool IsClientConnected(ulong clientId) 
        {
            foreach (SessionPlayerData playerData in m_playerDataNetworkList) {
                if (playerData.ClientId == clientId) {
                    return true;
                }
            }
            return false;
        }

        public bool IsConnected() => IsClientConnected(NetworkManager.Singleton.LocalClientId);

        private int GetPlayerDataIndexFromClientId(ulong clientId) 
        {
            for (int i = 0; i< m_playerDataNetworkList.Count; i++) 
            {
                if (m_playerDataNetworkList[i].ClientId == clientId) 
                {
                    return i;
                }
            }
            return -1;
        }
        
        public NetworkList<SessionPlayerData> GetPlayerDataNetworkList() {
            return m_playerDataNetworkList;
        }
        
        public SessionPlayerData GetPlayerDataFromClientId(ulong clientId) {
            foreach (SessionPlayerData playerData in m_playerDataNetworkList) {
                if (playerData.ClientId == clientId) {
                    return playerData;
                }
            }
            return default;
        }
    }
}
