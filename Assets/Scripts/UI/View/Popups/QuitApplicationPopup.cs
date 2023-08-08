using System;
using Minimax.CoreSystems;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax.UI.View.Popups
{
    public class QuitApplicationPopup : PopupView
    {
        protected override void SetPopupType() => Type = PopupType.QuitApp;
        
        [SerializeField] private Button m_cancelButton;
        
        private void Awake()
        {
            m_cancelButton.onClick.AddListener(OnCancelButtonClicked);
        }
        
        private void OnCancelButtonClicked()
        {
            GlobalManagers.Instance.Popup.RequestHidePopup();
        }
    }
}
