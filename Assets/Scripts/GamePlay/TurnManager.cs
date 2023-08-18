using System.Collections.Generic;
using Minimax.Utilities;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax
{
    public class TurnManager : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] NetworkTimer m_networkTimer;
        [SerializeField] private TextMeshProUGUI m_turnText;
        [SerializeField] private Button m_endTurnButton;
        
        private NetworkManager m_networkManager => NetworkManager.Singleton;

        /// <summary>
        /// 현재 진행중인 턴을 가진 플레이어의 clientId를 반환합니다.
        /// </summary>
        private NetworkVariable<ulong> m_whosTurn = new NetworkVariable<ulong>(100);
        
        private IReadOnlyList<ulong> m_playerIds = new List<ulong>();
        private int m_currentPlayerIndex;
       
        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                m_playerIds = m_networkManager.ConnectedClientsIds;
            }

            if (IsClient)
            {
                m_whosTurn.OnValueChanged += OnClientTurnChanged;
                m_endTurnButton.onClick.AddListener(() => EndTurnServerRpc());
            }
            
            base.OnNetworkSpawn();
        }
        
        private void OnClientTurnChanged(ulong previousTurn, ulong newTurn)
        {
            if (newTurn == m_networkManager.LocalClientId)
            {
                m_endTurnButton.interactable = true;
                m_turnText.text = "Your Turn";
            }
            else
            {
                m_endTurnButton.interactable = false;
                m_turnText.text = "Opponent's Turn";
            }
        }

        [ServerRpc]
        public void StartGameServerRpc()
        {
            DecideWhoPlaysFirstServerRpc();
            m_networkTimer.StartTimerForGameStartServerRpc();
        }
        
        [ServerRpc]
        private void DecideWhoPlaysFirstServerRpc()
        {
            m_currentPlayerIndex = UnityEngine.Random.Range(0, m_playerIds.Count);
            m_whosTurn.Value = m_playerIds[m_currentPlayerIndex];
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void EndTurnServerRpc(ServerRpcParams serverRpcParams = default)
        {
            var clientId = serverRpcParams.Receive.SenderClientId;
            if (clientId != m_whosTurn.Value) return;
            
            TakeNextTurnServerRpc();
        }

        [ServerRpc]
        public void TakeNextTurnServerRpc()
        {
            m_networkTimer.StopTimerServerRpc();
            m_currentPlayerIndex++;
            if (m_currentPlayerIndex >= m_playerIds.Count) m_currentPlayerIndex = 0;
            m_whosTurn.Value = m_playerIds[m_currentPlayerIndex];
            
            m_networkTimer.StartTimerForNextTurnServerRpc();
        }
    }
}
