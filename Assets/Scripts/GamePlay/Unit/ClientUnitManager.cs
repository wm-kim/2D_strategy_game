using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Minimax.GamePlay;
using Minimax.GamePlay.CommandSystem;
using Minimax.GamePlay.GridSystem;
using Minimax.GamePlay.Logic;
using Minimax.GamePlay.Unit;
using Minimax.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minimax
{
    /// <summary>
    /// Responsible for spawning units and visualizing them, cache client unit based on player number
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
        private Dictionary<int, UnitVisual> m_unitVisuals = new ();
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
                    if (!CheckIfIOwnUnit(m_selectedUnitUID)) return;
                    if (!CheckIfUnitIsMovable(m_selectedUnitUID)) return;
                    m_unitLogic.CommandMoveUnitServerRpc(m_selectedUnitUID, clientCell.Coord);
                }
                else
                {
                    m_clientMap.DisableHighlightCells();
                }
            }
        }
        
        private bool CheckIfIOwnUnit(int unitUID)
        {
            var clientUnit = ClientUnit.UnitsCreatedThisGame[unitUID];
            if (!clientUnit.IsMine)
            {
                DebugWrapper.Log("You don't own this unit");
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Wrapper for checking if the unit is movable
        /// </summary>
        private bool CheckIfUnitIsMovable(int unitUID)
        {
            var clientUnit = ClientUnit.UnitsCreatedThisGame[unitUID];
            return clientUnit.CheckIfMovable();
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
            
            // TODO : pass both player rotation and current rotation
            unitVisual.Init(m_clientMap.GetMyPlayerRotation());
            
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
            
            Vector2Int dir = coord - clientUnit.Coord;
            // TODO : Change it to current rotation
            var playerRotation = m_clientMap.GetMyPlayerRotation();
            
            m_unitVisuals[unitUID].AnimateMove(dir, playerRotation, m_clientMap[coord].transform.position).OnComplete(() =>
                {
                    clientUnit.Coord = coord;
                    clientUnit.MoveRange -= 1;
                    m_clientMap.PlaceUnitOnMap(unitUID, coord);
                    Command.ExecutionComplete();
                });
        }
    }
}
