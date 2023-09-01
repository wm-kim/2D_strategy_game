using System.Collections.Generic;
using Minimax.ScriptableObjects.CardDatas;
using Unity.Netcode;

namespace Minimax.GamePlay.PlayerHand
{
    public class ServerPlayersHandManager : NetworkBehaviour
    {
         private Dictionary<int, CardLogic> m_cardsInHand = new Dictionary<int, CardLogic>();

         public void AddCardToHand(CardLogic card)
         {
            if (!IsServer) return;
            m_cardsInHand.Add(card.ID, card);
         }
    }
}
