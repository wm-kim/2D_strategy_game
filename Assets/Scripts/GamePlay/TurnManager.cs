using Minimax.CoreSystems;
using Minimax.Utilities;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Minimax.GamePlay
{
    public class TurnManager : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] NetworkTimer m_networkTimer;
        [SerializeField] private Button m_endTurnButton;
        [SerializeField] private TextMeshProUGUI m_turnText;
        
        [Header("Settings")]
        [SerializeField] private float m_turnDuration = 10f;
        
        private NetworkManager m_networkManager => NetworkManager.Singleton;
        private NetworkVariable<int> m_whosTurn = new NetworkVariable<int>(-1);

        private void Awake()
        {
            m_endTurnButton.onClick.AddListener(RequestEndTurn);
        }

        public void StartInitialTurn()
        {
            if (!IsServer) return;
            
            DecideWhoGoesFirst();
            m_networkTimer.ConFig(m_turnDuration, StartNewTurn);
            m_networkTimer.StartTimer();
        }
        
        /// <summary>
        /// 랜덤으로 누가 먼저 턴을 시작할지 결정합니다.
        /// </summary>
        private void DecideWhoGoesFirst()
        {
            if (!IsServer) return;
            
            m_whosTurn.Value = Random.Range(0, 2);
            BroadCastCurrentTurnToClients();
            DebugWrapper.Log($"Player {m_whosTurn.Value} goes first");
        }
        
        private void StartNewTurn()
        {
            if (!IsServer) return;
            
            m_whosTurn.Value = m_whosTurn.Value == 0 ? 1 : 0;
            DebugWrapper.Log($"Player {m_whosTurn.Value} turn starts");
            m_networkTimer.StartTimer();
            BroadCastCurrentTurnToClients();
        }

        private void BroadCastCurrentTurnToClients()
        {
            if (IsServer)
            {
                var connectedClientIds = m_networkManager.ConnectedClientsIds;
                var connectionManager = GlobalManagers.Instance.Connection;
            
                foreach (var clientId in connectedClientIds)
                {
                    int playerNumber = connectionManager.GetPlayerNumber(clientId);
                    bool isMyTurn = playerNumber == m_whosTurn.Value;
                    var clientRpcParam = connectionManager.ClientRpcParams[clientId];
                    OnTurnStartClientRpc(isMyTurn, clientRpcParam);
                }
            }
        }
        
        private void RequestEndTurn()
        {
            if (!IsClient) return;
            EndTurnServerRpc();
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void EndTurnServerRpc(ServerRpcParams serverRpcParams = default)
        {
            // check if it's the player's turn
            var playerNumber = GlobalManagers.Instance.Connection.GetPlayerNumber(serverRpcParams.Receive.SenderClientId);
            if (playerNumber != m_whosTurn.Value)
            {
                DebugWrapper.LogError($"Player request denied to end turn");
                return;
            }
            
            m_networkTimer.EndTimerImmediately();
        }
        
        [ClientRpc]
        private void OnTurnStartClientRpc(bool isMyTurn, ClientRpcParams clientRpcParams = default)
        {
            if (!IsClient) return;
            
            m_turnText.text = isMyTurn ? "End Turn" : "Opponent's Turn";
            m_endTurnButton.interactable = isMyTurn;
        }
    }
}
