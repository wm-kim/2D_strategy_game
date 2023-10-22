using System;
using Minimax.CoreSystems;
using Minimax.SceneManagement;
using Minimax.ScriptableObjects.Events;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Minimax.UI.View.Popups
{
    public class SurrenderPopup : PopupView
    {
        
        [Header("References")]
        [Space(10f)]
        [SerializeField] private Button m_yesButton;
        [SerializeField] private Button m_noButton;
        [SerializeField] private Button m_exitButton;

        protected override void SetPopupType() => Type = PopupType.SurrenderPopup;
        
        private void Start()
        {
            m_yesButton.onClick.AddListener(OnConfirmButtonClicked);
            m_noButton.onClick.AddListener(OnCancelButtonClicked);
            m_exitButton.onClick.AddListener(OnCancelButtonClicked);
        }
        
        private void OnConfirmButtonClicked()
        {
            PopupManager.Instance.HideCurrentPopup();
            GlobalManagers.Instance.Connection.RequestShutdown();
        }
        
        private void OnCancelButtonClicked()
        {
            PopupManager.Instance.HideCurrentPopup();
        }
    }
}
