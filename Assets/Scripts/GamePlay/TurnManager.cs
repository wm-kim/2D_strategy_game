using System;
using Minimax.CoreSystems;
using Minimax.UnityGamingService.Multiplayer;
using Minimax.Utilities;
using TMPro;
using Unity.Multiplayer.Samples.BossRoom;
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

        /// <summary>
        /// 서버에서 턴이 시작될 때 호출됩니다. 인자는 플레이어 번호입니다.
        /// </summary>
        public event Action<int> OnServerTurnStart;
        
        /// <summary>
        /// 클라이언트에서 턴이 시작될 때 호출됩니다. 인자는 내 턴인지 여부입니다.
        /// 클라이언트에서는 playerNumber를 알 수 없기 때문에 내 턴인지 여부를 인자로 받습니다.
        /// </summary>
        public event Action<bool> OnClientTurnStart;
        
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
            OnServerTurnStart?.Invoke(m_whosTurn.Value);
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
            OnServerTurnStart?.Invoke(m_whosTurn.Value);
            m_networkTimer.StartTimer();
            BroadCastCurrentTurnToClients();
        }

        private void BroadCastCurrentTurnToClients()
        {
            if (IsServer)
            {
                var connectedClientIds = m_networkManager.ConnectedClientsIds;
                var sessionPlayers = SessionPlayerManager.Instance;
            
                foreach (var clientId in connectedClientIds)
                {
                    int playerNumber = sessionPlayers.GetPlayerNumber(clientId);
                    bool isMyTurn = playerNumber == m_whosTurn.Value;
                    var clientRpcParam = sessionPlayers.ClientRpcParams[clientId];
                    OnTurnStartClientRpc(isMyTurn, clientRpcParam);
                }
            }
        }
        
        /// <summary>
        /// 클라이언트가 턴을 끝내기를 요청합니다.
        /// </summary>
        private void RequestEndTurn()
        {
            if (!IsClient) return;
            EndTurnServerRpc();
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void EndTurnServerRpc(ServerRpcParams serverRpcParams = default)
        {
            // check if it's the player's turn
            var senderClientId = serverRpcParams.Receive.SenderClientId;
            if (!CheckIfPlayerTurn(senderClientId, "end turn")) return;
            m_networkTimer.EndTimerImmediately();
        }
        
        [ClientRpc]
        private void OnTurnStartClientRpc(bool isMyTurn, ClientRpcParams clientRpcParams = default)
        {
            if (!IsClient) return;
            
            OnClientTurnStart?.Invoke(isMyTurn);
            m_turnText.text = isMyTurn ? "End Turn" : "Opponent's Turn";
            m_endTurnButton.interactable = isMyTurn;
        }

        public bool CheckIfPlayerTurn(ulong clientId, string logMessage = "")
        {
            var playerNumber = SessionPlayerManager.Instance.GetPlayerNumber(clientId);
            if (playerNumber != m_whosTurn.Value)
            {
                DebugWrapper.LogError($"Player {playerNumber} request denied to {logMessage}");
                return false;
            }

            return true;
        }
    }
}
