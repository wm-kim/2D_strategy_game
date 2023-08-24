using System;
using Minimax.CoreSystems;

namespace Minimax.UI.View.Popups
{
    /// <summary>
    /// 로딩 팝업을 띄우는 컨텍스트, using 구문을 사용하여 자동으로 팝업을 닫을 수 있음
    /// </summary>
    public class LoadingPopupContext : IDisposable
    {
        /// <param name="key">queue안에서 중복되는지 확인하기 위한 식별자입니다.</param>
        public LoadingPopupContext(string key, string message, PopupCommandType commandType = PopupCommandType.Duplicate, int priority = 0)
        {
            GlobalManagers.Instance.Popup.RegisterLoadingPopupToQueue(key, message, commandType, priority);
        }

        public void Dispose()
        {
            GlobalManagers.Instance.Popup.HideCurrentPopup();
        }
    }
}