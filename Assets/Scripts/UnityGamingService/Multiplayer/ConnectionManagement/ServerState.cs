using Minimax.CoreSystems;
using Minimax.SceneManagement;
using Minimax.Utilities;
using Newtonsoft.Json;
using Unity.Multiplayer.Samples.BossRoom;
using Unity.Netcode;
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
            // check if server reached max players and if so, start the game
            var currentScene = GlobalManagers.Instance.Scene.CurrentlyLoadedScene;
            if (currentScene == SceneType.MenuScene.ToString())
            {
                if (!m_connectionManager.HasAvailablePlayerSlot())
                {
                    Debug.Log("Server reached max players, automatically starting game");
                    GlobalManagers.Instance.Scene.RequestLoadScene(SceneType.GamePlayScene, true);
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
            // automatically shutdown if there are no more clients in the gameplay scene
            var currentScene = GlobalManagers.Instance.Scene.CurrentlyLoadedScene;
            if (currentScene == SceneType.GamePlayScene.ToString())
            {
                if (m_connectionManager.HasNoPlayerConnected())
                {
                    Debug.Log("No more clients in the gameplay scene, shutting down server");
                    m_connectionManager.NetworkManager.Shutdown();
                    Application.Quit();
                }
            }
#endif
        }
        
        public override void OnServerStopped()
        {
            m_connectionManager.ConnectStatusChannel.Publish(ConnectStatus.GenericDisconnect);
            m_connectionManager.ChangeState(m_connectionManager.Offline);
        }
        
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
    }
}