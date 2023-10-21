using System.Collections;
using System.Collections.Generic;
using Minimax.GamePlay;
using Minimax.UnityGamingService.Multiplayer;
using Minimax.Utilities;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Minimax
{
    public class ServerManaManager : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_myManaText;
        [SerializeField] private TextMeshProUGUI m_opponentManaText;
        
        /// <summary>
        /// key is playerNumber value is mana
        /// </summary>
        private Dictionary<int, int> m_playerMana = new Dictionary<int, int>();
        
        private NetworkManager m_networkManager = NetworkManager.Singleton;
        
        public void InitPlayersMana()
        {
            if (!IsServer) return;
            
            foreach (var playerNumber in SessionPlayerManager.Instance.GetAllPlayerNumbers())
            {
                m_playerMana.Add(playerNumber, Define.InitialManaCapacity);
                SetPlayersManaClientRpc(playerNumber, m_playerMana[playerNumber]);
            }
        }
        
        [ClientRpc]
        private void SetPlayersManaClientRpc(int playerNumber, int mana)
        {
           if (playerNumber == TurnManager.Instance.MyPlayerNumber) m_myManaText.text = mana.ToString();
           else m_opponentManaText.text = mana.ToString();
        }
    }
}
