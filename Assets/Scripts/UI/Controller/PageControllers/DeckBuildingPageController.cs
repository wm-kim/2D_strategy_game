using Minimax.CoreSystems;
using Minimax.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Minimax.UI.Controller.PageControllers
{
    public class DeckBuildingPageController : MonoBehaviour
    {
        [SerializeField] private DeckBuildingManager m_deckBuildingManager;
        
        [Header("References")]
        [Space(10f)]
        [SerializeField] private Button m_exitAndSaveButton;
        [SerializeField] private Button m_exitWithoutSaveButton;
        [SerializeField] private TMP_InputField m_deckNameInputField;
        
        private void Start()
        {
            m_exitAndSaveButton.onClick.AddListener(OnExitAndSavePressed);
            m_exitWithoutSaveButton.onClick.AddListener(OnExitWithoutSavePressed);
            GlobalManagers.Instance.Input.OnBackButton += OnBackButtonPressed;
            m_deckNameInputField.onValueChanged.AddListener(OnDeckNameChanged);
            
            m_deckNameInputField.text = "New Deck Name";
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
            m_deckBuildingManager.SaveDeckToCloud();
        }
        
        private void OnDeckNameChanged(string deckName)
        {
            m_deckBuildingManager.DeckListSO.SetDeckName(deckName);
            m_deckBuildingManager.DeckListPanelView.SetDeckName(deckName);
        }
    }
}
