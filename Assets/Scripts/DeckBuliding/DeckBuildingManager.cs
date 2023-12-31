using System.Collections.Generic;
using Minimax.CoreSystems;
using Minimax.Definitions;
using Minimax.SceneManagement;
using Minimax.ScriptableObjects;
using Minimax.UI.View.ComponentViews.DeckBuilding;
using Minimax.UI.View.Popups;
using Unity.Services.CloudCode;
using UnityEngine;
using Utilities;
using Debug = Utilities.Debug;

namespace Minimax.DeckBuliding
{
    /// <summary>
    /// Stores references to all the views in the deck building scene.
    /// </summary>
    public class DeckBuildingManager : MonoBehaviour
    {
        [Header("Model References")]
        [SerializeField]
        private DeckListSO m_deckListSO;

        public DeckListSO DeckListSO => m_deckListSO;

        [Header("View References")]
        [SerializeField]
        private DeckListPanelView m_deckListPanelView;

        [SerializeField]
        private DeckListView m_deckListView;

        [SerializeField]
        private DBCardScrollView m_dbCardScrollView;

        [SerializeField]
        private DBCardItemMenuView m_dbCardItemMenuView;

        [SerializeField]
        private DeckListItemMenuView m_deckListItemMenuView;

        public DeckListPanelView    DeckListPanelView    => m_deckListPanelView;
        public DeckListView         DeckListView         => m_deckListView;
        public DBCardScrollView     DBCardScrollView     => m_dbCardScrollView;
        public DBCardItemMenuView   DBCardItemMenuView   => m_dbCardItemMenuView;
        public DeckListItemMenuView DeckListItemMenuView => m_deckListItemMenuView;

        public DBCardItemView   SelectedDBCardItemView   { get; set; }
        public DeckListItemView SelectedDeckListItemView { get; set; }

        private const int k_requiredDeckSize = 5;

        public async void SaveDeckToCloud()
        {
            var deckDTO = m_deckListSO.GetDeckDTO();

            // Validate the deck in the client side before sending it to the cloud
            if (!ClientSideDeckValidation(deckDTO))
            {
                PopupManager.Instance.RegisterOneButtonPopupToQueue(
                    Define.InvalidDeckPopup,
                    $"Deck must contain {k_requiredDeckSize} cards.", "OK",
                    () => PopupManager.Instance.HideCurrentPopup());
                return;
            }

            // Serialize the deck to JSON
            var deckJson = Newtonsoft.Json.JsonConvert.SerializeObject(deckDTO);

            try
            {
                // Show a loading popup while we wait for the cloud code to finish
                using (new LoadingPopupContext(Define.SavingDeckPopup, "Saving Deck to Cloud..."))
                {
                    // Call the function within the module and provide the parameters we defined in there
                    await CloudCodeService.Instance.CallModuleEndpointAsync("Deck", "SaveDeckData",
                        new Dictionary<string, object> { { "value", deckJson } });
                }

                // Save deck name into player prefs
                PlayerPrefs.SetString(Define.CurrentDeckNameCache, deckDTO.Name);
                GlobalManagers.Instance.Cache.SetNeedUpdate(Define.DeckDtoCollectionCache);
                GlobalManagers.Instance.Scene.LoadScene(SceneType.MenuScene);
            }
            catch (CloudCodeException exception)
            {
                Debug.LogError(exception.Message);
            }
        }

        private bool ClientSideDeckValidation(DeckDTO deckDto)
        {
            return deckDto.CardIds.Count == k_requiredDeckSize;
        }
    }
}