using System.Collections.Generic;
using Minimax.ScriptableObjects.CardDatas;
using Unity.Netcode;

namespace Minimax.GamePlay.PlayerHand
{
    public class ServerPlayersHandManager : NetworkBehaviour
    {
         private List<ServerCard> m_cardsInHand = new List<ServerCard>();

         public void AddCardToHand(ServerCard card)
         {
            if (!IsServer) return;
            m_cardsInHand.Add(card);
         }
    }
}
