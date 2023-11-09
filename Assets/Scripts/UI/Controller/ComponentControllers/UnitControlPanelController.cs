using System;
using Minimax.GamePlay;
using Minimax.GamePlay.GridSystem;
using Minimax.GamePlay.Unit;
using Minimax.UI.Controller.ComponentControllers;
using UnityEngine;

namespace Minimax
{
    public enum UnitCommandType
    {
        Move   = 0,
        Attack = 1
    }

    /// <summary>
    /// Responsible for showing or hiding unit control panel
    /// </summary>
    public class UnitControlPanelController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private GameObject m_unitControlPanel;

        [SerializeField]
        private ButtonGroupController m_controlButtons;

        [Header("Other References")]
        [SerializeField]
        private ClientUnitManager m_clientUnitManager;

        public event Action<UnitCommandType> OnControlButtonClicked;

        private void Awake()
        {
            m_unitControlPanel.SetActive(false);
            m_controlButtons.Init();
        }

        private void OnEnable()
        {
            AddEventListeners();
        }

        private void OnDisable()
        {
            RemoveEventListeners();
        }

        private void AddEventListeners()
        {
            m_controlButtons.OnButtonSelected  += OnButtonSelected;
            m_clientUnitManager.OnUnitSpawned  += OnUnitSpawned;
            m_clientUnitManager.OnUnitSelect   += OnUnitSelect;
            m_clientUnitManager.OnUnitDeselect += OnUnitDeselect;
        }

        private void RemoveEventListeners()
        {
            m_controlButtons.OnButtonClicked   -= OnButtonSelected;
            m_clientUnitManager.OnUnitSpawned  -= OnUnitSpawned;
            m_clientUnitManager.OnUnitSelect   -= OnUnitSelect;
            m_clientUnitManager.OnUnitDeselect -= OnUnitDeselect;
        }

        private void OnButtonSelected(int index)
        {
            OnControlButtonClicked?.Invoke((UnitCommandType)index);
        }

        private void OnUnitSpawned(ClientCell clientCell)
        {
            var unitUID = clientCell.CurrentUnitUID;
            if (!TurnManager.Instance.IsMyTurn || !ClientUnit.IsMyUnit(unitUID)) return;
            ResetAndShowIfMyUnit(unitUID);
        }

        private void SelectUnitControlButton(UnitCommandType unitCommandType)
        {
            m_controlButtons.SelectButton((int)unitCommandType);
        }

        private void OnUnitSelect((int unitUID, bool isSameUnit) unitSelectInfo)
        {
            var (unitUID, isSameUnit) = unitSelectInfo;

            if (TurnManager.Instance.IsMyTurn)
            {
                if (!ClientUnit.IsMyUnit(unitUID))
                {
                    ResetAndHide();
                }
                
                if (!isSameUnit)
                {
                    ResetAndShowIfMyUnit(unitUID);
                }
            }
        }

        private void OnUnitDeselect()
        {
            ResetAndHide();
        }

        public void ResetAndShowIfMyUnit(int unitUID)
        {
            if (!ClientUnit.IsMyUnit(unitUID)) return;
            m_controlButtons.Reset();
            m_unitControlPanel.SetActive(true);
            // select move button by default
            SelectUnitControlButton(UnitCommandType.Move);
        }

        public void ResetAndHide()
        {
            m_controlButtons.Reset();
            m_unitControlPanel.SetActive(false);
        }

        public UnitCommandType? GetSelectedUnitCommandType()
        {
            if (m_controlButtons.ActiveIndices.Count == 0) return null;
            var selectedUnitCommandIndex = m_controlButtons.ActiveIndices[0];
            return (UnitCommandType)selectedUnitCommandIndex;
        }
    }
}