namespace Minimax.UI.View.Popups
{
    public class LoadingPopupCommand : IPopupCommand
    {
        public string Key { get; }
        public PopupType Type { get; }
        public PopupCommandType CommandType { get; }
        public string Message { get; }
        
        public LoadingPopupCommand(string key, string message, PopupCommandType commandType = PopupCommandType.Duplicate)
        {
            key = key;
            Type = PopupType.LoadingPopup;
            CommandType = commandType;
            Message = message;
        }
    }
}