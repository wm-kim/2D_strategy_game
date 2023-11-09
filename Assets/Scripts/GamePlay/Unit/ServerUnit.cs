using System.Collections.Generic;
using Minimax.GamePlay.Card;
using Minimax.ScriptableObjects.CardData;
using UnityEngine;
using System.Linq;
using Debug = Utilities.Debug;

namespace Minimax.GamePlay.Unit
{
    /// <summary>
    /// Holds the data of a unit and related methods for server
    /// </summary>
    public class ServerUnit : IIdentifiable
    {
        public int Owner { get; set; }

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
        public static Dictionary<int, ServerUnit> UnitsCreatedThisGame = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            UnitsCreatedThisGame.Clear();
        }

        public ServerUnit(int cardUID, Vector2Int coord)
        {
            UID       = IDFactory.GetUniqueID();
            m_cardUID = cardUID;
            var serverCard = ServerCard.CardsCreatedThisGame[m_cardUID];
            Owner = serverCard.Owner;

            var unitData = serverCard.Data as UnitBaseData;
            Health      = unitData.Health;
            Attack      = unitData.Attack;
            MoveRange   = unitData.MoveRange;
            AttackRange = unitData.AttackRange;
            Coord       = coord;

            UnitsCreatedThisGame.Add(UID, this);

            Debug.Log($"ServerUnit {UID} is created\n" +
                      $"Health: {Health}\n" +
                      $"Attack: {Attack}\n" +
                      $"MoveRange: {MoveRange}" +
                      $"AttackRange: {AttackRange}");
        }

        public int Health { get; set; }

        public int Attack { get; set; }

        public int MoveRange { get; set; }

        public int AttackRange { get; set; }

        public void ResetOnTurnStart()
        {
            MoveRange = ((UnitBaseData)ServerCard.CardsCreatedThisGame[m_cardUID].Data).MoveRange;
        }

        /// <summary>
        /// Checks if the unit is movable, and log if it is not.
        /// </summary>
        public bool CheckIfMovable()
        {
            return Debug.CheckIfTrueLog(MoveRange > 0, $"Unit {UID} is not movable");
        }

        public static List<ServerUnit> GetAllUnitsByOwner(int playerNumber)
        {
            return UnitsCreatedThisGame.Values.Where(unit => unit.Owner == playerNumber).ToList();
        }
    }
}