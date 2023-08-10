using Minimax.CoreSystems;
using Minimax.UI.View.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax.UI.Controller.PageControllers
{
    public class DeckBuildingPageController : MonoBehaviour
    {
        [SerializeField] private CloudSaveManager m_cloudSaveManager;
        
        [Header("References")]
        [Space(10f)]
        [SerializeField] private Button m_exitAndSaveButton;
        [SerializeField] private Button m_exitWithoutSaveButton;
        
        private void OnEnable()
        {
            m_exitAndSaveButton.onClick.AddListener(OnExitAndSavePressed);
            m_exitWithoutSaveButton.onClick.AddListener(OnExitWithoutSavePressed);
            GlobalManagers.Instance.Input.OnBackButton += OnBackButtonPressed;
        }
        
        private void OnDisable()
        {
            m_exitWithoutSaveButton.onClick.RemoveListener(OnExitWithoutSavePressed);
            GlobalManagers.Instance.Input.OnBackButton -= OnBackButtonPressed;
        }
        
        private void OnExitWithoutSavePressed()
        {
            OnBackButtonPressed();
        }
        
        private void OnBackButtonPressed()
        {
            GlobalManagers.Instance.Popup.MobileBackButtonTwoButtonPopup(
                "Exit without saving?",
                "Yes",
                "No",
                () =>
                {
                    GlobalManagers.Instance.Scene.RequestLoadScene(SceneType.MenuScene);
                    GlobalManagers.Instance.Popup.HideCurrentPopup();
                },
                () =>
                {
                    GlobalManagers.Instance.Popup.HideCurrentPopup();
                });
        }
        
        private void OnExitAndSavePressed()
        {
            m_cloudSaveManager.SaveDeckToCloud();
        }
    }
}
