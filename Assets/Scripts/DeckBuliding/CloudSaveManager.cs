using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Minimax.CoreSystems;
using Minimax.ScriptableObjects;
using Minimax.Utilities;
using Unity.Services.CloudCode;
using UnityEngine;

namespace Minimax
{
    public class CloudSaveManager : MonoBehaviour
    {
        public static readonly string DeckSaveKey = "decks";
        
        [SerializeField] private DeckDataSO m_deckDataSO;

        private const int k_requiredDeckSize = 5;

        public async void SaveDeckToCloud()
        {
            var deckDTO = m_deckDataSO.GetDeckDTO();
            
            // Validate the deck in the client side before sending it to the cloud
            if (!ClientSideDeckValidation(deckDTO))
            {
                GlobalManagers.Instance.Popup.RegisterOneButtonPopupToQueue($"Deck must contain {k_requiredDeckSize} cards.", "OK",
                    () => GlobalManagers.Instance.Popup.HideCurrentPopup());
                return;
            }
            
            // Serialize the deck to JSON
            var deckJson = Newtonsoft.Json.JsonConvert.SerializeObject(deckDTO);
            DebugWrapper.Log(deckJson);
            
            try
            {
                GlobalManagers.Instance.Popup.RegisterLoadingPopupToQueue("Saving Deck to Cloud");
                
                // Call the function within the module and provide the parameters we defined in there
                string result = await CloudCodeService.Instance.CallModuleEndpointAsync("Deck", "SaveDeckData",
                    new Dictionary<string, object> { { "key", DeckSaveKey }, { "value", deckJson } });
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
