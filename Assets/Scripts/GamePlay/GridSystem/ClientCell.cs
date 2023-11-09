using System;
using System.Collections.Generic;
using DG.Tweening;
using Minimax.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;
using Debug = Utilities.Debug;

namespace Minimax.GamePlay.GridSystem
{
    public enum OverlayType
    {
        Highlight,
        Placeable
    }

    /// <summary>
    /// Class representing a single field (cell) on the grid.   
    /// </summary>
    public class ClientCell : MonoBehaviour, IEquatable<ClientCell>, ICell
    {
        [Header("References")]
        [SerializeField]
        private SpriteRenderer m_highlightOverlayPrefab;

        private Dictionary<OverlayType, SpriteRenderer> m_overlays = new();

        [SerializeField]
        private Color m_baseOverlayColor = Color.white;

        [SerializeField]
        private Color m_myHighlightColor = Color.green;

        [SerializeField]
        private Color m_opponentHighlightColor = Color.red;

        [Header("Settings")]
        [SerializeField]
        [Range(0, 1)]
        private float m_overlayAlpha = 0.30f;

        [SerializeField]
        [Range(0, 1)]
        private float m_overlayFadeDuration = 0.2f;

        private int m_hash = -1;

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
        public bool IsWalkable { get; set; } = true;

        /// <summary>
        /// Returns true if the cell is placeable.
        /// </summary>
        public bool IsPlaceable { get; set; } = false;

        private void Awake()
        {
            CreateOverlay(OverlayType.Highlight);
        }

        public void Init(int x, int y)
        {
            Coord           = new Vector2Int(x, y);
            gameObject.name = $"Cell[{x},{y}]";
        }

        public void CreateOverlay(OverlayType overlayType, int? playerNumber = null)
        {
            var overlay      = Instantiate(m_highlightOverlayPrefab, transform);
            var overlayColor = GetOverlayColor(playerNumber);
            overlay.color = overlayColor;
            m_overlays.Add(overlayType, overlay);
        }

        private Color GetOverlayColor(int? playerNumber)
        {
            var overlayColor = playerNumber == null
                ? m_baseOverlayColor
                : playerNumber == TurnManager.Instance.MyPlayerNumber
                    ? m_myHighlightColor
                    : m_opponentHighlightColor;
            return overlayColor;
        }

        /// <summary>
        /// Checks if the cell is placeable and logs if it is not.
        /// </summary>
        /// <returns></returns>
        public bool CheckIfPlaceable()
        {
            return Debug.CheckIfTrueLogError(IsPlaceable, $"Cell {Coord} is not placeable");
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

        public void Highlight(int unitOwner)
        {
            m_overlayFadeTween?.Kill();
            m_overlays[OverlayType.Highlight].color = GetOverlayColor(unitOwner);
            m_overlayFadeTween = m_overlays[OverlayType.Highlight].DOFade(m_overlayAlpha, m_overlayFadeDuration);
        }

        public void DisableHighlight()
        {
            m_overlayFadeTween?.Kill();
            m_overlayFadeTween = m_overlays[OverlayType.Highlight].DOFade(0, m_overlayFadeDuration);
        }

        public bool Equals(ClientCell other)
        {
            return Coord.x == other.Coord.x && Coord.y == other.Coord.y;
        }

        public override bool Equals(object other)
        {
            return other is ClientCell && Equals(other as ClientCell);
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