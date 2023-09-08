using System.Collections.Generic;
using Minimax.ScriptableObjects.CardDatas;
using UnityEngine;

namespace Minimax.GamePlay.Unit
{
    /// <summary>
    /// Holds the data of a unit and related methods for server
    /// </summary>
    public class ServerUnit : IIdentifiable
    {
        /// <summary>
        /// for referencing the card that this unit is created from
        /// </summary>
        private int m_cardUID;
        
        /// <summary>
        /// an ID for this unit instance, server and client share this ID to communicate
        /// </summary>
        public int UID { get; private set; }
        
        /// <summary>
        /// a static dictionary of all the cards that have been created
        /// key: UniqueCardID, value: ServerUnit
        /// </summary>
        public static Dictionary<int, ServerUnit> UnitsCreatedThisGame = new Dictionary<int, ServerUnit>();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            UnitsCreatedThisGame.Clear();
        }
        
        public ServerUnit(int cardUID)
        {
            UID = IDFactory.GetUniqueID();
            m_cardUID = cardUID;
            var serverCard = ServerCard.CardsCreatedThisGame[m_cardUID];
            var unitData = serverCard.Data as UnitBaseData;
            
            Health = unitData.Health;
            Attack = unitData.Attack;
            Movement = unitData.Movement;
            
            UnitsCreatedThisGame.Add(UID, this);
        }
        
        public int Health { get; private set; }
        
        public int Attack { get; private set; }
        
        public int Movement { get; private set; }
    }
}