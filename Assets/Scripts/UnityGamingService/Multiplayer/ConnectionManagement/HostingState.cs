using Minimax.Utilities;
using Newtonsoft.Json;
using Unity.Multiplayer.Samples.BossRoom;
using Unity.Netcode;
using UnityEngine;

namespace Minimax.UnityGamingService.Multiplayer.ConnectionManagement
{
    public class HostingState : OnlineState
    {
        public HostingState(ConnectionManager connectionManager) : base(connectionManager) { }
        
        public override void Enter()
        {
            
        }

        public override void Exit()
        {
            SessionManager<SessionPlayerData>.Instance.OnServerEnded();
        }

        public override void OnClientConnected(ulong clientId) { }
        
        public override void OnClientDisconnect(ulong clientId)
        {
            if (clientId != m_connectionManager.NetworkManager.LocalClientId)
            {
                var playerId = SessionManager<SessionPlayerData>.Instance.GetPlayerId(clientId);
                if (playerId != null)
                {
                    SessionManager<SessionPlayerData>.Instance.DisconnectClient(clientId);
                }
            }
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
            
            // If response.Approved is false, you can provide a message that explains the reason why via ConnectionApprovalResponse.
            // On the client-side, NetworkManager.DisconnectReason will be populated with this message via DisconnectReasonMessage
            response.Reason = JsonUtility.ToJson(gameReturnStatus);
        }

       
    }
}