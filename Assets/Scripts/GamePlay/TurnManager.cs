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
        private NetworkVariable<ulong> m_whosTurn = new NetworkVariable<ulong>(1000);
        private IReadOnlyList<ulong> m_playerIds = new List<ulong>();
        private int m_currentPlayerIndex;
       
    }
}
