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
    public class MenuOptionUIController : MonoBehaviour
    {
        [SerializeField] private Button m_settingsButton;
        [SerializeField] private Button m_exitButton;
        
        private void Awake()
        {
            m_settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            m_exitButton.onClick.AddListener(OnExitButtonClicked);
        }

        private void OnSettingsButtonClicked()
        {
            AudioManager.PlaySound(AudioLibrarySounds.PopupSound);
            PopupManager.Instance.RegisterPopupToQueue(PopupType.AppSettingPopup, PopupCommandType.Unique);
            gameObject.SetActive(false);
        }
        
        private void OnExitButtonClicked()
        {
            AudioManager.PlaySound(AudioLibrarySounds.PopupSound);
            PopupManager.Instance.RegisterPopupToQueue(PopupType.ExitAppPopup, PopupCommandType.Unique);
            gameObject.SetActive(false);
        } 
    }
}
