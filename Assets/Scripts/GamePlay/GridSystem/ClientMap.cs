using System;
using System.Collections;
using System.Collections.Generic;
using Minimax.CoreSystems;
using Minimax.Utilities;
using Unity.Netcode;
using UnityEngine;
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

        private IsoGrid m_isoGrid;
        
        private void Awake()
        {
            m_isoGrid = new IsoGrid(m_mapSize, m_mapSize, m_tilemap.cellSize, Vector3.zero,
                (grid, x, y) =>
                    new Cell(x, y, grid.GetWorldPosFromCoord(x, y)));
            
            // Set Camera boundary
            m_cameraController.SetCameraBoundary(m_isoGrid.GetGridCenterPos(), m_isoGrid.GetSize());
            
#if UNITY_EDITOR
            m_isoGrid.DebugCoord(m_coordTexts, m_debugTextScale);
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
        
        public void SetPlayersMapRotation()
        {
            if (!IsServer) return;

            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                var playerNumber = GlobalManagers.Instance.Connection.GetPlayerNumber(clientId);
                var clientRpcParam = GlobalManagers.Instance.Connection.ClientRpcParams[clientId];
                if (playerNumber == 0) SetRotationClientRpc(GridRotation.Default, clientRpcParam);
                else if (playerNumber == 1) SetRotationClientRpc(GridRotation.Rotate180, clientRpcParam);
            }
        }
        
        [ClientRpc]
        private void SetRotationClientRpc(GridRotation rotation, ClientRpcParams clientRpcParams = default)
        {
            m_isoGrid.SetRotation(rotation);
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
                SelectedCell = cell;
                OnTap?.Invoke(cell);
            }
        }
    }
}
