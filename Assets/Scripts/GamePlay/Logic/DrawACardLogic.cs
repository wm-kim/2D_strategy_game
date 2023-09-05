using System;
using System.Collections.Generic;
using Minimax.CoreSystems;
using Minimax.GamePlay.CommandSystem;
using Minimax.GamePlay.PlayerHand;
using Minimax.Utilities;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minimax.GamePlay.Logic
{
    public class CardDrawingLogic : NetworkBehaviour
    {
        [Header("Server References")]
        [SerializeField] private ServerPlayersDeckManager m_serverPlayersDeck;
        [SerializeField] private ServerPlayersHandManager m_serverPlayersHand;
        
        [Header("Client References")]
        [SerializeField] private ClientMyDeckManager m_clientMyDeck;
        [SerializeField] private ClientOpponentDeckManager m_clientOpponentDeck;
        [SerializeField] private ClientMyHandManager m_clientMyHand;
        [SerializeField] private ClientOpponentHandManager m_clientOpponentHand;
        
        public void CommandDrawAllPlayerInitialCards()
        {
            DebugWrapper.Log("Server is drawing initial cards for all players");
            var connectedClientIds = NetworkManager.Singleton.ConnectedClientsIds;
            try
            {
                Dictionary<int, int[]> cardUIDs = new Dictionary<int, int[]>();
                
                foreach (var clientId in connectedClientIds)
                {
                    var playerNumber = GlobalManagers.Instance.Connection.GetPlayerNumber(clientId);
                    cardUIDs.Add(playerNumber, new int[Define.InitialHandCardCount]);
                    for (int i = 0; i < Define.InitialHandCardCount; i++)
                    {
                        var cardUID = DrawACard(clientId);
                        cardUIDs[playerNumber][i] = cardUID;
                    }
                }
                
                var clientRpcParam = GlobalManagers.Instance.Connection.ClientRpcParams;
                foreach (var clientId in connectedClientIds)
                {
                    var playerNumber = GlobalManagers.Instance.Connection.GetPlayerNumber(clientId);
                    var opponentNumber = GlobalManagers.Instance.Connection.GetOpponentPlayerNumber(clientId);
                    DrawAllPlayerInitialCardsClientRpc(cardUIDs[playerNumber], cardUIDs[opponentNumber], clientRpcParam[clientId]);
                }
            }
            catch (Exception e)
            {
                DebugWrapper.LogError(e.Message);
            }
        }
        
        public void CommandDrawACardFromDeck(ulong clientId)
        {
            try
            {
                DebugWrapper.Log($"Drawing a card for client {clientId}");
                var cardUID = DrawACard(clientId);
                
                var clientRpcParam = GlobalManagers.Instance.Connection.ClientRpcParams[clientId];
                DrawACardClientRpc(cardUID, clientRpcParam);
            }
            catch (Exception e)
            {
                DebugWrapper.LogError(e.Message);
            }
        }

        private int DrawACard(ulong clientId)
        {
            var playerDeck = m_serverPlayersDeck.GetPlayerDeck(clientId);
            if (playerDeck.Count == 0)
            {
                throw new Exception($"client {clientId} has no more cards in deck.");
            }

            var card = playerDeck[0];
            var cardUID = card.UID;
            m_serverPlayersHand.AddCardToHand(card);
            playerDeck.RemoveAt(0);

            return cardUID;
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
            new DrawACardCommand(cardUID, m_clientMyHand, m_clientMyDeck).AddToQueue();
        }
        
        [ClientRpc]
        private void DrawAllPlayerInitialCardsClientRpc(int[] myCardUIDs, int[] opponentCardUIDs, ClientRpcParams clientRpcParams = default)
        {
            new DrawAllPlayerInitialCardsCommand(myCardUIDs, opponentCardUIDs, m_clientMyHand, m_clientOpponentHand, m_clientMyDeck, m_clientOpponentDeck)
                .AddToQueue();
        }
    }
}
