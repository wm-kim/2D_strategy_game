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
    public class Cell : IEquatable<Cell>
    {
        int m_hash = -1;
        private Vector2 m_offsetCoord;
        
        public Vector2 OffsetCoord { get => m_offsetCoord; set => m_offsetCoord = value; }
        
        public Cell(int x, int y)
        {
            m_offsetCoord = new Vector2(x, y);
        }
        
        public Cell(Vector2 offsetCoord)
        {
            m_offsetCoord = offsetCoord;
        }

        public bool Equals(Cell other)
        {
            return OffsetCoord.x == other.OffsetCoord.x && OffsetCoord.y == other.OffsetCoord.y;
        }
        
        public override bool Equals(object other)
        {
            return (other is Cell) && Equals(other as Cell);
        }

        public override int GetHashCode()
        {
            if (m_hash == -1)
            {
                m_hash = 23;

                m_hash = (m_hash * 37) + (int)OffsetCoord.x;
                m_hash = (m_hash * 37) + (int)OffsetCoord.y;
            }

            return m_hash;
        }
        
        public override string ToString()
        {
            return OffsetCoord.ToString();
        }
    }
}
