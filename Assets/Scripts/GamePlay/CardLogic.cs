using System.Collections;
using System.Collections.Generic;
using Minimax.ScriptableObjects.CardDatas;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minimax
{
    [System.Serializable]
    public class CardLogic : IIdentifiable
    {
        /// <summary>
        /// the owner of this card (PlayerNumber)
        /// </summary>
        public int Owner;
        
        /// <summary>
        /// an ID for this card instance
        /// </summary>
        public int UniqueCardID;
        
        /// <summary>
        /// a reference to the card asset
        /// </summary>
        public CardBaseData Data;
        
        public int ID { get { return UniqueCardID; } }
        
        /// <summary>
        /// a static dictionary of all the cards that have been created
        /// </summary>
        public static Dictionary<int, CardLogic> CardsCreatedThisGame = new Dictionary<int, CardLogic>();
        
        public CardLogic(CardBaseData data)
        {
            Data = data;
            UniqueCardID = IDFactory.GetUniqueID();
            CardsCreatedThisGame.Add(UniqueCardID, this);
        }
    }
}
