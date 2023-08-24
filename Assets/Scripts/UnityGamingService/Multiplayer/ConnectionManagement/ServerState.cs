using System.Collections.Generic;
using Minimax.CoreSystems;
using Minimax.SceneManagement;
using Minimax.Utilities;
using Newtonsoft.Json;
using Unity.Netcode;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Multiplay;
using UnityEngine;

namespace Minimax.UnityGamingService.Multiplayer.ConnectionManagement
{
    public class ServerState : OnlineState
    {
        public ServerState(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

        public override async void Enter()
        {
#if DEDICATED_SERVER
            Debug.Log("Ready server for accepting players");
            await MultiplayService.Instance.ReadyServerForPlayersAsync();
#endif
        }

        public override void Exit()
        {
            SessionManager<SessionPlayerData>.Instance.OnServerEnded();
        }

        public override void OnClientConnected(ulong clientId)
        {
            DebugWrapper.Log($"Client {clientId} connected");
            
            m_connectionManager.ConnectionEventChannel.Publish(
                new ConnectionEventMessage()
                {
                    ConnectStatus = ConnectStatus.Success,
                    PlayerName = SessionManager<SessionPlayerData>.Instance.GetPlayerData(clientId)?.PlayerName
                });
            
#if DEDICATED_SERVER
            HandleUpdateBackfillTickets();
            
            // check if server reached max players and if so, start the game
            var currentScene = GlobalManagers.Instance.Scene.CurrentlyLoadedScene;
            if (currentScene != SceneType.GamePlayScene.ToString())
            {
                if (!m_connectionManager.HasAvailablePlayerSlot())
                {
                    DebugWrapper.Log("Server reached max players, automatically starting game");
                    GlobalManagers.Instance.Scene.LoadScene(SceneType.GamePlayScene, true);
                }
            }
#endif
        }
        
        public override void OnClientDisconnect(ulong clientId)
        {
            if (clientId != m_connectionManager.NetworkManager.LocalClientId)
            {
                var playerId = SessionManager<SessionPlayerData>.Instance.GetPlayerId(clientId);
                if (playerId != null)
                {
                    var sessionData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(playerId);
                    if (sessionData.HasValue)
                    {
                        m_connectionManager.ConnectionEventChannel.Publish(
                            new ConnectionEventMessage()
                            {
                                ConnectStatus = ConnectStatus.GenericDisconnect,
                                PlayerName = sessionData.Value.PlayerName
                            });
                    }
                    SessionManager<SessionPlayerData>.Instance.DisconnectClient(clientId);
                }
            }
 
#if DEDICATED_SERVER
            HandleUpdateBackfillTickets();
#endif
        }
        
        public override void OnServerStopped()
        {
            m_connectionManager.ConnectStatusChannel.Publish(ConnectStatus.GenericDisconnect);
            m_connectionManager.ChangeState(m_connectionManager.Offline);
        }
        
        // [TODO] Need to authenticate your user against an UGS' auth service, send auth token to dedicated server
        public override void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response)
        {
            var connectionData = request.Payload;
            var clientId = request.ClientNetworkId;
            if (connectionData.Length > Define.MaxConnectPayloadSize)
            {
                // If connectionData too high, deny immediately to avoid wasting time on the server. This is intended as
                // a bit of light protection against DOS attacks that rely on sending silly big buffers of garbage.
                response.Approved = false;
                return;
            }
            
            var payload = System.Text.Encoding.UTF8.GetString(connectionData);
            var connectionPayload = JsonConvert.DeserializeObject<ConnectionPayload>(payload);
            var gameReturnStatus = m_connectionManager.GetConnectStatus(connectionPayload);

            if (gameReturnStatus == ConnectStatus.Success)
            {
                DebugWrapper.Log($"Client {clientId} approved");
                SessionManager<SessionPlayerData>.Instance.SetupConnectingPlayerSessionData(clientId, connectionPayload.playerId,
                    new SessionPlayerData(clientId, connectionPayload.playerName, true));
                
                response.Approved = true;
                return;
            }
            
            response.Approved = false;
            DebugWrapper.Log($"Client {clientId} denied: {gameReturnStatus}");
            
            // If response.Approved is false, you can provide a message that explains the reason why via ConnectionApprovalResponse.
            // On the client-side, NetworkManager.DisconnectReason will be populated with this message via DisconnectReasonMessage
            response.Reason = JsonUtility.ToJson(gameReturnStatus);
        }
        
#if DEDICATED_SERVER
        private async void HandleUpdateBackfillTickets() {
            if (m_connectionManager.BackfillTicketId != null 
                && m_connectionManager.PayloadAllocation != null 
                && m_connectionManager.HasAvailablePlayerSlot()) {
                
                DebugWrapper.Log("HandleUpdateBackfillTickets");

                List<Unity.Services.Matchmaker.Models.Player> playerList = new List<Unity.Services.Matchmaker.Models.Player>();

                var connectedClientIds = m_connectionManager.NetworkManager.ConnectedClientsIds;
                foreach(var clientId in connectedClientIds)
                {
                    var playerId = SessionManager<SessionPlayerData>.Instance.GetPlayerId(clientId);
                    playerList.Add(new Unity.Services.Matchmaker.Models.Player(playerId));
                }

                var payloadAllocation = m_connectionManager.PayloadAllocation;
                MatchProperties matchProperties = new MatchProperties(
                    payloadAllocation.MatchProperties.Teams, 
                    playerList, 
                    payloadAllocation.MatchProperties.Region, 
                    payloadAllocation.MatchProperties.BackfillTicketId
                );

                try {
                    await MatchmakerService.Instance.UpdateBackfillTicketAsync(payloadAllocation.BackfillTicketId,
                        new BackfillTicket(m_connectionManager.BackfillTicketId, properties: new BackfillTicketProperties(matchProperties))
                    );
                } catch (MatchmakerServiceException e) {
                    DebugWrapper.Log("Error: " + e);
                }
            }
        }
#endif
    }
}