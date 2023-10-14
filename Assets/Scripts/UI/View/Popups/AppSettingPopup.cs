using System;
using System.Collections;
using System.Collections.Generic;
using JSAM;
using Minimax.CoreSystems;
using Minimax.UI.View.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax
{
    public class AppSettingPopup : PopupView
    {
        protected override void SetPopupType() => Type = PopupType.AppSettingPopup;

        [Header("References")]
        [SerializeField] private Button m_exitButton;

        private void Awake()
        {
            m_exitButton.onClick.AddListener(OnExitButtonClicked);
        }
        
        private void OnExitButtonClicked()
        {
            AudioManager.PlaySound(AudioLibrarySounds.GeneralButtonSound);
            PopupManager.Instance.HideCurrentPopup();
        }
    }
}
