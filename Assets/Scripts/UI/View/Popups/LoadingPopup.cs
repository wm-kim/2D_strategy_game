using UnityEngine;
using TMPro;

namespace Minimax.UI.View.Popups
{
    public class LoadingPopup : PopupView
    {
        [Header("References")]
        [SerializeField]
        private TextMeshProUGUI m_messageText;

        protected override void SetPopupType()
        {
            Type = PopupType.LoadingPopup;
        }

        public void ConfigureWithCommand(LoadingPopupCommand command)
        {
            SetUp(command.Message);
        }

        private void SetUp(string message)
        {
            m_messageText.text = message;
        }
    }
}