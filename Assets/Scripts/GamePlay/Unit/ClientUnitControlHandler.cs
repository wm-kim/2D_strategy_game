using System;
using Minimax.GamePlay.GridSystem;
using Minimax.GamePlay.Logic;
using UnityEngine;

namespace Minimax.GamePlay.Unit
{
    public class ClientUnitControlHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private ClientUnitManager m_clientUnitManager;

        [SerializeField]
        private UnitControlPanelController m_unitControlPanel;

        [SerializeField]
        private ClientMap m_clientMap;

        [Header("Logic References")]
        [SerializeField]
        private UnitLogic m_unitLogic;

        private void OnEnable()
        {
            m_unitControlPanel.OnControlButtonClicked += OnControlButtonClicked;
            m_clientUnitManager.OnUnitCommand         += OnUnitCommand;
            m_clientUnitManager.OnUnitSelect          += OnUnitSelect;
            m_clientUnitManager.OnUnitDeselect        += OnUnitDeselect;
        }

        private void OnDisable()
        {
            m_unitControlPanel.OnControlButtonClicked -= OnControlButtonClicked;
            m_clientUnitManager.OnUnitCommand         -= OnUnitCommand;
            m_clientUnitManager.OnUnitSelect          -= OnUnitSelect;
            m_clientUnitManager.OnUnitDeselect        -= OnUnitDeselect;
        }

        private void OnControlButtonClicked(UnitCommandType commandType)
        {
            var selectedUnit = m_clientUnitManager.CurrentUnitUID;
            var clientUnit   = ClientUnit.UnitsCreatedThisGame[selectedUnit];
            var unitCell     = m_clientMap[clientUnit.Coord];
            
            m_clientMap.DisableHighlightCells();
            
            switch (commandType)
            {
                case UnitCommandType.Move:
                    m_clientMap.HighlightMovableCells(unitCell, clientUnit.MoveRange);
                    break;
                case UnitCommandType.Attack:
                    m_clientMap.HighlightAttackableCells(unitCell, clientUnit.AttackRange);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(commandType), commandType, null);
            }
        }
        
        private void OnUnitSelect((int unitUID, bool isSameUnit) unitSelectInfo)
        {
            var (unitUID, isSameUnit) = unitSelectInfo;

            if (TurnManager.Instance.IsMyTurn)
            {
                if (!ClientUnit.IsMyUnit(unitUID))
                {
                    m_clientMap.DisableHighlightCells();
                }
            }
        }
        
        private void OnUnitDeselect()
        {
            m_clientMap.DisableHighlightCells();
        }
        
        private void OnUnitCommand(Vector2Int coord)
        {
            if (!TurnManager.Instance.IsMyTurn) return;
            
            var selectedUnitUID = m_clientUnitManager.CurrentUnitUID;

            UnitCommandType? commandType = m_unitControlPanel.GetSelectedUnitCommandType();
            if (commandType == null) return;
            
            switch(commandType)
            {
                case UnitCommandType.Move:
                    if (!ClientUnit.CheckIfMovable(selectedUnitUID)) return;
                    m_unitLogic.CommandMoveUnitServerRpc(selectedUnitUID, coord);
                    break;
                case UnitCommandType.Attack:
                    var targetCell = m_clientMap[coord];
                    if (!targetCell.IsOccupiedByUnit) return;
                    m_unitLogic.CommandAttackUnitServerRpc(selectedUnitUID, targetCell.CurrentUnitUID);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(commandType), commandType, null);
            }
        }
    }
}