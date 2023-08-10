using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax.UI.View.Popups
{
    public class TwoButtonPopup : PopupView
    {
        [Header("References")]
        [SerializeField] TextMeshProUGUI m_messageText;
        [SerializeField] Button m_leftButton;
        [SerializeField] Button m_rightButton;

        protected override void SetPopupType() => Type = PopupType.TwoButtonPopup;
        
        public void ConfigureWithCommand(TwoButtonPopupCommand command) => 
            SetUp(command.Message, command.LeftButtonText, command.RightButtonText, command.LeftButtonAction, command.RightButtonAction);
        
        private void SetUp(string message, string leftButtonText, string rightButtonText, Action leftButtonCallback, Action rightButtonCallback)
        {
            m_messageText.text = message;
            m_leftButton.GetComponentInChildren<TextMeshProUGUI>().text = leftButtonText;
            m_rightButton.GetComponentInChildren<TextMeshProUGUI>().text = rightButtonText;
            m_leftButton.onClick.AddListener(() =>
            {
                leftButtonCallback?.Invoke();
            });
            m_rightButton.onClick.AddListener(() =>
            {
                rightButtonCallback?.Invoke();
            });
        }
    }
}
