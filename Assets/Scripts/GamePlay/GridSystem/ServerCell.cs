using System;
using System.Collections.Generic;
using Minimax.GamePlay.Unit;
using Minimax.UnityGamingService.Multiplayer;
using UnityEngine;
using Utilities;
using Debug = Utilities.Debug;

namespace Minimax.GamePlay.GridSystem
{
    public class ServerCell : IEquatable<ServerCell>, ICell
    {
        private int m_hash = -1;

        public Vector2Int Coord { get; private set; }

        public int CurrentUnitUID { get; private set; } = -1;

        /// <summary>
        /// Returns true if the cell is walkable.
        /// Cell이 유닛에 의해 점령되지 않았어도, 이동 불가능한 Cell일 수 있습니다.
        /// </summary>
        public bool IsWalkable { get; set; } = true;

        /// <summary>
        /// Returns true if the cell is placeable.
        /// </summary>
        public Dictionary<int, bool> IsPlaceable { get; set; }

        public ServerCell(int x, int y)
        {
            Coord       = new Vector2Int(x, y);
            IsPlaceable = new Dictionary<int, bool>();
            foreach (var playerNumber in SessionPlayerManager.Instance.GetAllPlayerNumbers())
                IsPlaceable.Add(playerNumber, false);
        }

        /// <summary>
        /// Checks if the cell is placeable by the given player and log error if not.
        /// </summary>
        /// <returns></returns>
        public bool CheckIfPlaceableBy(int playerNumber)
        {
            return Debug.CheckIfTrueLogError(IsPlaceable.ContainsKey(playerNumber),
                $"Cell {Coord} does not have player {playerNumber} in IsPlaceable dictionary");
        }

        public int GetDistance(ICell other)
        {
            return Mathf.Abs(Coord.x - other.Coord.x) + Mathf.Abs(Coord.y - other.Coord.y);
        }

        public void PlaceUnit(int unitUID)
        {
            CurrentUnitUID = unitUID;
            IsWalkable     = false;
        }

        public void RemoveUnit()
        {
            CurrentUnitUID = -1;
            IsWalkable     = true;
        }

        public bool Equals(ServerCell other)
        {
            return Coord.x == other.Coord.x && Coord.y == other.Coord.y;
        }

        public override bool Equals(object other)
        {
            return other is ServerCell && Equals(other as ServerCell);
        }

        public override int GetHashCode()
        {
            if (m_hash == -1)
            {
                m_hash = 23;

                m_hash = m_hash * 37 + Coord.x;
                m_hash = m_hash * 37 + Coord.y;
            }

            return m_hash;
        }

        public override string ToString()
        {
            return Coord.ToString();
        }
    }
}