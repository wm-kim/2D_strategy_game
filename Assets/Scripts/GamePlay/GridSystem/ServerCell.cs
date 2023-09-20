using System;
using Minimax.GamePlay.Unit;
using UnityEngine;

namespace Minimax.GamePlay.GridSystem
{
    public class ServerCell : IEquatable<ServerCell>, ICell
    {
        private int m_hash = -1;
        
        public Vector2Int Coord { get; private set; }

        public int CurrentUnitUID { get; private set; } = -1;
        
        public bool IsWalkable => CurrentUnitUID == -1;
        
        public ServerCell(int x, int y)
        {
            Coord = new Vector2Int(x, y);
        }
        
        public int GetDistance(ICell other)
        {
            return Mathf.Abs(Coord.x - other.Coord.x) + Mathf.Abs(Coord.y - other.Coord.y);
        }
        
        public void PlaceUnit(int unitUID)
        {
            CurrentUnitUID = unitUID;
        }
        
        public void RemoveUnit()
        {
            CurrentUnitUID = -1;
        }
        
        public bool Equals(ServerCell other)
        {
            return Coord.x == other.Coord.x && Coord.y == other.Coord.y;
        }
        
        public override bool Equals(object other)
        {
            return (other is ServerCell) && Equals(other as ServerCell);
        }
        
        public override int GetHashCode()
        {
            if (m_hash == -1)
            {
                m_hash = 23;
                
                m_hash = (m_hash * 37) + Coord.x;
                m_hash = (m_hash * 37) + Coord.y;
            }
            
            return m_hash;
        }
        
        public override string ToString()
        {
            return Coord.ToString();
        }
    }
}
    