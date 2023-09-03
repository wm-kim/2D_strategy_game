using System;
using System.Collections;
using System.Collections.Generic;
using Minimax.CoreSystems;
using Minimax.Utilities;
using UnityEngine;
using UnityEngine.Tilemaps;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Minimax.GamePlay.GridSystem
{
    public class Map : MonoBehaviour
    {
#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] 
        private Transform m_coordTexts;
        [SerializeField, Tooltip("Debug Text Scale, Relative to Cell Size")]
        private float m_debugTextScale = 0.1f;
#endif
        
        [Header("References")]
        [SerializeField] private Tilemap m_tilemap;
        [SerializeField] private CameraController m_cameraController;
        
        [Header("Settings")]
        [SerializeField] private int m_mapSize = 11;
        
        public Cell SelectedCell { get; private set; }

#region Events
        /// <summary>
        /// Invoked when the touch is over the map
        /// </summary>
        public event Action<Cell> OnTouchOverMap;
        
        /// <summary>
        /// Invoked when the touch is outside the map
        /// </summary>
        public event System.Action OnTouchOutsideOfMap;
        
        /// <summary>
        /// Invoked when the touch is over the map and ended
        /// </summary>
        public event Action<Cell> OnTouchEndOverMap;
        
        public event Action<Cell> OnTap;
#endregion

        private Grid<Cell> m_grid;
        
        private void Awake()
        {
            m_grid = new Grid<Cell>(m_mapSize, m_mapSize, m_tilemap.cellSize, Vector3.zero,
                (grid, x, y) =>
                    new Cell(x, y, grid.GetWorldPosFromCoord(x, y)));
            
            // Set Camera boundary
            m_cameraController.SetCameraBoundary(m_grid.GetGridCenterPos(), m_grid.GetSize());
            
#if UNITY_EDITOR
            m_grid.DebugCoord(m_coordTexts, m_debugTextScale);
#endif
        }

        private void OnEnable()
        {
            GlobalManagers.Instance.Input.OnTouch += HoverCell;
            GlobalManagers.Instance.Input.OnTap += SelectCell;
        }

        private void OnDisable()
        {
            if (GlobalManagers.Instance != null && GlobalManagers.Instance.Input != null)
            {
                GlobalManagers.Instance.Input.OnTouch -= HoverCell;
                GlobalManagers.Instance.Input.OnTap -= SelectCell;
            }
        }
        
        private void HoverCell(Touch touch)
        {
            var worldPos = m_cameraController.Camera.ScreenToWorldPoint(touch.screenPosition);
        
            if (m_grid.TryGetGridCellFromWorldIso(worldPos, out var cell))
            {
                if (touch.phase != UnityEngine.InputSystem.TouchPhase.Ended)
                {
                    OnTouchOverMap?.Invoke(cell);
                }
                else
                {
                    OnTouchEndOverMap?.Invoke(cell);
                }
            }
            else 
            {
                OnTouchOutsideOfMap?.Invoke();
            }
        }
        
        /// <summary>
        /// Selects the cell that is touched.
        /// </summary>
        private void SelectCell(Vector2 touchPosition)
        {
            var worldPos = m_cameraController.Camera.ScreenToWorldPoint(touchPosition);
            
            if (m_grid.TryGetGridCellFromWorldIso(worldPos, out var cell))
            {
                SelectedCell = cell;
                OnTap?.Invoke(cell);
            }
        }
    }
}
