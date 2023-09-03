using System.Collections.Generic;
using Minimax.ScriptableObjects.CardDatas;
using UnityEngine;

namespace Minimax.GamePlay
{
    public class ClientCard : IIdentifiable
    {
        public int UniqueID;
        
        public CardBaseData Data;
        
        public int UID => UniqueID;
        
        /// <summary>
        /// a static dictionary of all the cards that have been created
        /// key: UniqueCardID, value: ServerCardLogic
        /// </summary>
        public static Dictionary<int, ClientCard> CardsCreatedThisGame = new Dictionary<int, ClientCard>();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            CardsCreatedThisGame.Clear();
        }
        
        public ClientCard(int uid, CardBaseData data)
        {
            UniqueID = uid;
            Data = data;
            CardsCreatedThisGame.Add(UniqueID, this);
        }
    }
}