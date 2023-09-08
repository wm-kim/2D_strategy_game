using System.Collections.Generic;
using Minimax.ScriptableObjects.CardDatas;
using UnityEngine;

namespace Minimax.GamePlay.Unit
{
    /// <summary>
    /// The Sole purpose of this class is to store the data of unit for visualization
    /// </summary>
    public class ClientUnit : IIdentifiable
    {
        public bool IsMine => ClientCard.CardsCreatedThisGame[m_cardUID].IsMine;
        
        /// <summary>
        /// for referencing to the card that this unit is created from
        /// </summary>
        private int m_cardUID;
        
        /// <summary>
        /// an ID for this unit instance, server and client share this ID to communicate
        /// </summary>
        public int UID { get; private set; }
        
        /// <summary>
        /// a static dictionary of all the cards that have been created
        /// key: UniqueCardID, value: ClientUnit
        /// </summary>
        public static Dictionary<int, ClientUnit> UnitsCreatedThisGame = new Dictionary<int, ClientUnit>();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            UnitsCreatedThisGame.Clear();
        }
        
        public ClientUnit(int unitUID, int cardUID)
        {
            UID = unitUID;
            m_cardUID = cardUID;
            var clientCard = ClientCard.CardsCreatedThisGame[m_cardUID];
            var unitData = clientCard.Data as UnitBaseData;
            
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