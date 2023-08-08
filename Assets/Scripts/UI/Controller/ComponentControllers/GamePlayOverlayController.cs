using Cysharp.Threading.Tasks;
using Minimax.CoreSystems;
using Minimax.UI.View.Popups;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Minimax
{
    public class GamePlayOverlayController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button m_gamePlaySettingsButton;
        
        private void Start()
        {
            m_gamePlaySettingsButton.onClick.AddListener(RequestOpenGamePlaySettingsPopup);
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
            GlobalManagers.Instance.Popup.RequestShowPopup(PopupType.GamePlaySettings);
        }
        
        private void OnBackButtonPressed()
        {
            GlobalManagers.Instance.Popup.RequestMobileBackButton(PopupType.GamePlaySettings);
        }
    }
}
