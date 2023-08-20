using Minimax.Utilities;
using Unity.Multiplayer.Samples.BossRoom;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

namespace Minimax.UnityGamingService.Multiplayer.ConnectionManagement
{
    public class StartingHostState : OnlineState
    {
        public StartingHostState(ConnectionManager connectionManager) : base(connectionManager) { }
        
        public override void Enter()
        {
            StartHost();
        }

        public override void Exit()
        {
        }

        public override void OnServerStarted()
        {
            m_connectionManager.ChangeState(m_connectionManager.Hosting);
        }
        
        public override void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            var connectionData = request.Payload;
            var clientId = request.ClientNetworkId;
            
            // This happens when starting as a host, before the end of the StartHost call. In that case, we simply approve ourselves.
            if (clientId == m_connectionManager.NetworkManager.LocalClientId)
            {
                var payload = System.Text.Encoding.UTF8.GetString(connectionData);
                var connectionPayload = JsonUtility.FromJson<ConnectionPayload>(payload);

                SessionManager<SessionPlayerData>.Instance.SetupConnectingPlayerSessionData(clientId,
                    connectionPayload.playerId,
                    new SessionPlayerData(clientId, connectionPayload.playerName, true));

                response.Approved = true;
            }
        }
        
        public override void StartHost()
        {
            // Need to set connection payload for host as well, as host is a client too
            var payload = JsonUtility.ToJson(new ConnectionPayload()
            {
                playerId = AuthenticationService.Instance.PlayerId,
                playerName = "PlayerName_" + Random.Range(0, 1000)
            });
            var payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);
            m_connectionManager.NetworkManager.NetworkConfig.ConnectionData = payloadBytes;
            
            if (!m_connectionManager.NetworkManager.StartHost())
            {
                DebugWrapper.LogError("Failed to start host");
                m_connectionManager.ChangeState(m_connectionManager.Offline);
            }
        }
    }
}