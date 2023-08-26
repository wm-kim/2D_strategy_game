using Cysharp.Threading.Tasks;
using Minimax.CoreSystems;
using Minimax.UI.View.Popups;
using Minimax.Utilities;
using NovaSamples.UIControls;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minimax
{
    public class GamePlayMenuController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button m_menuButton;
        
        private void Start()
        {
            m_menuButton.OnClicked.AddListener(RequestOpenMenuPopup);
        }

        private void OnEnable()
        {
            GlobalManagers.Instance.Input.OnBackButton += OnBackButtonPressed;
        }
        
        private void OnDisable()
        {
            GlobalManagers.Instance.Input.OnBackButton -= OnBackButtonPressed;
        }

        private void RequestOpenMenuPopup()
        {
            GlobalManagers.Instance.Popup.RegisterPopupToQueue(PopupType.GameMenuPopup, PopupCommandType.Unique);
        }
        
        private void OnBackButtonPressed()
        {
            GlobalManagers.Instance.Popup.MobileBackButtonPopup(PopupType.GameMenuPopup, PopupCommandType.Unique);
        }
    }
}
