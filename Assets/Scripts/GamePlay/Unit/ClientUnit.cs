using System.Collections.Generic;
using Minimax.GamePlay.Card;
using Minimax.ScriptableObjects.CardData;
using UnityEngine;
using Utilities;

namespace Minimax.GamePlay.Unit
{
    /// <summary>
    /// The Sole purpose of this class is to store the data of unit for visualization
    /// </summary>
    public class ClientUnit : IIdentifiable
    {
        public int Owner { get; set; }

        public bool IsMine => Owner == TurnManager.Instance.MyPlayerNumber;

        /// <summary>
        /// for referencing to the card that this unit is created from
        /// </summary>
        private int m_cardUID;

        /// <summary>
        /// an ID for this unit instance, server and client share this ID to communicate
        /// </summary>
        public int UID { get; private set; }

        public Vector2Int Coord { get; set; }

        /// <summary>
        /// a static dictionary of all the cards that have been created
        /// key: UniqueCardID, value: ClientUnit
        /// </summary>
        public static Dictionary<int, ClientUnit> UnitsCreatedThisGame = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            UnitsCreatedThisGame.Clear();
        }

        public ClientUnit(int unitUID, int cardUID, Vector2Int coord)
        {
            UID       = unitUID;
            m_cardUID = cardUID;
            var clientCard = ClientCard.CardsCreatedThisGame[m_cardUID];
            Owner = clientCard.Owner;
            var unitData = clientCard.Data as UnitBaseData;

            Health      = unitData.Health;
            Attack      = unitData.Attack;
            MoveRange   = unitData.MoveRange;
            AttackRange = unitData.AttackRange;

            Coord = coord;

            UnitsCreatedThisGame.Add(UID, this);

            DebugWrapper.Log($"ClientUnit {UID} is created\n" +
                             $"Health: {Health}\n" +
                             $"Attack: {Attack}\n" +
                             $"MoveRange: {MoveRange}\n" +
                             $"AttackRange: {AttackRange}\n");
        }

        public int Health { get; set; }

        public int Attack { get; set; }

        public int MoveRange { get; set; }

        public int AttackRange { get; set; }

        /// <summary>
        /// 플레이어가 중복하여 이동 명령을 내리는 것을 방지하기 위한 플래그
        /// </summary>
        public bool IsMovable { get; set; } = true;

        /// <summary>
        /// Checks if the unit is movable, and log if it is not.
        /// </summary>
        public bool CheckIfMovable()
        {
            if (!IsMovable)
            {
                DebugWrapper.LogError($"Unit {UID} is not movable");
                return false;
            }

            return true;
        }
    }
}