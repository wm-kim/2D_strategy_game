using System;

namespace Minimax.UI.View.Popups
{
    [Serializable]
    public class DefaultPopupCommand : IPopupCommand
    {
        public string Key { get; }
        public PopupType Type { get; }
        public PopupCommandType CommandType { get; }
        public int Priority { get; }
        
        public DefaultPopupCommand(PopupType type, PopupCommandType commandType = PopupCommandType.Duplicate, int priority = 0)
        {
            Key = type.ToString();
            Type = type;
            CommandType = commandType;
            Priority = priority;
        }
    }
}