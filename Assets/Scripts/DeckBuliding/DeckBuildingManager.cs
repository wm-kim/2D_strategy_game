using System.Collections;
using System.Collections.Generic;
using Minimax.CoreSystems;
using Minimax.ScriptableObjects;
using Minimax.UI.View.ComponentViews;
using Minimax.Utilities;
using Unity.Services.CloudCode;
using UnityEngine;

namespace Minimax
{
    /// <summary>
    /// Stores references to all the views in the deck building scene.
    /// </summary>
    public class DeckBuildingManager : MonoBehaviour
    {
        [Header("Model References")]
        [SerializeField] private DeckDataSO m_deckDataSO;
        public DeckDataSO DeckDataSO => m_deckDataSO;
        
        [Header("View References")]
        [SerializeField] private DeckListView m_deckListView;
        [SerializeField] private DBCardScrollView m_dbCardScrollView;
        [SerializeField] private DBCardItemMenuView m_dbCardItemMenuView;
        [SerializeField] private DeckListItemMenuView m_deckListItemMenuView;
        
        public DeckListView DeckListView => m_deckListView;
        public DBCardScrollView DBCardScrollView => m_dbCardScrollView;
        public DBCardItemMenuView DBCardItemMenuView => m_dbCardItemMenuView;
        public DeckListItemMenuView DeckListItemMenuView => m_deckListItemMenuView;
        
        public DBCardItemView SelectedDBCardItemView { get; set; }
        public DeckListItemView SelectedDeckListItemView { get; set; }
        
        private const int k_requiredDeckSize = 5;
        
        public async void SaveDeckToCloud()
        {
            var deckDTO = m_deckDataSO.GetDeckDTO();
            
            // Validate the deck in the client side before sending it to the cloud
            if (!ClientSideDeckValidation(deckDTO))
            {
                GlobalManagers.Instance.Popup.RegisterOneButtonPopupToQueue($"Deck must contain {k_requiredDeckSize} cards.", "OK",
                    () => GlobalManagers.Instance.Popup.HideCurrentPopup());
            }

            // Serialize the deck to JSON
            var deckJson = Newtonsoft.Json.JsonConvert.SerializeObject(deckDTO);
            DebugWrapper.Log(deckJson);
            
            try
            {
                GlobalManagers.Instance.Popup.RegisterLoadingPopupToQueue("Saving Deck to Cloud");
                
                // Call the function within the module and provide the parameters we defined in there
                string result = await CloudCodeService.Instance.CallModuleEndpointAsync("Deck", "SaveDeckData",
                    new Dictionary<string, object> { { "key", Define.DeckSaveKey }, { "value", deckJson } });
                DebugWrapper.Log(result);
                
                GlobalManagers.Instance.Popup.HideCurrentPopup();
                GlobalManagers.Instance.Scene.RequestLoadScene(SceneType.MenuScene);
            }
            catch (CloudCodeException exception)
            {
                DebugWrapper.LogError(exception.Message);
            }
        }
        
        private bool ClientSideDeckValidation(DeckDTO deckDto) => deckDto.CardIds.Count == k_requiredDeckSize;
    }
}
