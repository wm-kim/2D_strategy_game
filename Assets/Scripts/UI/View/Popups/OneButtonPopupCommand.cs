using System;

namespace Minimax.UI.View.Popups
{
    public class OneButtonPopupCommand : IPopupCommand
    {
        public PopupType Type { get; }
        public string Message { get; }
        public string ButtonText { get; }
        public Action ButtonAction { get; }
        
        public OneButtonPopupCommand(string message, string buttonText, Action buttonAction)
        {
            Type = PopupType.OneButtonPopup;
            Message = message;
            ButtonText = buttonText;
            ButtonAction = buttonAction;
        }
    }
}
