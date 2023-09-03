using System.Collections;
using System.Collections.Generic;
using Minimax.ScriptableObjects.CardDatas;
using Minimax.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minimax
{
    [System.Serializable]
    public class ServerCard : IIdentifiable
    {
        /// <summary>
        /// the owner of this card (PlayerNumber)
        /// </summary>
        public int Owner;
        
        /// <summary>
        /// an ID for this card instance, server and client share this ID to communicate
        /// </summary>
        public int UniqueCardID;
        
        /// <summary>
        /// a reference to the card asset
        /// </summary>
        public CardBaseData Data;
        
        public int UID => UniqueCardID;

        /// <summary>
        /// a static dictionary of all the cards that have been created
        /// key: UniqueCardID, value: ServerCardLogic
        /// </summary>
        public static Dictionary<int, ServerCard> CardsCreatedThisGame = new Dictionary<int, ServerCard>();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            CardsCreatedThisGame.Clear();
        }
        
        public ServerCard(CardBaseData data)
        {
            Data = data;
            UniqueCardID = IDFactory.GetUniqueID();
            CardsCreatedThisGame.Add(UniqueCardID, this);
        }
    }
}
