using System.Collections;
using System.Collections.Generic;
using Minimax.ScriptableObjects.CardDatas;
using Minimax.Utilities;
using QFSW.QC;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minimax
{
    /// <summary>
    /// Holds the data of a card and related information for server
    /// </summary>
    [System.Serializable]
    public class ServerCard : IIdentifiable
    {
        /// <summary>
        /// the owner of this card (PlayerNumber)
        /// </summary>
        public int Owner { get; set; } = -1;
        
        /// <summary>
        /// a reference to the card asset, stores the copy of ScriptableObject CardBaseData
        /// </summary>
        public CardBaseData Data { get; private set; } = null;
        
        /// <summary>
        /// an ID for this card instance, server and client share this ID to communicate
        /// </summary>
        public int UID { get; private set; }

        /// <summary>
        /// a static dictionary of all the cards that have been created
        /// key: UniqueCardID, value: ServerCard
        /// </summary>
        public static Dictionary<int, ServerCard> CardsCreatedThisGame = new Dictionary<int, ServerCard>();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            CardsCreatedThisGame.Clear();
        }
        
        public ServerCard(CardBaseData data)
        {
            Data = data;
            UID = IDFactory.GetUniqueID();
            CardsCreatedThisGame.Add(UID, this);
        }
        
#if UNITY_EDITOR
        [Command("Server.Card.PrintAllCreated")]
        public static void PrintAllServerCards()
        {
            foreach (var card in CardsCreatedThisGame)
            {
                DebugWrapper.Log($"Card UID: {card.Key}, Card ID {card.Value.Data.CardId}");
            }
        }
#endif
    }
}