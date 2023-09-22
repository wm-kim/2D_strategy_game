using System.Collections;
using System.Collections.Generic;
using Minimax.Utilities;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;

namespace Minimax.UnityGamingService.Multiplayer.ConnectionManagement
{
    public class GameStartedState : OnlineState
    {
        /// <summary>
        /// key is playerNumber, value is the remain time for client reconnection
        /// </summary>
        private Dictionary<int, float> m_remainTimeForClientReconnect = new Dictionary<int, float>();
        /// <summary>
        /// key is playerNumber, value is the coroutine for waiting client reconnection
        /// </summary>
        private Dictionary<int, Coroutine> m_coroutines = new Dictionary<int, Coroutine>();
        
        public GameStartedState(ConnectionManager connectionManager) : base(connectionManager) { }

        public override void Enter()
        {
            foreach (var playerNumber in SessionPlayerManager.Instance.GetAllPlayerNumbers())
                m_remainTimeForClientReconnect[playerNumber] = Define.ServerWaitMsForClientReconnect;
        }

        public override void Exit()
        {
            // stop all the coroutines if exist
            foreach (var coroutine in m_coroutines)
                m_connectionManager.StopCoroutine(coroutine.Value);
            
            SessionPlayerManager.Instance.OnServerEnded();
        }
        
        // This is always a reconnection
        public override void OnClientConnected(ulong clientId)
        {
            DebugWrapper.Log($"Client {clientId} connected");
            var sessionPlayers = SessionPlayerManager.Instance;
            var playerNumber = sessionPlayers.GetPlayerNumber(clientId);
            var sessionData = sessionPlayers.GetPlayerData(clientId);
            if (sessionData.HasValue)
            {
                m_connectionManager.ConnectionEventChannel.Publish(
                    new ConnectionEventMessage()
                    {
                        ConnectStatus = ConnectStatus.Success,
                        PlayerName = sessionData.Value.PlayerName
                    });
            }
            
            if (m_coroutines.ContainsKey(playerNumber))
            {
                m_connectionManager.StopCoroutine(m_coroutines[playerNumber]);
                m_coroutines.Remove(playerNumber);
            }
            
#if DEDICATED_SERVER
            var playerId = sessionPlayers.GetPlayerId(clientId);
            m_connectionManager.DedicatedServer.UserJoinedServer(playerId);
#endif
        }
        
        public override void OnClientDisconnect(ulong clientId)
        {
            var sessionPlayers = SessionPlayerManager.Instance;
            
            if (clientId != m_connectionManager.NetworkManager.LocalClientId)
            {
                var playerNumber = sessionPlayers.GetPlayerNumber(clientId);
                var sessionData = sessionPlayers.GetPlayerData(clientId);
                var disconnectReason = m_connectionManager.DisconnectRelay.GetDisconnectReason(playerNumber);
                
                if (sessionData.HasValue)
                {
                    // for notifying other connected clients that clientId has disconnected
                    m_connectionManager.ConnectionEventChannel.Publish(
                        new ConnectionEventMessage()
                        {
                            ConnectStatus = disconnectReason,
                            PlayerName = sessionData.Value.PlayerName
                        });
                }

                if (disconnectReason == ConnectStatus.GenericDisconnect)
                {
                    m_coroutines[playerNumber] = 
                        m_connectionManager.StartCoroutine(WaitClientReconnection(playerNumber));
                }
            }
            
            sessionPlayers.DisconnectClient(clientId);
            sessionPlayers.ClientRpcParams.Remove(clientId);
            
#if DEDICATED_SERVER
            var playerId = sessionPlayers.GetPlayerId(clientId);
            m_connectionManager.DedicatedServer.UserLeft(playerId);
#endif
        }
        
         private IEnumerator WaitClientReconnection(int playerNumber)
        {
            DebugWrapper.Log($"Wait for player {playerNumber.ToString()} reconnection...\n" +
                             $"Remain time : {m_remainTimeForClientReconnect[playerNumber].ToString("F1")}");
            
            while (m_remainTimeForClientReconnect[playerNumber] > 0)
            {
                m_remainTimeForClientReconnect[playerNumber] -= m_connectionManager.NetworkManager.ServerTime.TimeAsFloat;
                yield return null;
            }
            
            DebugWrapper.Log($"player {playerNumber.ToString()} reconnection Timeout");
            m_connectionManager.DisconnectAllAndShutdown(playerNumber, ConnectStatus.ServerEndedSession);
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