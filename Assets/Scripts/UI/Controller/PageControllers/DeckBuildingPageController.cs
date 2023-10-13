using System;
using Minimax.CoreSystems;
using Minimax.SceneManagement;
using Minimax.UI.View.Popups;
using Minimax.Utilities;
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
            m_deckNameInputField.text = "New Deck Name";
            m_exitAndSaveButton.onClick.AddListener(OnExitAndSavePressed);
            m_exitWithoutSaveButton.onClick.AddListener(OnExitWithoutSavePressed);
            m_deckNameInputField.onValueChanged.AddListener(OnDeckNameChanged);
        }

        private void OnEnable()
        {
            GlobalManagers.Instance.Input.OnBackButton += OnBackButtonPressed;
        }
        
        private void OnDisable()
        {
            GlobalManagers.Instance.Input.OnBackButton -= OnBackButtonPressed;
        }
        
        private void OnExitWithoutSavePressed()
        {
            OnBackButtonPressed();
        }
        
        private void OnBackButtonPressed()
        {
            PopupManager.Instance.MobileBackTwoButtonPopup(
                Define.ExitDeckBuilderPopup,
                "Exit without saving?",
                "Yes",
                "No",
                () =>
                {
                    GlobalManagers.Instance.Scene.LoadScene(SceneType.MenuScene);
                    PopupManager.Instance.HideCurrentPopup();
                },
                () =>
                {
                    PopupManager.Instance.HideCurrentPopup();
                },
                PopupCommandType.Unique);
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
