using Minimax.CoreSystems;
using Minimax.GamePlay.PlayerHand;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minimax.GamePlay.Command
{
    /// <summary>
    /// Responsible for creating game commands 
    /// </summary>
    public class GameCommandFactory : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private ServerPlayersDeckManager m_playerDecks;
        [SerializeField] private ServerPlayersHandManager m_playerHands;
        
        /// <summary>
        /// This method is called by the server to draw a card for the given player.
        /// </summary>
        [ServerRpc]
        public void DrawACardServerRpc(ServerRpcParams serverRpcParams = default)
        {
            var clientId = serverRpcParams.Receive.SenderClientId;
            var playerNumber = GlobalManagers.Instance.Connection.GetPlayerNumber(clientId);
            var playerDeck = m_playerDecks.GetPlayerDeck(playerNumber);
            if (playerDeck.Count > 0)
            {
                CardLogic card = new CardLogic(playerDeck[0])
                {
                    Owner = playerNumber
                };
                m_playerHands.AddCardToHand(card);
                playerDeck.RemoveAt(0);
                
            }
        }
        
        [ClientRpc]
        public void DrawACardClientRpc(int cardId, ClientRpcParams clientRpcParams = default)
        {
            // Create a command to draw a card
        }
    }
}
