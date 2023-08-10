namespace Minimax.UI.View.Popups
{
    public class LoadingPopupCommand : IPopupCommand
    {
        public PopupType Type { get; }
        public string Message { get; }
        
        public LoadingPopupCommand(string message)
        {
            Type = PopupType.LoadingPopup;
            Message = message;
        }
    }
}