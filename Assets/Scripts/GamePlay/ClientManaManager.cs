using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minimax.GamePlay
{
    public class ClientManaManager : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_myCurrentMana;
        [SerializeField] private TextMeshProUGUI m_myManaCap;
        [SerializeField] private TextMeshProUGUI m_opponentCurrentMana;
        [SerializeField] private TextMeshProUGUI m_opponentManaCap;
        
        [ClientRpc]
        public void SetPlayersCurrentManaClientRpc(int playerNumber, int currentMana)
        {
            if (playerNumber == TurnManager.Instance.MyPlayerNumber) 
                m_myCurrentMana.text = currentMana.ToString();
            else m_opponentCurrentMana.text = currentMana.ToString();
        }
        
        [ClientRpc]
        public void SetPlayersManaCapClientRpc(int playerNumber, int manaCap)
        {
            if (playerNumber == TurnManager.Instance.MyPlayerNumber) 
                m_myManaCap.text = manaCap.ToString();
            else m_opponentManaCap.text = manaCap.ToString();
        }
    }
}