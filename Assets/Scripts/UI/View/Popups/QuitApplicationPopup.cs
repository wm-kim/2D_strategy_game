using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WMK
{
    public class QuitApplicationPopup : PopupView
    {
        protected override void SetPopupType() => Type = PopupType.QuitApp;
    }
}
