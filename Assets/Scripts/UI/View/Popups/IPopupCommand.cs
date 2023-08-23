namespace Minimax.UI.View.Popups
{
    /// <summary>
    /// 팝업을 대기열에 넣을 때 어떻게 처리할지에 대한 옵션
    /// </summary>
    public enum PopupCommandType
    {
        /// <summary>
        /// 이미 화면에 띄워져 있는 팝업이거나 대기열에 있는 팝업이면 무시
        /// </summary>
        Unique,         
        
        /// <summary>
        /// 이미 화면에 띄워져 있는 팝업이거나 대기열에 있는 팝업이라도 대기열에 중복하여 넣음
        /// </summary>
        Duplicate,      
        
        /// <summary>
        /// 다른 어떠한 종류의 팝업이라도 화면에 띄워져 있거나 대기열에 있으면 무시
        /// </summary>
        CancelIfExist,  
    }

    public interface IPopupCommand
    {
        public string Key { get; }
        PopupType Type { get; }
        PopupCommandType CommandType { get; }
    }
}