namespace Minimax.UI.View.Popups
{
    public class LoadingPopupCommand : IPopupCommand
    {
        public string Key { get; }
        public PopupType Type { get; }
        public PopupCommandType CommandType { get; }
        public int Priority { get; }
        public string Message { get; }
        
        public LoadingPopupCommand(string key, string message, PopupCommandType commandType = PopupCommandType.Duplicate, int priority = 0)
        {
            key = key;
            Type = PopupType.LoadingPopup;
            CommandType = commandType;
            Priority = priority;
            Message = message;
        }
    }
}