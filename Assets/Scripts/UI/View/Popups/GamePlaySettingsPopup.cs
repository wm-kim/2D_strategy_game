using System;
using Minimax.CoreSystems;
using Minimax.SceneManagement;
using Minimax.ScriptableObjects.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax.UI.View.Popups
{
    public class GamePlaySettingsPopup : PopupView
    {
        [Header("References")]
        [Space(10f)]
        [SerializeField] private Button m_confirmButton;
        [SerializeField] private Button m_cancelButton;

        protected override void SetPopupType() => Type = PopupType.GamePlaySettingsPopup;
        
        private void Start()
        {
            m_confirmButton.onClick.AddListener(OnConfirmButtonClicked);
            m_cancelButton.onClick.AddListener(OnCancelButtonClicked);
        }
        
        private void OnConfirmButtonClicked()
        {
            GlobalManagers.Instance.Popup.HideCurrentPopup();
            GlobalManagers.Instance.Scene.RequestLoadScene(SceneType.MenuScene);
        }
        
        private void OnCancelButtonClicked()
        {
            GlobalManagers.Instance.Popup.HideCurrentPopup();
        }
    }
}
