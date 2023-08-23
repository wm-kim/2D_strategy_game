namespace Minimax.UI.View.Popups
{
    public class DefaultPopupCommand : IPopupCommand
    {
        public string Key { get; }
        public PopupType Type { get; }
        public PopupCommandType CommandType { get; }
        
        public DefaultPopupCommand(PopupType type, PopupCommandType commandType = PopupCommandType.Duplicate)
        {
            Key = Type.ToString();
            Type = type;
            CommandType = commandType;
        }
    }
}