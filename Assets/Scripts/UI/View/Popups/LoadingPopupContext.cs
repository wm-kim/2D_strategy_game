using System;
using Minimax.CoreSystems;

namespace Minimax.UI.View.Popups
{
    /// <summary>
    /// 로딩 팝업을 띄우는 컨텍스트, using 구문을 사용하여 자동으로 팝업을 닫을 수 있음
    /// </summary>
    public class LoadingPopupContext : IDisposable
    {
        public LoadingPopupContext(string message)
        {
            GlobalManagers.Instance.Popup.RegisterLoadingPopupToQueue(message);
        }

        public void Dispose()
        {
            GlobalManagers.Instance.Popup.HideCurrentPopup();
        }
    }
}