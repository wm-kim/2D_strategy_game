using System;
using JSAM;
using Minimax.CoreSystems;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax.UI.View.Popups
{
    public class ExitApplicationPopup : PopupView
    {
        [SerializeField] private Button m_cancelButton;
        [SerializeField] private Button m_noButton;
        [SerializeField] private Button m_yesButton;
        
        protected override void SetPopupType() => Type = PopupType.ExitAppPopup;
        
        private void Awake()
        {
            m_cancelButton.onClick.AddListener(OnCancelButtonClicked);
            m_noButton.onClick.AddListener(OnNoButtonClicked);
            m_yesButton.onClick.AddListener(OnYesButtonClicked);
        }

        private void OnCancelButtonClicked()
        {
            AudioManager.PlaySound(AudioLibrarySounds.GeneralButtonSound);
            PopupManager.Instance.HideCurrentPopup();
        }
        
        private void OnNoButtonClicked()
        {
            AudioManager.PlaySound(AudioLibrarySounds.GeneralButtonSound);
            PopupManager.Instance.HideCurrentPopup();
        }
        
        private void OnYesButtonClicked()
        {
            AudioManager.PlaySound(AudioLibrarySounds.GeneralButtonSound);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
