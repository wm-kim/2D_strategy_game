using System.Collections.Generic;
using Minimax.ScriptableObjects.CardDatas;
using Minimax.Utilities;
using QFSW.QC;
using UnityEngine;

namespace Minimax.GamePlay
{
    /// <summary>
    /// The Sole purpose of this class is to store the data of a card for visualization
    /// </summary>
    [System.Serializable]
    public class ClientCard : IIdentifiable
    {
        public bool IsMine { get; private set; } = false;
        
        /// <summary>
        /// Stores the copy of ScriptableObject CardBaseData
        /// This could be null if this card is opponent's card and we don't have the data of it
        /// Set this data after opponent's card is revealed from server
        /// </summary>
        public CardBaseData Data { get; set; } = null;
        
        /// <summary>
        /// an ID for this card instance, server and client share this ID to communicate
        /// </summary>
        public int UID { get; private set; }

        /// <summary>
        /// a static dictionary of all the cards that have been created
        /// key: UniqueCardID, value: ClientCard
        /// </summary>
        public static Dictionary<int, ClientCard> CardsCreatedThisGame = new Dictionary<int, ClientCard>();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            CardsCreatedThisGame.Clear();
        }
        
        public ClientCard(int cardUID, bool isMine, CardBaseData data = null)
        {
            UID = cardUID;
            IsMine = isMine;
            Data = data;
            CardsCreatedThisGame.Add(UID, this);
        }
        
#if UNITY_EDITOR
        [Command("Client.Card.PrintAllCreated")]
        public static void PrintAllClientCards()
        {
            foreach (var card in CardsCreatedThisGame)
            {
                DebugWrapper.Log($"Card UID: {card.Key}, Card ID {card.Value.Data.CardId}");
            }
        }
#endif
    }
}