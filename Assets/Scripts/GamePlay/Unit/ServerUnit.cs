using System.Collections.Generic;
using Minimax.ScriptableObjects.CardDatas;
using Minimax.Utilities;
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
        
        public Vector2Int Coord { get; set; }
        
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
        
        public ServerUnit(int cardUID, Vector2Int coord)
        {
            UID = IDFactory.GetUniqueID();
            m_cardUID = cardUID;
            var serverCard = ServerCard.CardsCreatedThisGame[m_cardUID];
            var unitData = serverCard.Data as UnitBaseData;
            
            Health = unitData.Health;
            Attack = unitData.Attack;
            MoveRange = unitData.MoveRange;
            Coord = coord;
            
            UnitsCreatedThisGame.Add(UID, this);
            
            DebugWrapper.Log($"ServerUnit {UID} is created\n" +
                             $"Health: {Health}\n" +
                             $"Attack: {Attack}\n" +
                             $"MoveRange: {MoveRange}");
        }
        
        public int Health { get; set; }
        
        public int Attack { get; set; }
        
        public int MoveRange { get; set; }
        
        public bool IsMovable { get; set; } = true;
    }
}