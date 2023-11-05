using System;
using System.Collections.Generic;
using Minimax.CoreSystems;
using Minimax.GamePlay.Card;
using Minimax.UnityGamingService.Multiplayer;
using QFSW.QC;
using Unity.Netcode;
using Utilities;

namespace Minimax.GamePlay.PlayerHand
{
    public class ServerPlayersHandManager : NetworkBehaviour
    {
        /// <summary>
        /// key is player number, value is a list of cards UID in hand
        /// </summary>
        private Dictionary<int, List<int>> m_plyersCardInHand = new();

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;

            foreach (var playerNumber in SessionPlayerManager.Instance.GetAllPlayerNumbers())
                m_plyersCardInHand.Add(playerNumber, new List<int>());

            base.OnNetworkSpawn();
        }

        public void AddCardToHand(int playerNumber, int cardUID)
        {
            if (!IsServer) return;
            m_plyersCardInHand[playerNumber].Add(cardUID);
        }

        public void RemoveCardFromHand(int playerNumber, int cardUID)
        {
            if (!IsServer) return;

            foreach (var card in m_plyersCardInHand[playerNumber])
                if (card == cardUID)
                {
                    m_plyersCardInHand[playerNumber].Remove(card);
                    return;
                }

            DebugWrapper.LogError($"Card with UID {cardUID} not found in player {playerNumber} hand");
        }

#if UNITY_EDITOR
        [Command("Server.Hand.PrintAll", MonoTargetType.All)]
        public void PrintAllPlayerHands()
        {
            foreach (var player in m_plyersCardInHand)
            {
                DebugWrapper.Log($"Player Number {player.Key} has {player.Value.Count} cards in hand");
                foreach (var cardUID in player.Value)
                    DebugWrapper.Log(
                        $"Card UID: {cardUID}, Card ID {ServerCard.CardsCreatedThisGame[cardUID].Data.CardId}");
            }
        }
#endif
    }
}