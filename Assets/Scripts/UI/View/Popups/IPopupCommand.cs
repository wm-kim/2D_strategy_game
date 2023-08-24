using System;
using Sirenix.OdinInspector;
using UnityEngine;

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
    }

    public interface IPopupCommand
    {
        public string Key { get; }
        PopupType Type { get;}

        PopupCommandType CommandType { get;}

        /// <summary>
        /// 팝업을 표시할 때 우선순위. 숫자가 클수록 우선순위가 높음
        /// </summary>
        public int Priority { get;}
    }
}