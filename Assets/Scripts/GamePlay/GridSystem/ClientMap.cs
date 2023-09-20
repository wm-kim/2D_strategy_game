using System;
using System.Collections;
using System.Collections.Generic;
using Minimax.CoreSystems;
using Minimax.Utilities;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Minimax.GamePlay.GridSystem
{
    public class ClientMap : NetworkBehaviour
    {
#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] 
        private Transform m_coordTexts;
        [SerializeField, Tooltip("Debug Text Scale, Relative to Cell Size")]
        private float m_debugTextScale = 0.1f;
#endif
        
        [Header("References")]
        [SerializeField] private ClientCell m_clientCellPrefab;
        [SerializeField] private Tilemap m_tilemap;
        [SerializeField] private CameraController m_cameraController;
        
        public ClientCell SelectedClientCell { get; private set; }
        
        private List<ClientCell> m_highlightedCells = new List<ClientCell>();

#region Events
        /// <summary>
        /// Invoked when the touch is over the map
        /// </summary>
        public event Action<ClientCell> OnTouchOverMap;
        
        /// <summary>
        /// Invoked when the touch is outside the map
        /// </summary>
        public event System.Action OnTouchOutsideOfMap;
        
        /// <summary>
        /// Invoked when the touch is over the map and ended
        /// </summary>
        public event Action<ClientCell> OnTouchEndOverMap;
        
        public event Action<ClientCell> OnTapMap;
#endregion

        private ClientIsoGrid m_isoGrid;
        
        private void OnEnable()
        {
            GlobalManagers.Instance.Input.OnTouch += HoverCell;
            GlobalManagers.Instance.Input.OnTap += SelectCell;
        }

        private void OnDisable()
        {
            if (GlobalManagers.IsAvailable && GlobalManagers.Instance.Input != null)
            {
                GlobalManagers.Instance.Input.OnTouch -= HoverCell;
                GlobalManagers.Instance.Input.OnTap -= SelectCell;
            }
        }
        
        [ClientRpc]
        public void GenerateMapClientRpc(int mapSize, PathFinding pathFinding, GridRotation rotation, ClientRpcParams clientRpcParams = default)
        {
            m_isoGrid = new ClientIsoGrid(mapSize, mapSize, pathFinding, m_tilemap.cellSize, Vector3.zero,
                (grid, x, y) =>
                {
                    var cell = Instantiate(m_clientCellPrefab, grid.GetWorldPosFromCoord(x, y), Quaternion.identity, transform);
                    cell.Init(x, y);
                    return cell;
                });
            
#if UNITY_EDITOR
            m_isoGrid.GenerateDebugCoord(m_coordTexts, m_debugTextScale);
#endif
            
            // Set Rotation, need to be set after generating grid and debug coords
            SetRotation(rotation);
            
            // Set Camera boundary
            m_cameraController.SetCameraPositionAndBoundary(m_isoGrid.GetGridCenterPos(), m_isoGrid.GetSize());
        }
        
        private void SetRotation(GridRotation rotation) => m_isoGrid.SetRotation(rotation);

        public ClientCell this[Vector2Int coord] => m_isoGrid.Cells[coord.x, coord.y];
        
        /// <summary>
        /// Wrapper method for placing unit on the map.
        /// Also handles overlay visuals, if there is any overlay on the cell, it will be removed.
        /// </summary>
        public void PlaceUnitOnMap(int unitId, Vector2Int coord)
        {
            m_isoGrid.Cells[coord.x, coord.y].PlaceUnit(unitId);
        }
        
        private void HoverCell(Touch touch)
        {
            var worldPos = m_cameraController.Camera.ScreenToWorldPoint(touch.screenPosition);
        
            if (m_isoGrid.TryGetGridCellFromWorldIso(worldPos, out var cell))
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
            
            if (m_isoGrid.TryGetGridCellFromWorldIso(worldPos, out var cell))
            {
                DebugWrapper.Log($"Selected Cell: {cell}");
                SelectedClientCell = cell;
                OnTapMap?.Invoke(cell);
            }
        }
        
        public void HighlightReachableCells(ClientCell cell, int range)
        {
            DisableHighlightCells();
            m_highlightedCells = m_isoGrid.GetReachableCells(cell, range);
            DebugWrapper.Log($"Highlighting {m_highlightedCells.Count} cells, range: {range}");
            foreach (var clientCell in m_highlightedCells)
            {
                clientCell.HighlightAsReachable();
            }
        }
        
        public void DisableHighlightCells()
        {
            foreach (var clientCell in m_highlightedCells)
            {
                clientCell.DisableHighlight();
            }
        }
        
        public bool IsHighlightedCell(ClientCell cell)
        {
            return m_highlightedCells.Contains(cell);
        }
        
        /// <summary>
        /// Wrapper for <see cref="ClientIsoGrid.GetPath"/>
        /// </summary>
        public List<ClientCell> GetPath(Vector2Int start, Vector2Int target) => m_isoGrid.GetPath(start, target);
    }
}
