using System.Collections.Generic;
using Minimax.GamePlay.Card;
using Unity.Netcode;

namespace Minimax.GamePlay.PlayerDeck
{
    public class ClientOpponentDeckManager : NetworkBehaviour
    {
        /// <summary>
        /// In clients deck card order does not matter, stores cardUIDs
        /// </summary>
        private HashSet<int> m_cardsInDeck = new();

        [ClientRpc]
        public void SetupOpponentDeckClientRpc(int[] cardUIds, ClientRpcParams clientRpcParams = default)
        {
            for (var i = 0; i < cardUIds.Length; i++)
            {
                var opponentPlayerNumber = TurnManager.Instance.OpponentPlayerNumber;
                new ClientCard(cardUIds[i], opponentPlayerNumber, null);
                m_cardsInDeck.Add(cardUIds[i]);
            }
        }

        public void RemoveCard(int cardUID)
        {
            m_cardsInDeck.Remove(cardUID);
        }
    }
}