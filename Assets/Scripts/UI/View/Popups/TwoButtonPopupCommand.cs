using System;

namespace Minimax.UI.View.Popups
{
    public class TwoButtonPopupCommand : IPopupCommand
    {
        public PopupType Type { get; }
        public string Message { get; }
        public string LeftButtonText { get; }
        public string RightButtonText { get; }
        public Action LeftButtonAction { get; }
        public Action RightButtonAction { get; }
        
        public TwoButtonPopupCommand(string message, string leftButtonText, string rightButtonText, Action leftButtonAction, Action rightButtonAction)
        {
            Type = PopupType.TwoButtonPopup;
            Message = message;
            LeftButtonText = leftButtonText;
            RightButtonText = rightButtonText;
            LeftButtonAction = leftButtonAction;
            RightButtonAction = rightButtonAction;
        }
    }
}