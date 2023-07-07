using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WMK
{
    public class MainPageView : PageView
    {
        [SerializeField] private DataString m_gameTitle;
        [SerializeField] private DataString m_gameVersion;
        
        private void Start()
        {
            m_gameTitle.Value = GameSettings.instance.GameTitle;
            m_gameVersion.Value = GameSettings.instance.GameVersion;
        }
    }
}
