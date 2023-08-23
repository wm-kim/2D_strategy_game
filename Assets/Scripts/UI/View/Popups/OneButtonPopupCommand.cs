using System;

namespace Minimax.UI.View.Popups
{
    public class OneButtonPopupCommand : IPopupCommand
    {
        public string Key { get; }
        public PopupType Type { get; }
        public PopupCommandType CommandType { get; }
        public string Message { get; }
        public string ButtonText { get; }
        public Action ButtonAction { get; }
        
        public OneButtonPopupCommand(string key, string message, string buttonText, Action buttonAction, 
            PopupCommandType commandType = PopupCommandType.Duplicate)
        {
            Key = key;
            Type = PopupType.OneButtonPopup;
            CommandType = commandType;
            Message = message;
            ButtonText = buttonText;
            ButtonAction = buttonAction;
            
        }
    }
}
