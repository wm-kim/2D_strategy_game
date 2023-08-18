using Minimax.Utilities;
using Newtonsoft.Json;
using Unity.Multiplayer.Samples.BossRoom;
using Unity.Netcode;
using UnityEngine;

namespace Minimax.UnityGamingService.Multiplayer.ConnectionManagement
{
    public class HostingState : ConnectionState
    {
        public HostingState(ConnectionManager connectionManager) : base(connectionManager) { }
        
        private const int k_maxConnectPayload = 1024;

        public override void Enter()
        {
            
        }

        public override void Exit()
        {
        }

        public override void OnClientConnected(ulong clientId)
        {
            m_connectionManager.NetworkManager.SceneManager.PostSynchronizationSceneUnloading = true;
        }
        
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
            if (connectionData.Length > k_maxConnectPayload)
            {
                // If connectionData too high, deny immediately to avoid wasting time on the server. This is intended as
                // a bit of light protection against DOS attacks that rely on sending silly big buffers of garbage.
                response.Approved = false;
                return;
            }
            
            var payload = System.Text.Encoding.UTF8.GetString(connectionData);
            var connectionPayload = JsonConvert.DeserializeObject<ConnectionPayload>(payload);
            var gameReturnStatus = GetConnectStatus(connectionPayload);

            if (gameReturnStatus == ConnectStatus.Success)
            {
                SessionManager<SessionPlayerData>.Instance.SetupConnectingPlayerSessionData(clientId, connectionPayload.playerId,
                    new SessionPlayerData(clientId, connectionPayload.playerName, true));
                
                response.Approved = true;
                return;
            }
            
            response.Approved = false;
            response.Reason = JsonUtility.ToJson(gameReturnStatus);
        }

        private ConnectStatus GetConnectStatus(ConnectionPayload connectionPayload)
        {
            if (m_connectionManager.NetworkManager.ConnectedClientsIds.Count >= Define.MaxConnectedPlayers)
            {
                return ConnectStatus.ServerFull;
            }
            
            return SessionManager<SessionPlayerData>.Instance.IsDuplicateConnection(connectionPayload.playerId) ?
                ConnectStatus.LoggedInAgain : ConnectStatus.Success;
        }
    }
}