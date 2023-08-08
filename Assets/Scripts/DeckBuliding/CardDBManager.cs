using System;
using System.Collections;
using System.Collections.Generic;
using Minimax.ScriptableObjects.CardDatas;
using Minimax.ScriptableObjects.Events;
using Minimax.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Minimax
{
    public class CardDBManager : MonoBehaviour
    {
        [Header("Addressable Assets")]
        public AssetLabelReference m_dbCardAssetsLabel;

        [Header("Broadcasting on")] 
        [SerializeField] private VoidEventSO m_onDBCardsLoadedEvent;
        
        private Dictionary<int, CardBaseData> m_cardDB = new Dictionary<int, CardBaseData>();
        
        public Dictionary<int, CardBaseData> CardDB => m_cardDB;
        
        public CardBaseData GetCardData(int cardID)
        {
            if (m_cardDB.TryGetValue(cardID, out var data)) return data;
            DebugWrapper.LogError($"CardDBManager: Card ID {cardID} not found in DB");
            return null;
        }
        
        private void Awake()
        {
            Addressables.LoadAssetsAsync<CardBaseData>(m_dbCardAssetsLabel, null).Completed += OnLoadDBCardsCompleted;
        }
        
        private void OnLoadDBCardsCompleted(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<IList<CardBaseData>> obj)
        {
            if (obj.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                foreach (var cardData in obj.Result) m_cardDB.Add(cardData.CardId, cardData);
                m_onDBCardsLoadedEvent.RaiseEvent();
            }
            else
            {
                DebugWrapper.LogError("DBCardScrollView: Failed to load DB cards");
            }
        }
    }
}
