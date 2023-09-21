using System;
using DG.Tweening;
using UnityEngine;

namespace Minimax.GamePlay.GridSystem
{
    /// <summary>
    /// Class representing a single field (cell) on the grid.   
    /// </summary>
    public class ClientCell : MonoBehaviour, IEquatable<ClientCell>, ICell
    {
        private int m_hash = -1;
        
        [SerializeField] private SpriteRenderer m_overlaySpriteRenderer;
        
        [Header("Settings")]
        [SerializeField, Range(0, 1)]
        private float m_overlayAlpha = 0.30f;
        
        [SerializeField, Range(0, 1)]
        private float m_overlayFadeDuration = 0.2f;
        
        /// <summary>
        /// Coordinates of the cell on the grid.
        /// </summary>
        public Vector2Int Coord { get; private set; }
        
        private Tween m_overlayFadeTween;

        public int GetDistance(ICell other)
        {
            return Mathf.Abs(Coord.x - other.Coord.x) + Mathf.Abs(Coord.y - other.Coord.y);
        }

        public int CurrentUnitUID { get; private set; } = -1;
        
        /// <summary>
        /// Returns true if the cell is occupied by a unit.
        /// </summary>
        public bool IsOccupiedByUnit => CurrentUnitUID != -1;
        
        /// <summary>
        /// Returns true if the cell is walkable.
        /// Cell이 유닛에 의해 점령되지 않았어도, 이동 불가능한 Cell일 수 있습니다.
        /// </summary>
        public bool IsWalkable => CurrentUnitUID == -1;

        private void Awake()
        {
            m_overlaySpriteRenderer.color = new Color(1, 1, 1, 0);
        }

        public void Init(int x, int y)
        {
            Coord = new Vector2Int(x, y);
            gameObject.name = $"Cell[{x},{y}]";
        }
        
        public void PlaceUnit(int unitUID)
        {
            CurrentUnitUID = unitUID;
        }
        
        public void RemoveUnit()
        {
            CurrentUnitUID = -1;
        }
        
        public void Highlight()
        {
            m_overlayFadeTween?.Kill();
            m_overlayFadeTween = m_overlaySpriteRenderer.DOFade(m_overlayAlpha, m_overlayFadeDuration);
        }
        
        public void DisableHighlight()
        {
            m_overlayFadeTween?.Kill();
            m_overlayFadeTween = m_overlaySpriteRenderer.DOFade(0, m_overlayFadeDuration);
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
