using System;
using Cysharp.Threading.Tasks;
using Minimax.Utilities;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Multiplay;
using UnityEngine;

namespace Minimax.UnityGamingService.Multiplayer.ConnectionManagement
{
    public class StartingServerState : OnlineState
    {
        public StartingServerState(ConnectionManager connectionManager) : base(connectionManager)
        { }

        public override void Enter()
        {
#if !DEDICATED_SERVER
            StartServer();
#else
            StartDedicatedServer();
#endif
        }

        public override void Exit() { }
        
        public override void OnServerStarted()
        {
            m_connectionManager.ChangeState(m_connectionManager.Server);
        }

        public override void OnServerStopped()
        {
            StartServerFailed();
        }

        public override void StartServer()
        {
            if (!m_connectionManager.NetworkManager.StartServer())
            {
                StartServerFailed();
            }
        }

        private void StartServerFailed()
        {
            DebugWrapper.LogError("Failed to start server");
            m_connectionManager.ConnectStatusChannel.Publish(ConnectStatus.StartServerFailed);
            m_connectionManager.ChangeState(m_connectionManager.Offline);
        }

#region Dedicated Server
#if DEDICATED_SERVER
        private float m_autoAllocateTimer = 9999999f;
        private IServerQueryHandler m_serverQueryHandler;
        private float m_acceptBackfillTicketsTimer;
        private float m_acceptBackfillTicketsTimerMax = 1.1f;
        bool m_alreadyAllocated = false;

         private async void StartDedicatedServer()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                // Unity 서비스가 초기화되지 않은 경우 초기화 옵션을 설정하고 초기화합니다.
                InitializationOptions initOptions = new InitializationOptions();
                await UnityServices.InitializeAsync(initOptions);

                DebugWrapper.Log("DEDICATED_SERVER");

                MultiplayEventCallbacks multiplayEventCallbacks = new MultiplayEventCallbacks();
                multiplayEventCallbacks.Allocate += MultiplayEventCallbacks_Allocate;
                multiplayEventCallbacks.Deallocate += MultiplayEventCallbacks_Deallocate;
                multiplayEventCallbacks.Error += MultiplayEventCallbacks_Error;
                multiplayEventCallbacks.SubscriptionStateChanged += MultiplayEventCallbacks_SubscriptionStateChanged;
                IServerEvents serverEvents =
                    await MultiplayService.Instance.SubscribeToServerEventsAsync(multiplayEventCallbacks);

                // this lets the server tell the multiplay service directly, should call this on every update
                m_serverQueryHandler =
                    await MultiplayService.Instance.StartServerQueryHandlerAsync((ushort)Define.MaxConnectedPlayers,
                        "MyServerName", "CardWars", "1.0", "Default");

                var serverConfig = MultiplayService.Instance.ServerConfig;
                if (serverConfig.AllocationId != "")
                {
                    // 이미 할당된 경우
                    MultiplayEventCallbacks_Allocate(new MultiplayAllocation("", serverConfig.ServerId,
                        serverConfig.AllocationId));
                }
            }
            else
            {
                // 이미 초기화된 경우
                DebugWrapper.Log("DEDICATED_SERVER Already Initialized");

                var serverConfig = MultiplayService.Instance.ServerConfig;
                if (serverConfig.AllocationId != "")
                {
                    // 이미 할당된 경우
                    MultiplayEventCallbacks_Allocate(new MultiplayAllocation("", serverConfig.ServerId,
                        serverConfig.AllocationId));
                }
            }
        }
         
         private void MultiplayEventCallbacks_Deallocate(MultiplayDeallocation obj)
         {
             DebugWrapper.Log("DEDICATED_SERVER MultiplayEventCallbacks_Deallocate");
         }

         private void MultiplayEventCallbacks_Error(MultiplayError obj)
         {
             DebugWrapper.Log("DEDICATED_SERVER MultiplayEventCallbacks_Error");
             DebugWrapper.Log(obj.Reason.ToString());
         }

         private void MultiplayEventCallbacks_SubscriptionStateChanged(MultiplayServerSubscriptionState obj)
         {
             DebugWrapper.Log("DEDICATED_SERVER MultiplayEventCallbacks_SubscriptionStateChanged");
             DebugWrapper.Log(obj.ToString());
         }
         
         private void MultiplayEventCallbacks_Allocate(MultiplayAllocation allocation)
         {
             DebugWrapper.Log("MultiplayEventCallbacks_Allocate");
             if (m_alreadyAllocated)
             {
                 DebugWrapper.Log("Already auto allocated!");
                 return;
             }
            
             SetupBackfillTickets();

             m_alreadyAllocated = true;

             var serverConfig = MultiplayService.Instance.ServerConfig;
             DebugWrapper.Log($"Server ID[{serverConfig.ServerId}]");
             DebugWrapper.Log($"Allocation ID[{allocation.AllocationId}]");
             DebugWrapper.Log($"Port[{serverConfig.Port}]");
             DebugWrapper.Log($"Query Port[{serverConfig.QueryPort}]");
             DebugWrapper.Log($"LogDirectory[{serverConfig.ServerLogDirectory}]");

             string ipv4address = "0.0.0.0";
             ushort port = serverConfig.Port;
             NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4address, port, "0.0.0.0");
             
            StartServer();
         }
         
         /// <summary>
         /// multiplay service automatically generate tickets for backfilling
         /// </summary>
         private async void SetupBackfillTickets()
         {
             DebugWrapper.Log("SetupBackfillTickets");
             m_connectionManager.PayloadAllocation = await MultiplayService.Instance.GetPayloadAllocationFromJsonAs<PayloadAllocation>();

             m_connectionManager.BackfillTicketId = m_connectionManager.PayloadAllocation.BackfillTicketId;
             DebugWrapper.Log("backfillTicketId: " + m_connectionManager.BackfillTicketId);

             m_acceptBackfillTicketsTimer = m_acceptBackfillTicketsTimerMax;
         }
         
        public override void OnUpdate()
        {
            m_autoAllocateTimer -= Time.deltaTime;
            if (m_autoAllocateTimer <= 0f)
            {
                m_autoAllocateTimer = 999f;
                MultiplayEventCallbacks_Allocate(null);
            }
            
            if (m_serverQueryHandler != null)
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    // setting the current players count, you can see on the dashboard
                    m_serverQueryHandler.CurrentPlayers = (ushort)NetworkManager.Singleton.ConnectedClientsIds.Count;
                }

                // lets know the multiplay service backend that we are still alive and active
                m_serverQueryHandler.UpdateServerCheck();
            }

            if (m_connectionManager.BackfillTicketId != null)
            {
                m_acceptBackfillTicketsTimer -= Time.deltaTime;
                if (m_acceptBackfillTicketsTimer <= 0f)
                {
                    m_acceptBackfillTicketsTimer = m_acceptBackfillTicketsTimerMax;
                    HandleBackfillTicket();
                }
            }
        }
        
        /// <summary>
        /// tell the machmaker service that this server has available slots for backfilling, and allow other players to join
        /// </summary>
        private async void HandleBackfillTicket()
        {
            if (m_connectionManager.HasAvailablePlayerSlot())
            {
                BackfillTicket backfillTicket = await MatchmakerService.Instance.ApproveBackfillTicketAsync(m_connectionManager.BackfillTicketId);
                m_connectionManager.BackfillTicketId = backfillTicket.Id;
            }
        }
#endif
#endregion
    }
}