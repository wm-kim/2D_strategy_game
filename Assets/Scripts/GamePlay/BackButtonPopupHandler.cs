using Minimax.CoreSystems;
using Minimax.UI.View.Popups;
using UnityEngine;

namespace Minimax
{
    public class BackButtonPopupHandler : MonoBehaviour
    {
        private void OnEnable()
        {
            GlobalManagers.Instance.Input.OnBackButton += OnMobileBackButton;
        }

        private void OnDisable()
        {
            GlobalManagers.Instance.Input.OnBackButton -= OnMobileBackButton;
        }

        private void OnMobileBackButton()
        {
            AudioManager.Instance.PlaySFX(AudioLib.Popup);
            PopupManager.Instance.MobileBackButtonPopup(PopupType.SurrenderPopup, PopupCommandType.Unique);
        }
    }
}