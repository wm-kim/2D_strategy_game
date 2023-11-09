using Minimax.CoreSystems;
using Minimax.Definitions;
using Minimax.UI.View.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax.UI.Controller.ComponentControllers
{
    public class MenuOptionUIController : MonoBehaviour
    {
        [SerializeField]
        private Button m_settingsButton;

        [SerializeField]
        private Button m_exitButton;

        private void Awake()
        {
            m_settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            m_exitButton.onClick.AddListener(OnExitButtonClicked);
        }

        private void OnSettingsButtonClicked()
        {
            AudioManager.Instance.PlaySFX(AudioLib.Popup);
            PopupManager.Instance.RegisterPopupToQueue(PopupType.AppSettingPopup, PopupCommandType.Unique);
            gameObject.SetActive(false);
        }

        private void OnExitButtonClicked()
        {
            AudioManager.Instance.PlaySFX(AudioLib.Popup);
            PopupManager.Instance.RegisterPopupToQueue(PopupType.ExitAppPopup, PopupCommandType.Unique);
            gameObject.SetActive(false);
        }
    }
}