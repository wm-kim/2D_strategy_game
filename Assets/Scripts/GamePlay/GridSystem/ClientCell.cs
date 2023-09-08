using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Minimax
{
    /// <summary>
    /// Class representing a single field (cell) on the grid.   
    /// </summary>
    public class ClientCell : MonoBehaviour, IEquatable<ClientCell>
    {
        private int m_hash = -1;
        
        [SerializeField] private SpriteRenderer m_overlaySpriteRenderer;
        
        /// <summary>
        /// Coordinates of the cell on the grid.
        /// </summary>
        public Vector2Int Coord { get; private set; } 
        
        public int CurrentUnitUID { get; private set; } = -1;
        
        public void Init(int x, int y)
        {
            Coord = new Vector2Int(x, y);
            gameObject.name = $"Cell[{x},{y}]";
        }
        
        public void PlaceUnit(int unitUID)
        {
            CurrentUnitUID = unitUID;
        }

        public bool Equals(ClientCell other)
        {
            return Coord.x == other.Coord.x && Coord.y == other.Coord.y;
        }
        
        public override bool Equals(object other)
        {
            return (other is ClientCell) && Equals(other as ClientCell);
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
