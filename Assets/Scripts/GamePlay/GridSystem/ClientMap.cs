using System;
using System.Collections;
using System.Collections.Generic;
using Minimax.CoreSystems;
using Minimax.GamePlay.Unit;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Utilities;
using Debug = Utilities.Debug;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Minimax.GamePlay.GridSystem
{
    public class ClientMap : NetworkBehaviour
    {
#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField]
        private Transform m_coordTexts;

        [SerializeField]
        [Tooltip("Debug Text Scale, Relative to Cell Size")]
        private float m_debugTextScale = 0.1f;
#endif

        [Header("References")]
        [SerializeField]
        private ClientCell m_clientCellPrefab;

        [SerializeField]
        private Tilemap m_tilemap;

        [SerializeField]
        private CameraController m_cameraController;

        [Header("Settings")]
        [SerializeField]
        private bool m_displayDebugCoords = false;

        public ClientCell SelectedClientCell { get; private set; }

        private List<ClientCell> m_highlightedCells = new();

        private IReadOnlyDictionary<int, GridRotation> m_playerNumberToRotation = new Dictionary<int, GridRotation>()
        {
            { 0, GridRotation.Default },
            { 1, GridRotation.Rotate180 }
        };

        #region Events

        /// <summary>
        /// Invoked when the touch is over the map
        /// </summary>
        public event Action<ClientCell> OnTouchOverMap;

        /// <summary>
        /// Invoked when the touch is outside the map
        /// </summary>
        public event Action OnTouchOutsideOfMap;

        /// <summary>
        /// Invoked when the touch is over the map and ended
        /// </summary>
        public event Action<ClientCell> OnTouchEndOverMap;

        /// <summary>
        /// Invoked when the map is tapped
        /// </summary>
        public event Action<ClientCell> OnTapMap;

        #endregion

        private ClientIsoGrid m_isoGrid;

        private void OnEnable()
        {
            GlobalManagers.Instance.Input.OnTouch += HoverCell;
            GlobalManagers.Instance.Input.OnTap   += SelectCell;
        }

        private void OnDisable()
        {
            if (GlobalManagers.IsAvailable && GlobalManagers.Instance.Input != null)
            {
                GlobalManagers.Instance.Input.OnTouch -= HoverCell;
                GlobalManagers.Instance.Input.OnTap   -= SelectCell;
            }
        }

        [ClientRpc]
        public void GenerateMapClientRpc(int mapSize, PathFinding pathFinding, int playerNumber,
            ClientRpcParams clientRpcParams = default)
        {
            m_isoGrid = new ClientIsoGrid(mapSize, mapSize, pathFinding, m_tilemap.cellSize, Vector3.zero,
                (grid, x, y) =>
                {
                    var cell = Instantiate(m_clientCellPrefab, grid.GetWorldPosFromCoord(x, y), Quaternion.identity,
                        transform);
                    cell.Init(x, y);
                    return cell;
                });

#if UNITY_EDITOR
            m_isoGrid.GenerateDebugCoord(m_coordTexts, m_debugTextScale);
            m_isoGrid.DisplayDebugCoords(m_displayDebugCoords);
#endif

            // Set Rotation, need to be set after generating grid and debug coords
            SetRotation(m_playerNumberToRotation[playerNumber]);

            // Set Camera boundary
            m_cameraController.SetCameraPositionAndBoundary(m_isoGrid.GetGridCenterPos(), m_isoGrid.GetSize());
        }

        [ClientRpc]
        public void SetPlayersInitialPlaceableAreaClientRpc()
        {
            var myPlayerNumber       = TurnManager.Instance.MyPlayerNumber;
            var opponentPlayerNumber = TurnManager.Instance.OpponentPlayerNumber;
            var mapWidth             = m_isoGrid.Width;
            var mapHeight            = m_isoGrid.Height;

            if (myPlayerNumber == 0)
                for (var y = 0; y < mapHeight; y++)
                {
                    m_isoGrid.Cells[0, y].IsPlaceable = true;
                    m_isoGrid.Cells[0, y].CreateOverlay(OverlayType.Placeable, myPlayerNumber);
                    m_isoGrid.Cells[mapWidth - 1, y].CreateOverlay(OverlayType.Placeable, opponentPlayerNumber);
                }
            else if (myPlayerNumber == 1)
                for (var y = 0; y < mapHeight; y++)
                {
                    m_isoGrid.Cells[mapWidth - 1, y].IsPlaceable = true;
                    m_isoGrid.Cells[mapWidth - 1, y].CreateOverlay(OverlayType.Placeable, myPlayerNumber);
                    m_isoGrid.Cells[0, y].CreateOverlay(OverlayType.Placeable, opponentPlayerNumber);
                }
        }

        private void SetRotation(GridRotation rotation)
        {
            m_isoGrid.SetRotation(rotation);
        }

        public ClientCell this[Vector2Int coord] => m_isoGrid.Cells[coord.x, coord.y];

        /// <summary>
        /// Wrapper method for placing unit on the map.
        /// </summary>
        public void PlaceUnitOnMap(int unitId, Vector2Int coord)
        {
            m_isoGrid.Cells[coord.x, coord.y].PlaceUnit(unitId);
        }

        private void HoverCell(Touch touch)
        {
            if (EventSystem.current.IsPointerOverGameObject(touch.touchId))
                return;

            var worldPos = m_cameraController.Camera.ScreenToWorldPoint(touch.screenPosition);

            if (m_isoGrid.TryGetGridCellFromWorldIso(worldPos, out var cell))
            {
                if (touch.phase != UnityEngine.InputSystem.TouchPhase.Ended)
                    OnTouchOverMap?.Invoke(cell);
                else // TouchPhase.Ended
                    OnTouchEndOverMap?.Invoke(cell);
            }
            else
            {
                OnTouchOutsideOfMap?.Invoke();
            }
        }

        public bool TryGetCellFromTouchPos(Vector2 touchPosition, out ClientCell cell)
        {
            var worldPos = m_cameraController.Camera.ScreenToWorldPoint(touchPosition);
            return m_isoGrid.TryGetGridCellFromWorldIso(worldPos, out cell);
        }

        /// <summary>
        /// Selects the cell that is touched.
        /// </summary>
        private void SelectCell(Touch touch)
        {
            if (EventSystem.current.IsPointerOverGameObject(touch.touchId))
                return;

            var worldPos = m_cameraController.Camera.ScreenToWorldPoint(touch.screenPosition);

            if (m_isoGrid.TryGetGridCellFromWorldIso(worldPos, out var cell))
            {
                Debug.Log($"Selected Cell: {cell}");
                SelectedClientCell = cell;
                OnTapMap?.Invoke(cell);
            }
        }

        public void HighlightMovableCells(ClientCell cell, int range)
        {
            if (range <= 0) return;
            if (!TurnManager.Instance.IsMyTurn) return;

            var unitOwner = ClientUnit.UnitsCreatedThisGame[cell.CurrentUnitUID].Owner;
            m_highlightedCells = m_isoGrid.GetMovableCells(cell, range);
            foreach (var clientCell in m_highlightedCells) clientCell.Highlight(unitOwner);
        }

        public void HighlightAttackableCells(ClientCell cell, int range)
        {
            if (range <= 0) return;
            if (!TurnManager.Instance.IsMyTurn) return;

            var unitOwner = ClientUnit.UnitsCreatedThisGame[cell.CurrentUnitUID].Owner;
            m_highlightedCells = m_isoGrid.GetAttackableCells(cell, range);
            foreach (var clientCell in m_highlightedCells) clientCell.Highlight(unitOwner);
        }

        public void HighlightMovingPath(Vector2Int[] pathCoords, int unitOwner)
        {
            for (var i = 0; i < pathCoords.Length; i++)
            {
                var coord      = pathCoords[i];
                var clientCell = m_isoGrid.Cells[coord.x, coord.y];
                clientCell.Highlight(unitOwner);
                m_highlightedCells.Add(clientCell);
            }
        }

        public void DisableHighlightCells()
        {
            foreach (var clientCell in m_highlightedCells) clientCell.DisableHighlight();
        }

        public bool IsHighlightedCell(ClientCell cell)
        {
            return m_highlightedCells.Contains(cell);
        }

        /// <summary>
        /// Wrapper for <see cref="ClientIsoGrid.GetPath"/>
        /// </summary>
        public List<ClientCell> GetPath(Vector2Int start, Vector2Int target)
        {
            return m_isoGrid.GetPath(start, target);
        }

        public GridRotation GetMyPlayerRotation()
        {
            return m_isoGrid.PlayerRotation;
        }

        public GridRotation GetPlayerRotation(int playerNumber)
        {
            return m_playerNumberToRotation[playerNumber];
        }

        public GridRotation GetOpponentPlayerRotation()
        {
            var myPlayerNumber = TurnManager.Instance.MyPlayerNumber;

            // Assuming there are only two players: 0 and 1.
            var opponentPlayerNumber = myPlayerNumber == 0 ? 1 : 0;
            if (m_playerNumberToRotation.TryGetValue(opponentPlayerNumber, out var opponentRotation))
                return opponentRotation;
            throw new Exception($"Invalid Player Number: {opponentPlayerNumber} for getting opponent rotation");
        }
    }
}