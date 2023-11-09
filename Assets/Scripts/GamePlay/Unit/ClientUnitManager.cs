using System;
using System.Collections.Generic;
using DG.Tweening;
using Minimax.GamePlay.CommandSystem;
using Minimax.GamePlay.GridSystem;
using Minimax.GamePlay.Logic;
using Minimax.UI.Controller.ComponentControllers;
using UnityEngine;
using Debug = Utilities.Debug;

namespace Minimax.GamePlay.Unit
{
    /// <summary>
    /// Responsible for spawning units and visualizing them, cache client unit based on player number
    /// </summary>
    public class ClientUnitManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private UnitVisual m_unitVisualPrefab;

        [SerializeField]
        private ClientMap m_clientMap;

        [SerializeField]
        private Transform m_unitContainer;

        public event Action<ClientCell>                     OnUnitSpawned;
        public event Action<(int unitUID, bool isSameUnit)> OnUnitSelect;
        public event Action<Vector2Int>                     OnUnitCommand;
        public event Action                                 OnUnitDeselect;

        /// <summary>
        /// Key is unit UID
        /// </summary>
        private Dictionary<int, UnitVisual> m_unitVisuals = new();

        private int m_currentUnitUID = -1;
        public  int CurrentUnitUID => m_currentUnitUID;

        public void SelectUnit(int unitUID) => m_currentUnitUID = unitUID;
        public void DeselectUnit() => m_currentUnitUID = -1;
        public bool IsUnitSelected() => m_currentUnitUID != -1;

        private void OnEnable()
        {
            m_clientMap.OnTapMap += OnTabMap;
        }

        private void OnDisable()
        {
            m_clientMap.OnTapMap -= OnTabMap;
        }

        public void SpawnUnit(int unitUID, int cardUID, Vector2Int coord)
        {
            var clientUnit = new ClientUnit(unitUID, cardUID, coord);
            var clientCell = m_clientMap[coord];
            
            if (clientUnit.IsMine) SelectUnit(unitUID);
            m_clientMap.PlaceUnitOnMap(unitUID, coord);

            // Instantiate unit visual
            var unitVisual = Instantiate(m_unitVisualPrefab, clientCell.transform.position,
                Quaternion.identity, m_unitContainer);
            m_unitVisuals.Add(unitUID, unitVisual);
            unitVisual.Init(CalculateRelativeRotation(clientUnit.Owner), clientUnit.Owner);

            OnUnitSpawned?.Invoke(clientCell);
        }

        // TODO : change GetMyPlayerRotation to GetMyCurrentRotation
        private GridRotation CalculateRelativeRotation(int unitOwner)
        {
            var myRotation   = m_clientMap.GetMyPlayerRotation();
            var unitRotation = m_clientMap.GetPlayerRotation(unitOwner);
            return myRotation.GetRelativeRotation(unitRotation);
        }

        private void OnTabMap(ClientCell clientCell)
        {
            var isHighlightedCell = m_clientMap.IsHighlightedCell(clientCell);
            var isOccupiedCell    = clientCell.IsOccupiedByUnit;

            if (isHighlightedCell)
            {
                OnUnitCommand?.Invoke(clientCell.Coord);
                return;
            }

            if (isOccupiedCell)
            {
                Debug.Log($"Unit {clientCell.CurrentUnitUID} is tapped");
                var isSameUnit = clientCell.CurrentUnitUID == m_currentUnitUID;
                SelectUnit(clientCell.CurrentUnitUID);
                OnUnitSelect?.Invoke((m_currentUnitUID, isSameUnit));
            }
            else
            {
                if (m_currentUnitUID != -1)
                {
                    DeselectUnit();
                    OnUnitDeselect?.Invoke();
                }
            }
        }

        public void MoveUnitOneCell(int unitUID, Vector2Int coord)
        {
            var clientUnit = ClientUnit.UnitsCreatedThisGame[unitUID];
            var clientCell = m_clientMap[clientUnit.Coord];
            clientCell.RemoveUnit();

            var dir = coord - clientUnit.Coord;

            // TODO : Change it to current rotation
            var playerRotation = m_clientMap.GetMyPlayerRotation();

            m_unitVisuals[unitUID].AnimateMove(dir, playerRotation, m_clientMap[coord].transform.position).OnComplete(
                () =>
                {
                    clientUnit.Coord     =  coord;
                    clientUnit.MoveRange -= 1;
                    m_clientMap.PlaceUnitOnMap(unitUID, coord);
                    Command.ExecutionComplete();
                });
        }

        public void AttackUnit(int attackerUID, int targetUnitUID)
        {
            var attackerUnit = ClientUnit.UnitsCreatedThisGame[attackerUID];
            var targetUnit   = ClientUnit.UnitsCreatedThisGame[targetUnitUID];
            targetUnit.Health -= attackerUnit.Attack;
            m_unitVisuals[targetUnitUID].SetHealthBarFill(targetUnit.HealthPercentage);
        }

        public void ResetAllUnitsOnTurnStart()
        {
            foreach (var clientUnit in ClientUnit.UnitsCreatedThisGame.Values) clientUnit.ResetOnTurnStart();
        }
    }
}