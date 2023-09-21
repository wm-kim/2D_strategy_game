using System;
using System.Collections;
using System.Collections.Generic;
using Minimax.GamePlay;
using Minimax.GamePlay.GridSystem;
using Minimax.GamePlay.Logic;
using Minimax.GamePlay.Unit;
using Minimax.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minimax
{
    /// <summary>
    /// Responsible for spawning units and visualizing them
    /// </summary>
    public class ClientUnitManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UnitVisual m_unitVisualPrefab;
        [SerializeField] private ClientMap m_clientMap;
        [SerializeField] private Transform m_unitContainer;
        [SerializeField] private UnitLogic m_unitLogic;
        [SerializeField] private TurnManager m_turnManager;
        
        public event Action<ClientCell> OnUnitSpawned;
        
        /// <summary>
        /// Key is unit UID
        /// </summary>
        private Dictionary<int, UnitVisual> m_unitVisuals = new Dictionary<int, UnitVisual>();
        private int m_selectedUnitUID = -1;

        private void OnEnable()
        {
            m_clientMap.OnTapMap += OnTabMap;
        }
        
        private void OnDisable()
        {
            m_clientMap.OnTapMap -= OnTabMap;
        }   
        
        private void OnTabMap(ClientCell clientCell)
        {
            if (clientCell.IsOccupiedByUnit)
            {
                OnTapUnit(clientCell);
            }
            else
            {
                bool isHighlightedCell = m_clientMap.IsHighlightedCell(clientCell);
                if (isHighlightedCell)
                {
                    if (!m_turnManager.IsMyTurn) return;
                    if (!CheckIfUnitIsMovable(m_selectedUnitUID)) return;
                    m_unitLogic.CommandMoveUnitServerRpc(m_selectedUnitUID, clientCell.Coord);
                }
                else
                {
                    m_clientMap.DisableHighlightCells();
                }
            }
        }
        
        /// <summary>
        /// Wrapper for checking if the unit is movable, and log if it is not.
        /// </summary>
        private bool CheckIfUnitIsMovable(int unitUID)
        {
            var clientUnit = ClientUnit.UnitsCreatedThisGame[unitUID];
            if (!clientUnit.IsMovable)
            {
                DebugWrapper.Log("Unit is not movable");
                return false;
            }
            
            return true;
        }
        
        private void OnTapUnit(ClientCell clientCell)
        {
            m_selectedUnitUID = clientCell.CurrentUnitUID;
            DebugWrapper.Log($"Unit {m_selectedUnitUID} is tapped");
            var clientUnit = ClientUnit.UnitsCreatedThisGame[m_selectedUnitUID];
            var moveRange = clientUnit.MoveRange;
            m_clientMap.DisableHighlightCells();
            if (moveRange > 0)
            {
                m_clientMap.HighlightReachableCells(clientCell, moveRange);
            }
        }

        public void SpawnUnit(int unitUID, int cardUID, Vector2Int coord)
        {
            m_selectedUnitUID = unitUID;

            var clientUnit = new ClientUnit(unitUID, cardUID, coord);
            var clientCell = m_clientMap[coord];
            
            m_clientMap.PlaceUnitOnMap(unitUID, coord);
           
            // Instantiate unit visual
            var unitVisual = Instantiate(m_unitVisualPrefab, clientCell.transform.position, 
                Quaternion.identity, m_unitContainer);
            m_unitVisuals.Add(unitUID, unitVisual);
            
            if (clientUnit.IsMine)
            {
                m_clientMap.DisableHighlightCells();
                m_clientMap.HighlightReachableCells(clientCell, clientUnit.MoveRange);
            }
            
            OnUnitSpawned?.Invoke(clientCell);
        }
        
        public void MoveUnitOneCell(int unitUID, Vector2Int coord)
        {
            var clientUnit = ClientUnit.UnitsCreatedThisGame[unitUID];
            var clientCell = m_clientMap[clientUnit.Coord];
            clientCell.RemoveUnit();
            
            m_unitVisuals[unitUID].MoveTo(m_clientMap[coord].transform.position, 
                () =>
            {
                clientUnit.Coord = coord;
                clientUnit.MoveRange -= 1;
                m_clientMap.PlaceUnitOnMap(unitUID, coord);
            });
        }
    }
}
