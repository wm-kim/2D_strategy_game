using Minimax.CoreSystems;
using Minimax.UI.View.Popups;
using Minimax.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax.UI.Controller.ComponentControllers
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
            AudioManager.Instance.PlaySFX(AudioLib.Popup);
            gameObject.SetActive(false);
        }

        private void OnSurrenderButtonClicked()
        {
            AudioManager.Instance.PlaySFX(AudioLib.Popup);
            PopupManager.Instance.RegisterPopupToQueue(PopupType.SurrenderPopup, PopupCommandType.Unique);
            gameObject.SetActive(false);
        }
    }
}