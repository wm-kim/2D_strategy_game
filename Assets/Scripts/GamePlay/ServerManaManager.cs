using System.Collections;
using System.Collections.Generic;
using Minimax.GamePlay;
using Minimax.UnityGamingService.Multiplayer;
using Minimax.Utilities;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Minimax.GamePlay
{
    public class ServerManaManager : NetworkBehaviour
    {
        [Header("References")] 
        [SerializeField] private ClientManaManager m_clientManager;

        [Header("Mana Configuration")]
        [SerializeField] private int m_initialManaCapacity = 3;
        [SerializeField] private int m_maxManaCapacity = 10;
        [SerializeField] private int m_incrementCapacityAmount = 1;
        
        /// <summary>
        /// key is playerNumber value is mana
        /// </summary>
        private Dictionary<int, int> m_playerManaCap = new Dictionary<int, int>();
        
        /// <summary>
        /// key is playerNumber, value is current mana
        /// </summary>
        private Dictionary<int, int> m_playerCurrentMana = new Dictionary<int, int>();
        
        public void InitPlayersMana()
        {
            if (!IsServer) return;
            
            foreach (var playerNumber in SessionPlayerManager.Instance.GetAllPlayerNumbers())
            {
                m_playerManaCap.Add(playerNumber, m_initialManaCapacity);
                // player의 현재 마나는 초기 마나와 같다.
                m_playerCurrentMana.Add(playerNumber, m_initialManaCapacity); 
                
                m_clientManager.SetPlayersCurrentManaClientRpc(playerNumber, m_playerManaCap[playerNumber]);
                m_clientManager.SetPlayersManaCapClientRpc(playerNumber, m_playerManaCap[playerNumber]);
            }
        }
        
        public void IncrementManaCapacity(int playerNumber)
        {
            if (!IsServer) return;

            if (m_playerManaCap.ContainsKey(playerNumber) && m_playerManaCap[playerNumber] < m_maxManaCapacity)
            {
                m_playerManaCap[playerNumber] = Mathf.Min(m_playerManaCap[playerNumber] + m_incrementCapacityAmount, m_maxManaCapacity);
                m_clientManager.SetPlayersManaCapClientRpc(playerNumber, m_playerManaCap[playerNumber]);
            }
        }
        
        private void ModifyCurrentMana(int playerNumber, int cost)
        {
            if (!IsServer) return;

            if (!m_playerCurrentMana.ContainsKey(playerNumber)) return;
            
            m_playerCurrentMana[playerNumber] = Mathf.Clamp(m_playerCurrentMana[playerNumber] + cost, 0, m_playerManaCap[playerNumber]);
            m_clientManager.SetPlayersCurrentManaClientRpc(playerNumber, m_playerCurrentMana[playerNumber]);
        }
        
        private bool IsManaEnough(int playerNumber, int cost)
        {
            if (!IsServer) return false;
            if (m_playerCurrentMana.TryGetValue(playerNumber, out var value))
            {
                if (value >= cost) return true;
                else
                {
                    DebugWrapper.LogError($"Player {playerNumber} does not have enough mana");
                    return false;
                }
            }
            return false;
        }
        
        public bool TryConsumeMana(int playerNumber, int cost)
        {
            if (!IsServer) return false;
            if (IsManaEnough(playerNumber, cost))
            {
                ModifyCurrentMana(playerNumber, -cost);
                return true;
            }
            return false;
        }
        
        public void RefillMana(int playerNumber)
        {
            if (!IsServer) return;
            ModifyCurrentMana(playerNumber, m_playerManaCap[playerNumber]);
        }
    }
}
