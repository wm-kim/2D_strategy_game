using System;

namespace Minimax.UI.View.Popups
{
    public class TwoButtonPopupCommand : IPopupCommand
    {
        public string Key { get; }
        public PopupType Type { get; }
        public PopupCommandType CommandType { get; }
        public string Message { get; }
        public string LeftButtonText { get; }
        public string RightButtonText { get; }
        public Action LeftButtonAction { get; }
        public Action RightButtonAction { get; }
        
        public TwoButtonPopupCommand(string key, string message, string leftButtonText, string rightButtonText, 
            Action leftButtonAction, Action rightButtonAction, PopupCommandType commandType = PopupCommandType.Duplicate)
        {
            Key = key;
            Type = PopupType.TwoButtonPopup;
            CommandType = commandType;
            Message = message;
            LeftButtonText = leftButtonText;
            RightButtonText = rightButtonText;
            LeftButtonAction = leftButtonAction;
            RightButtonAction = rightButtonAction;
        }
    }
}