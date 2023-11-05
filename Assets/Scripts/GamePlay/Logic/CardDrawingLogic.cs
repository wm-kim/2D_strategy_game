using System;
using System.Collections.Generic;
using Minimax.CoreSystems;
using Minimax.Definitions;
using Minimax.GamePlay.CommandSystem;
using Minimax.GamePlay.PlayerDeck;
using Minimax.GamePlay.PlayerHand;
using Minimax.UnityGamingService.Multiplayer;
using Minimax.Utilities;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minimax.GamePlay.Logic
{
    /// <summary>
    /// Responsible for generating Card Drawing Commands and sending them to the clients.
    /// </summary>
    public class CardDrawingLogic : NetworkBehaviour
    {
        [Header("Server References")] [SerializeField]
        private ServerPlayersDeckManager m_serverPlayersDeck;

        [SerializeField] private ServerPlayersHandManager m_serverPlayersHand;

        [Header("Client References")] [SerializeField]
        private ClientMyDeckManager m_clientMyDeck;

        [SerializeField] private ClientOpponentDeckManager m_clientOpponentDeck;
        [SerializeField] private ClientMyHandManager       m_clientMyHand;
        [SerializeField] private ClientOpponentHandManager m_clientOpponentHand;

        public void CommandDrawAllPlayerInitialCards()
        {
            DebugWrapper.Log("Server is drawing initial cards for all players");
            var sessionPlayers  = SessionPlayerManager.Instance;
            var clientRpcParams = sessionPlayers.ClientRpcParams;

            var cardUIDs      = new Dictionary<int, int[]>();
            var playerNumbers = sessionPlayers.GetAllPlayerNumbers();

            foreach (var playerNumber in playerNumbers)
            {
                cardUIDs.Add(playerNumber, new int[Define.InitialHandCardCount]);
                for (var i = 0; i < Define.InitialHandCardCount; i++)
                {
                    var cardUID = DrawACard(playerNumber);
                    cardUIDs[playerNumber][i] = cardUID;
                }
            }

            foreach (var playerNumber in playerNumbers)
            {
                var opponentNumber = sessionPlayers.GetOpponentPlayerNumber(playerNumber);
                DrawAllPlayerInitialCardsClientRpc(cardUIDs[playerNumber], cardUIDs[opponentNumber],
                    clientRpcParams[playerNumber]);
            }
        }

        public void CommandDrawACardFromDeck(int playerNumber)
        {
            try
            {
                DebugWrapper.Log($"Drawing a card for player {playerNumber}");
                var sessionPlayers  = SessionPlayerManager.Instance;
                var clientRpcParams = sessionPlayers.ClientRpcParams;
                var opponentNumber  = sessionPlayers.GetOpponentPlayerNumber(playerNumber);
                var cardUID         = DrawACard(playerNumber);

                DrawMyCardClientRpc(cardUID, clientRpcParams[playerNumber]);
                DrawOpponentCardClientRpc(cardUID, clientRpcParams[opponentNumber]);
            }
            catch (Exception e)
            {
                DebugWrapper.LogError(e.Message);
            }
        }

        private int DrawACard(int playerNumber)
        {
            var playerDeck = m_serverPlayersDeck.GetPlayerDeck(playerNumber);
            if (!m_serverPlayersDeck.IsCardLeftInDeck(playerNumber))
                throw new Exception($"Player {playerNumber} has no more cards in deck.");

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
            new DrawMyCardCmd(cardUID, m_clientMyHand, m_clientMyDeck).AddToQueue();
        }

        [ClientRpc]
        private void DrawOpponentCardClientRpc(int cardUID, ClientRpcParams clientRpcParams = default)
        {
            new DrawOpponentCardCmd(cardUID, m_clientOpponentHand, m_clientOpponentDeck).AddToQueue();
        }

        [ClientRpc]
        private void DrawAllPlayerInitialCardsClientRpc(int[] myCardUIDs, int[] opponentCardUIDs,
            ClientRpcParams clientRpcParams = default)
        {
            new DrawAllPlayerInitialCardsCmd(myCardUIDs, opponentCardUIDs, m_clientMyHand, m_clientOpponentHand,
                    m_clientMyDeck, m_clientOpponentDeck)
                .AddToQueue();
        }
    }
}