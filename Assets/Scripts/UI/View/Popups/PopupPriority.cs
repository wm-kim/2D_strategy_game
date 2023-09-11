namespace Minimax.UI.View.Popups
{
    /// <summary>
    /// The priority of the popup. This is used to determine which popup should be displayed first.
    /// </summary>
    public enum PopupPriority
    {
        Low = 0,        // 낮음
        Normal = 1,     // 보통
        High = 2,       // 높음
        Critical = 3    // 중요
    }
}