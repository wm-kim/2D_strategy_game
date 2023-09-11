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
            
                Dictionary<int, int[]> cardUIDs = new Dictionary<int, int[]>();
                
                foreach (var clientId in connectedClientIds)
                {
                    var playerNumber = GlobalManagers.Instance.Connection.GetPlayerNumber(clientId);
                    cardUIDs.Add(playerNumber, new int[Define.InitialHandCardCount]);
                    for (int i = 0; i < Define.InitialHandCardCount; i++)
                    {
                        var cardUID = DrawACard(playerNumber);
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
        
        public void CommandDrawACardFromDeck(ulong clientId)
        {
            try
            {
                DebugWrapper.Log($"Drawing a card for client {clientId}");
                var connection = GlobalManagers.Instance.Connection;
                var playerNumber = connection.GetPlayerNumber(clientId);
                var opponentNumber = connection.GetOpponentPlayerNumber(clientId);
                var cardUID = DrawACard(playerNumber);
                
                DrawMyCardClientRpc(cardUID, connection.ClientRpcParams[playerNumber]);
                DrawOpponentCardClientRpc(cardUID, connection.ClientRpcParams[opponentNumber]);
            }
            catch (Exception e)
            {
                DebugWrapper.LogError(e.Message);
            }
        }

        private int DrawACard(int playerNumber)
        {
            var playerDeck = m_serverPlayersDeck.GetPlayerDeck(playerNumber);
            if (playerDeck.Count == 0)
            {
                throw new Exception($"Player {playerNumber} has no more cards in deck.");
            }

            var cardUID = playerDeck[0];
            m_serverPlayersHand.AddCardToHand(playerNumber, cardUID);
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
        private void DrawMyCardClientRpc(int cardUID, ClientRpcParams clientRpcParams = default)
        {
            new DrawMyCardCommand(cardUID, m_clientMyHand, m_clientMyDeck).AddToQueue();
        }
        
        [ClientRpc]
        private void DrawOpponentCardClientRpc(int cardUID, ClientRpcParams clientRpcParams = default)
        {
            new DrawOpponentCardCommand(cardUID, m_clientOpponentHand, m_clientOpponentDeck).AddToQueue();
        }
        
        [ClientRpc]
        private void DrawAllPlayerInitialCardsClientRpc(int[] myCardUIDs, int[] opponentCardUIDs, ClientRpcParams clientRpcParams = default)
        {
            new DrawAllPlayerInitialCardsCommand(myCardUIDs, opponentCardUIDs, m_clientMyHand, m_clientOpponentHand, m_clientMyDeck, m_clientOpponentDeck)
                .AddToQueue();
        }
    }
}