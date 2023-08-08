namespace Minimax.UI.View.Popups
{
    public class DefaultPopupCommand : IPopupCommand
    {
        public PopupType Type { get; }
        public DefaultPopupCommand(PopupType type) => Type = type;
    }
}