using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Minimax.ScriptableObjects.CardData;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Utilities;
using Debug = Utilities.Debug;

namespace Minimax.DeckBuliding
{
    /// <summary>
    /// CardDBManager는 카드 데이터베이스를 관리합니다.
    /// </summary>
    public class CardDBManager : MonoBehaviour
    {
        [Header("Addressable Assets")] public AssetLabelReference m_dbCardAssetsLabel;

        private Dictionary<int, CardBaseData> m_cardDB = new();
        public  Dictionary<int, CardBaseData> CardDB => m_cardDB;

        public CardBaseData GetCardData(int cardID)
        {
            if (m_cardDB.TryGetValue(cardID, out var data)) return data;
            Debug.LogError($"CardDBManager: Card ID {cardID} not found in DB");
            return null;
        }

        public async UniTask<bool> LoadDBCardsAsync()
        {
            var loadOp = Addressables.LoadAssetsAsync<CardBaseData>(m_dbCardAssetsLabel, null);
            await loadOp.Task;

            if (loadOp.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                foreach (var cardData in loadOp.Result) m_cardDB.Add(cardData.CardId, cardData);
                return true;
            }
            else
            {
                Debug.LogError("CardDBManager: Failed to load DB cards");
                return false;
            }
        }
    }
}