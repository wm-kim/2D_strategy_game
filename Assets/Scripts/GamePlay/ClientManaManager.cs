using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = Utilities.Debug;

namespace Minimax.GamePlay
{
    public class ClientManaManager : NetworkBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_myCurrentManaText;

        [SerializeField]
        private TextMeshProUGUI m_myManaCapText;

        [SerializeField]
        private TextMeshProUGUI m_opponentCurrentManaText;

        [SerializeField]
        private TextMeshProUGUI m_opponentManaCapText;

        private int m_myCurrentMana;
        private int m_myManaCap;
        private int m_opponentCurrentMana;
        private int m_opponentManaCap;

        [ClientRpc]
        public void SetPlayersCurrentManaClientRpc(int playerNumber, int currentMana)
        {
            if (playerNumber == TurnManager.Instance.MyPlayerNumber)
            {
                m_myCurrentManaText.text = currentMana.ToString();
                m_myCurrentMana          = currentMana;
            }
            else
            {
                m_opponentCurrentManaText.text = currentMana.ToString();
                m_opponentCurrentMana          = currentMana;
            }
        }

        [ClientRpc]
        public void SetPlayersManaCapClientRpc(int playerNumber, int manaCap)
        {
            if (playerNumber == TurnManager.Instance.MyPlayerNumber)
            {
                m_myManaCapText.text = manaCap.ToString();
                m_myManaCap          = manaCap;
            }
            else
            {
                m_opponentManaCapText.text = manaCap.ToString();
                m_opponentManaCap          = manaCap;
            }
        }

        // For client to check if it has enough mana to play a card
        public bool CheckIfMyManaIsEnough(int cost)
        {
            return Debug.CheckIfTrueLog(m_myCurrentMana >= cost, "Not enough mana to play this card");
        }
    }
}