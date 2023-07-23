using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Multiplay;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Minimax
{
    public class DedicatedServerInitializer : MonoBehaviour
    {
#if DEDICATED_SERVER
        private float autoAllocateTimer = 9999999f;
        private IServerQueryHandler serverQueryHandler;
        bool m_alreadyAllocated = false;

        private async void Start()
        {
            StartDedicatedServer();
            Debug.Log("ReadyServerForPlayersAsync");
            await MultiplayService.Instance.ReadyServerForPlayersAsync();

            Camera.main.enabled = false;
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
                serverQueryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync(2, "MyServerName", "CardWars", "1.0", "Default");

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
            autoAllocateTimer -= Time.deltaTime;
            if (autoAllocateTimer <= 0f) {
                autoAllocateTimer = 999f;
                MultiplayEventCallbacks_Allocate(null);
            }

            if (serverQueryHandler != null) {
                if (NetworkManager.Singleton.IsServer) {
                    serverQueryHandler.CurrentPlayers = (ushort)NetworkManager.Singleton.ConnectedClientsIds.Count;
                }
                serverQueryHandler.UpdateServerCheck();
            }
        }
#endif
    }
}
