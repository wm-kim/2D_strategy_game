using System;

namespace Minimax.UI.View.Popups
{
    [Serializable]
    public class DefaultPopupCommand : IPopupCommand
    {
        public string           Key         { get; }
        public PopupType        Type        { get; }
        public PopupCommandType CommandType { get; }
        public PopupPriority    Priority    { get; }

        public DefaultPopupCommand(PopupType type, PopupCommandType commandType = PopupCommandType.Duplicate,
            PopupPriority priority = PopupPriority.Low)
        {
            Key         = type.ToString();
            Type        = type;
            CommandType = commandType;
            Priority    = priority;
        }
    }
}