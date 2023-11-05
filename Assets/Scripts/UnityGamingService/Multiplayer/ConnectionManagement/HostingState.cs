using Minimax.Definitions;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;
using Utilities;
#if DEDICATED_SERVER
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Multiplay;
#endif

namespace Minimax.UnityGamingService.Multiplayer.ConnectionManagement
{
    public class HostingState : OnlineState
    {
        public HostingState(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override void OnClientDisconnect(ulong clientId)
        {
            var sessionPlayers = SessionPlayerManager.Instance;
            sessionPlayers.DisconnectClient(clientId);
            sessionPlayers.ClientRpcParams.Remove(clientId);
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
            var clientId       = request.ClientNetworkId;
            if (connectionData.Length > Define.MaxConnectPayloadSize)
            {
                // If connectionData too high, deny immediately to avoid wasting time on the server. This is intended as
                // a bit of light protection against DOS attacks that rely on sending silly big buffers of garbage.
                response.Approved = false;
                return;
            }

            var payload           = System.Text.Encoding.UTF8.GetString(connectionData);
            var connectionPayload = JsonConvert.DeserializeObject<ConnectionPayload>(payload);
            var gameReturnStatus  = m_connectionManager.GetConnectStatus(connectionPayload);
            var sessionPlayers    = SessionPlayerManager.Instance;
            var playerNumber      = sessionPlayers.GetAvailablePlayerNumber();

            var isConnectSuccess        = gameReturnStatus == ConnectStatus.Success;
            var isPlayerNumberAvailable = playerNumber != -1;

            if (isConnectSuccess && isPlayerNumberAvailable)
            {
                DebugWrapper.Log($"Client {clientId.ToString()} approved");
                DebugWrapper.Log(
                    $"Player {connectionPayload.playerName} assigned to player number {playerNumber.ToString()}");

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