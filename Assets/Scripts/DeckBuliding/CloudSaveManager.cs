using System.Collections.Generic;
using Minimax.ScriptableObjects;
using Minimax.Utilities;
using Unity.Services.CloudSave;
using UnityEngine;

namespace Minimax
{
    public class CloudSaveManager : MonoBehaviour
    {
        [SerializeField] private DeckDataSO _deckDataSO;

        public async void SaveDeckToCloud()
        {
            var deckList = _deckDataSO.GetDeckList();
            var deckListJson = Newtonsoft.Json.JsonConvert.SerializeObject(deckList);
            var data = new Dictionary<string, object>{{"deckList", deckListJson}};
            try
            {
                await CloudSaveService.Instance.Data.ForceSaveAsync(data);
                DebugWrapper.Log("Deck Saved to Cloud");
            }
            catch (CloudSaveValidationException e)
            {
                DebugWrapper.LogException(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                DebugWrapper.LogException(e);
            }
            catch (CloudSaveException e)
            {
                DebugWrapper.LogException(e);
            }
        }
    }
}
