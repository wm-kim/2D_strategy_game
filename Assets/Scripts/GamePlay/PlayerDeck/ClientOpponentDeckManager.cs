using System.Collections;
using System.Collections.Generic;
using Minimax.GamePlay;
using Unity.Netcode;
using UnityEngine;

namespace Minimax
{
    public class ClientOpponentDeckManager : NetworkBehaviour
    {
        private Dictionary<int, ClientCard> m_cardsInDeck = new Dictionary<int, ClientCard>();

        [ClientRpc]
        public void SetupOpponentDeckClientRpc(int[] cardUIds, ClientRpcParams clientRpcParams = default)
        {
            for (int i = 0; i < cardUIds.Length; i++)
            {
                var clientCard = new ClientCard(cardUIds[i], null);
                m_cardsInDeck.Add(cardUIds[i], clientCard);
            }
        }
        
        public void RemoveCard(int cardUID)
        {
            m_cardsInDeck.Remove(cardUID);
        }
    }
}
