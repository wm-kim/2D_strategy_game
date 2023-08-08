using System;
using Minimax.CoreSystems;
using Minimax.ScriptableObjects.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax.UI.View.Popups
{
    public class GamePlaySettingsPopup : PopupView
    {
        protected override void SetPopupType() => Type = PopupType.GamePlaySettings;
        
        [Header("References")]
        [Space(10f)]
        [SerializeField] private Button m_confirmButton;
        [SerializeField] private Button m_cancelButton;

        private void Start()
        {
            m_confirmButton.onClick.AddListener(OnConfirmButtonClicked);
            m_cancelButton.onClick.AddListener(OnCancelButtonClicked);
        }
        
        private void OnConfirmButtonClicked()
        {
            GlobalManagers.Instance.Popup.RequestHidePopup();
            GlobalManagers.Instance.Scene.RequestLoadScene(SceneType.MenuScene);
        }
        
        private void OnCancelButtonClicked()
        {
            GlobalManagers.Instance.Popup.RequestHidePopup();
        }
    }
}
