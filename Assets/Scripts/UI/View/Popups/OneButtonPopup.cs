using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax.UI.View.Popups
{
    public class OneButtonPopup : PopupView
    {
        [Header("References")] [SerializeField]
        private TextMeshProUGUI m_messageText;

        [SerializeField] private Button m_Button;

        protected override void SetPopupType()
        {
            Type = PopupType.OneButtonPopup;
        }

        public void ConfigureWithCommand(OneButtonPopupCommand command)
        {
            SetUp(command.Message, command.ButtonText, command.ButtonAction);
        }

        private void SetUp(string message, string buttonText, Action buttonAction)
        {
            m_messageText.text                                      = message;
            m_Button.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
            m_Button.onClick.AddListener(() => { buttonAction?.Invoke(); });
        }
    }
}