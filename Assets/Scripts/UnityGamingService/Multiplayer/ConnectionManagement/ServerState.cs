using Minimax.Utilities;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;
#if DEDICATED_SERVER
using System.Collections.Generic;
using Minimax.CoreSystems;
using Minimax.SceneManagement;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Multiplay;
#endif

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
            DebugWrapper.Log("Ready server for accepting players");
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
            var playerId = SessionManager<SessionPlayerData>.Instance.GetPlayerId(clientId);
            m_connectionManager.DedicatedServer.UserJoinedServer(playerId);
            
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
                m_connectionManager.ClientRpcParams.Remove(clientId);
            }

#if DEDICATED_SERVER
            m_connectionManager.DedicatedServer.UserLeft(playerId);
#endif
        }
        
        public override void OnServerStopped()
        {
            m_connectionManager.ConnectStatusChannel.Publish(ConnectStatus.GenericDisconnect);
            m_connectionManager.ChangeState(m_connectionManager.Offline);
        }
        
        // TODO : Need to authenticate your user against an UGS' auth service, send auth token to dedicated server
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
            int playerNumber = m_connectionManager.GetAvailablePlayerNumber();
            
            bool isConnectSuccess = gameReturnStatus == ConnectStatus.Success;
            bool isPlayerNumberValid = playerNumber != -1;
            
            if (isConnectSuccess && isPlayerNumberValid)
            {
                DebugWrapper.Log($"Client {clientId} approved");
                DebugWrapper.Log($"Player {connectionPayload.playerName} assigned to player number {playerNumber}");
                
                SessionManager<SessionPlayerData>.Instance.SetupConnectingPlayerSessionData(clientId, connectionPayload.playerId,
                    new SessionPlayerData(clientId, connectionPayload.playerName, playerNumber, true));
                
                response.Approved = true;
                return;
            }
            
            response.Approved = false;
            DebugWrapper.Log($"Client {clientId} denied: {gameReturnStatus}");
            
            // If response.Approved is false, you can provide a message that explains the reason why via ConnectionApprovalResponse.
            // On the client-side, NetworkManager.DisconnectReason will be populated with this message via DisconnectReasonMessage
            response.Reason = JsonUtility.ToJson(gameReturnStatus);
        }
    }
}