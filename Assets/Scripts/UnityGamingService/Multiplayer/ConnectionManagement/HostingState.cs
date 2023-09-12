using System.Collections;
using System.Collections.Generic;
using Minimax.Utilities;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;

namespace Minimax.UnityGamingService.Multiplayer.ConnectionManagement
{
    public class HostingState : OnlineState
    {
        /// <summary>
        /// key is playerNumber, value is the remain time for client reconnection
        /// </summary>
        private Dictionary<int, float> m_remainTimeForClientReconnect = new Dictionary<int, float>();
        /// <summary>
        /// key is playerId, value is the coroutine for waiting client reconnection
        /// </summary>
        private Dictionary<int, Coroutine> m_coroutines = new Dictionary<int, Coroutine>();

        public HostingState(ConnectionManager connectionManager) : base(connectionManager) { }
        
        public override void Enter()
        {
        }

        public override void Exit()
        {
            // stop all the coroutines if exist
            foreach (var coroutine in m_coroutines)
                m_connectionManager.StopCoroutine(coroutine.Value);
            
            SessionPlayerManager.Instance.OnServerEnded();
        }

        public override void OnClientConnected(ulong clientId)
        {
            DebugWrapper.Log($"Client {clientId} connected");
            var sessionPlayers = SessionPlayerManager.Instance;
            var playerNumber = sessionPlayers.GetPlayerNumber(clientId);
            
            m_connectionManager.ConnectionEventChannel.Publish(
                new ConnectionEventMessage()
                {
                    ConnectStatus = ConnectStatus.Success,
                    PlayerName = sessionPlayers.GetPlayerData(clientId)?.PlayerName
                });
            
            // if this is a reconnection, stop the coroutine
            if (m_coroutines.ContainsKey(playerNumber))
            {
                m_connectionManager.StopCoroutine(m_coroutines[playerNumber]);
                m_coroutines.Remove(playerNumber);
            }
            else 
            {
                m_remainTimeForClientReconnect[playerNumber] = Define.ServerWaitMsForClientReconnect;
            }
        }
        
        public override void OnClientDisconnect(ulong clientId)
        {
            if (clientId != m_connectionManager.NetworkManager.LocalClientId)
            {
                var sessionPlayers = SessionPlayerManager.Instance;
                var playerId = sessionPlayers.GetPlayerId(clientId);
                var playerNumber = sessionPlayers.GetPlayerNumber(clientId);
                var sessionData = sessionPlayers.GetPlayerData(playerId);
                
                if (sessionData.HasValue)
                {
                    // for notifying other connected clients that clientId has disconnected
                    m_connectionManager.ConnectionEventChannel.Publish(
                        new ConnectionEventMessage()
                        {
                            ConnectStatus = ConnectStatus.GenericDisconnect,
                            PlayerName = sessionData.Value.PlayerName
                        });
                    
                    // always start a coroutine for waiting client reconnection no matter what the reason is
                    // because if the reason leads to server shutdown, all the coroutines will be stopped
                    m_coroutines[playerNumber] = 
                        m_connectionManager.StartCoroutine(WaitPlayerReconnection(playerNumber));
                }
                
                sessionPlayers.DisconnectClient(clientId);
                sessionPlayers.ClientRpcParams.Remove(clientId);
            }
        }

        private IEnumerator WaitPlayerReconnection(int playerNumber)
        {
            DebugWrapper.Log($"Wait for player {playerNumber.ToString()} reconnection...\n" +
                             $"Remain time : {m_remainTimeForClientReconnect[playerNumber].ToString("F1")}");
            
            while (m_remainTimeForClientReconnect[playerNumber] > 0)
            {
                m_remainTimeForClientReconnect[playerNumber] -= m_connectionManager.NetworkManager.ServerTime.TimeAsFloat;
                yield return null;
            }
            
            DebugWrapper.Log($"player {playerNumber.ToString()} reconnection Timeout");
            var reason = JsonUtility.ToJson(ConnectStatus.ServerEndedSession);
            m_connectionManager.SendGameResultAndShutdown(playerNumber, reason);
        }
        
        public override void OnServerStopped()
        {
            m_connectionManager.ConnectStatusChannel.Publish(ConnectStatus.GenericDisconnect);
            m_connectionManager.ChangeState(m_connectionManager.Offline);
            m_connectionManager.ShutDownApplication();
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
            var sessionPlayers = SessionPlayerManager.Instance;
            int playerNumber = sessionPlayers.GetAvailablePlayerNumber();
            
            bool isConnectSuccess = gameReturnStatus == ConnectStatus.Success;
            bool isPlayerNumberAvailable = playerNumber != -1;
            
            if (isConnectSuccess && isPlayerNumberAvailable)
            {
                DebugWrapper.Log($"Client {clientId.ToString()} approved");
                DebugWrapper.Log($"Player {connectionPayload.playerName} assigned to player number {playerNumber.ToString()}");
                
                sessionPlayers.SetupConnectingPlayerSessionData(clientId, connectionPayload.playerId,
                    new SessionPlayerData(clientId, connectionPayload.playerName, playerNumber, true));
                
                response.Approved = true;
                return;
            }
            
            response.Approved = false;
            DebugWrapper.Log($"Client {clientId.ToString()} denied: {gameReturnStatus}");
            
            // If response.Approved is false, you can provide a message that explains the reason why via ConnectionApprovalResponse.
            // On the client-side, NetworkManager.DisconnectReason will be populated with this message via DisconnectReasonMessage
            response.Reason = JsonUtility.ToJson(gameReturnStatus);
        }
    }
}