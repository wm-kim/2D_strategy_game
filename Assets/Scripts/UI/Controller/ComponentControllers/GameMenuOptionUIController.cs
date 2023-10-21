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
    public class GameMenuOptionUIController : MonoBehaviour
    {
        [SerializeField] private Button m_settingsButton;
        [SerializeField] private Button m_surrenderButton;

        private void Awake()
        {
            m_settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            m_surrenderButton.onClick.AddListener(OnSurrenderButtonClicked);
        }
        
        private void OnSettingsButtonClicked()
        {
            AudioManager.PlaySound(AudioLibrarySounds.PopupSound);
            gameObject.SetActive(false);
        }
        
        private void OnSurrenderButtonClicked()
        {
            AudioManager.PlaySound(AudioLibrarySounds.PopupSound);
            PopupManager.Instance.RegisterPopupToQueue(PopupType.SurrenderPopup, PopupCommandType.Unique);
            gameObject.SetActive(false);
        }
    }
}
