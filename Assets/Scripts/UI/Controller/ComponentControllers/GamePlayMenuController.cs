using Cysharp.Threading.Tasks;
using Minimax.CoreSystems;
using Minimax.UI.View.Popups;
using Minimax.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Minimax
{
    public class GamePlayMenuController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button m_surrenderButton;
        
        private void Start()
        {
            m_surrenderButton.onClick.AddListener(RequestOpenGamePlaySettingsPopup);
        }

        private void OnEnable()
        {
            GlobalManagers.Instance.Input.OnBackButton += OnBackButtonPressed;
        }
        
        private void OnDisable()
        {
            GlobalManagers.Instance.Input.OnBackButton -= OnBackButtonPressed;
        }

        private void RequestOpenGamePlaySettingsPopup()
        {
            GlobalManagers.Instance.Popup.RegisterPopupToQueue(PopupType.SurrenderPopup, PopupCommandType.Unique);
        }
        
        private void OnBackButtonPressed()
        {
            GlobalManagers.Instance.Popup.MobileBackButtonPopup(PopupType.SurrenderPopup, PopupCommandType.Unique);
        }
    }
}
