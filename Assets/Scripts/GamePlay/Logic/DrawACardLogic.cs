using Minimax.CoreSystems;
using Minimax.GamePlay.CommandSystem;
using Minimax.GamePlay.PlayerHand;
using Minimax.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace Minimax.GamePlay.Logic
{
    /// <summary>
    /// Responsible for creating game commands
    /// </summary>
    public class CardDrawingLogic : NetworkBehaviour
    {
        [Header("Server References")]
        [SerializeField] private ServerPlayersDeckManager m_serverPlayersDeck;
        [SerializeField] private ServerPlayersHandManager m_serverPlayersHand;
        
        [Header("Client References")]
        [SerializeField] private ClientPlayerDeckManager m_clientPlayerDeck;
        [SerializeField] private ClientPlayerHandManager m_clientPlayerHand;
        
        public void DrawAllPlayerInitialCards()
        {
            DebugWrapper.Log("Drawing all players initial cards");
            var connectedClientIds = NetworkManager.Singleton.ConnectedClientsIds;
            foreach (var clientId in connectedClientIds)
            {
                for (int i = 0; i < Define.InitialHandCardCount; i++)
                {
                    DrawACard(clientId);
                }
            }
        }
        
        public void DrawACard(ulong clientId)
        {
            DebugWrapper.Log($"Drawing a card for player {clientId}");
            var playerNumber = GlobalManagers.Instance.Connection.GetPlayerNumber(clientId);
            var playerDeck = m_serverPlayersDeck.GetPlayerDeck(playerNumber);
            if (playerDeck.Count > 0)
            {
                var card = playerDeck[0];
                var cardUID = card.UID;
                m_serverPlayersHand.AddCardToHand(card);
                playerDeck.RemoveAt(0);

                var clientRpcParam = GlobalManagers.Instance.Connection.GetClientRpcParams(playerNumber);
                DrawACardClientRpc(cardUID, clientRpcParam);
            }
            else
            {
                DebugWrapper.Log($"Player {playerNumber} has no more cards in deck.");
            }
        }
        
        /// <summary>
        /// need to pass on cardUID to client so that it can be removed from the deck
        /// since, client have no information about the order of the deck.
        /// </summary>
        /// <param name="cardUID"></param>
        /// <param name="clientRpcParams"></param>
        [ClientRpc]
        private void DrawACardClientRpc(int cardUID, ClientRpcParams clientRpcParams = default)
        {
            new DrawACardCommand(cardUID, m_clientPlayerHand, m_clientPlayerDeck).AddToQueue();
        }
    }
}
