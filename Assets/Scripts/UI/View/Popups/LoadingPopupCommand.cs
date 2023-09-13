namespace Minimax.UI.View.Popups
{
    public class LoadingPopupCommand : IPopupCommand
    {
        public string Key { get; }
        public PopupType Type { get; }
        public PopupCommandType CommandType { get; }
        public PopupPriority Priority { get; }
        public string Message { get; }
        
        public LoadingPopupCommand(string key, string message, PopupCommandType commandType = PopupCommandType.Duplicate, PopupPriority priority = PopupPriority.Low)
        {
            Key = key;
            Type = PopupType.LoadingPopup;
            CommandType = commandType;
            Priority = priority;
            Message = message;
        }
    }
}