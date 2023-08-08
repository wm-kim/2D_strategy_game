using System;
using Minimax.CoreSystems;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax.UI.View.Popups
{
    public class QuitApplicationPopup : PopupView
    {
        [SerializeField] private Button m_cancelButton;
        
        protected override void SetPopupType() => Type = PopupType.QuitAppPopup;
        
        private void Awake()
        {
            m_cancelButton.onClick.AddListener(OnCancelButtonClicked);
        }

        private void OnCancelButtonClicked()
        {
            GlobalManagers.Instance.Popup.HideCurrentPopup();
        }
    }
}
