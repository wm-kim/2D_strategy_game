using System.Collections;
using System.Collections.Generic;
using Minimax.GamePlay;
using Unity.Netcode;
using UnityEngine;

namespace Minimax
{
    public class ClientOpponentDeckManager : NetworkBehaviour
    {
        /// <summary>
        /// In clients deck card order does not matter, stores cardUIDs
        /// </summary>
        private HashSet<int> m_cardsInDeck = new HashSet<int>();

        [ClientRpc]
        public void SetupOpponentDeckClientRpc(int[] cardUIds, ClientRpcParams clientRpcParams = default)
        {
            for (int i = 0; i < cardUIds.Length; i++)
            {
                new ClientCard(cardUIds[i], false, null);
                m_cardsInDeck.Add(cardUIds[i]);
            }
        }
        
        public void RemoveCard(int cardUID)
        {
            m_cardsInDeck.Remove(cardUID);
        }
    }
}
