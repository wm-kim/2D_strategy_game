using UnityEngine;

namespace Minimax.Multiplayer
{
    public class DedicatedServerInitializer : MonoBehaviour
    {
#if DEDICATED_SERVER
        [Header("Listening To")]
        [SerializeField] private VoidEventSO m_onPlayerDataNetworkListChanged = default;
        
        private float m_autoAllocateTimer = 9999999f;
        private IServerQueryHandler m_serverQueryHandler;
        private PayloadAllocation m_payloadAllocation;
        private string m_backfillTicketId;
        private float m_acceptBackfillTicketsTimer;
        private float m_acceptBackfillTicketsTimerMax = 1.1f;
        
        bool m_alreadyAllocated = false;

        private void Start()
        {
            m_onPlayerDataNetworkListChanged.OnEventRaised.AddListener(DedicatedServerInitializer_OnPlayerDataNetworkListChanged);
            StartDedicatedServer();
            Camera.main.enabled = false;
        }
        
        private async void DedicatedServerInitializer_OnPlayerDataNetworkListChanged()
        {
            if (ConnectionManager.Instance.HasAvailablePlayerSlots())
            {
                Debug.Log("ReadyServerForPlayersAsync");
                await MultiplayService.Instance.ReadyServerForPlayersAsync();
            }
            else
            {
                Debug.Log("UnreadyServerAsync");
                await MultiplayService.Instance.UnreadyServerAsync();
            }
        }

        private async void StartDedicatedServer()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                InitializationOptions initOptions = new InitializationOptions();
                await UnityServices.InitializeAsync(initOptions);
                
                Debug.Log("DEDICATED_SERVER");
                
                MultiplayEventCallbacks multiplayEventCallbacks = new MultiplayEventCallbacks();
                multiplayEventCallbacks.Allocate += MultiplayEventCallbacks_Allocate;
                multiplayEventCallbacks.Deallocate += MultiplayEventCallbacks_Deallocate;
                multiplayEventCallbacks.Error += MultiplayEventCallbacks_Error;
                multiplayEventCallbacks.SubscriptionStateChanged += MultiplayEventCallbacks_SubscriptionStateChanged;
                IServerEvents serverEvents = await MultiplayService.Instance.SubscribeToServerEventsAsync(multiplayEventCallbacks);
                m_serverQueryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync(2, "MyServerName", "CardWars", "1.0", "Default");

                var serverConfig = MultiplayService.Instance.ServerConfig;
                if (serverConfig.AllocationId != "") {
                    // Already Allocated
                    MultiplayEventCallbacks_Allocate(new MultiplayAllocation("", serverConfig.ServerId, serverConfig.AllocationId));
                }
            }
            else {
                // Already Initialized
                Debug.Log("DEDICATED_SERVER Already Initialized");
                
                var serverConfig = MultiplayService.Instance.ServerConfig;
                if (serverConfig.AllocationId != "") {
                    // Already Allocated
                    MultiplayEventCallbacks_Allocate(new MultiplayAllocation("", serverConfig.ServerId, serverConfig.AllocationId));
                }
            }
        }
        
        private void MultiplayEventCallbacks_Allocate(MultiplayAllocation allocation)
        {
            Debug.Log("MultiplayEventCallbacks_Allocate");
            if (m_alreadyAllocated)
            {
                Debug.Log("Already allocated");
                return;
            }
            
            SetupBackfillTickets();
            
            m_alreadyAllocated = true;

            var serverConfig = MultiplayService.Instance.ServerConfig;
            Debug.Log($"Server ID[{serverConfig.ServerId}]");
            Debug.Log($"Allocation ID[{allocation.AllocationId}]");
            Debug.Log($"Port[{serverConfig.Port}]");
            Debug.Log($"Query Port[{serverConfig.QueryPort}]");
            Debug.Log($"LogDirectory[{serverConfig.ServerLogDirectory}]");

            string ipv4address = "0.0.0.0";
            ushort port = serverConfig.Port;
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4address, port, "0.0.0.0");
            
            ConnectionManager.Instance.StartServer();
            
        }
        
        private async void SetupBackfillTickets() {
            Debug.Log("SetupBackfillTickets");
            m_payloadAllocation = await MultiplayService.Instance.GetPayloadAllocationFromJsonAs<PayloadAllocation>();

            m_backfillTicketId = m_payloadAllocation.BackfillTicketId;
            Debug.Log("backfillTicketId: " + m_backfillTicketId);

            m_acceptBackfillTicketsTimer = m_acceptBackfillTicketsTimerMax;
        }
        
        private void MultiplayEventCallbacks_Deallocate(MultiplayDeallocation obj) {
            Debug.Log("DEDICATED_SERVER MultiplayEventCallbacks_Deallocate");
        }
        
        private void MultiplayEventCallbacks_Error(MultiplayError obj) {
            Debug.Log("DEDICATED_SERVER MultiplayEventCallbacks_Error");
            Debug.Log(obj.Reason);
        }
        
        private void MultiplayEventCallbacks_SubscriptionStateChanged(MultiplayServerSubscriptionState obj) {
            Debug.Log("DEDICATED_SERVER MultiplayEventCallbacks_SubscriptionStateChanged");
            Debug.Log(obj);
        }

        private void Update()
        {
            m_autoAllocateTimer -= Time.deltaTime;
            if (m_autoAllocateTimer <= 0f) {
                m_autoAllocateTimer = 999f;
                MultiplayEventCallbacks_Allocate(null);
            }

            if (m_serverQueryHandler != null) {
                if (NetworkManager.Singleton.IsServer) {
                    m_serverQueryHandler.CurrentPlayers = (ushort)NetworkManager.Singleton.ConnectedClientsIds.Count;
                }
                m_serverQueryHandler.UpdateServerCheck();
            }
        }
        
        [Serializable]
        public class PayloadAllocation {
            public Unity.Services.Matchmaker.Models.MatchProperties MatchProperties;
            public string GeneratorName;
            public string QueueName;
            public string PoolName;
            public string EnvironmentId;
            public string BackfillTicketId;
            public string MatchId;
            public string PoolId;
        }
#endif
    }
}
