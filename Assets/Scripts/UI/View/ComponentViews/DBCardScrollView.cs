using System.Collections.Generic;
using Minimax.ScriptableObjects.CardDatas;
using Minimax.ScriptableObjects.Events;
using Minimax.UI.View.ComponentViews;
using Minimax.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace Minimax
{
    public class DBCardScrollView : MonoBehaviour
    {
        [SerializeField] private DeckBuildingManager m_deckBuildingManager;
        [SerializeField] private CardDBManager m_cardDBManager;
        
        [Header("Card Prefab")]
        public GameObject m_dbCardItemPrefab;
        public Transform m_dbCardItemParent;
        
        [Header("Listening To")]
        [SerializeField] private VoidEventSO m_onDBCardsLoadedEvent;
        
        private Dictionary<int, DBCardItemView> m_dbCardItems = new Dictionary<int, DBCardItemView>();

        private void Start()
        {
            m_onDBCardsLoadedEvent.OnEventRaised.AddListener(OnDBCardsLoaded);
        }
        
        private void OnDBCardsLoaded()
        {
            for (int i = 0; i < m_cardDBManager.CardDB.Count; i++)
            {
                var cardData = m_cardDBManager.CardDB[i];
                var dbCardItem = Instantiate(m_dbCardItemPrefab, m_dbCardItemParent).GetComponent<DBCardItemView>();
                dbCardItem.Init(cardData, m_deckBuildingManager);
                m_dbCardItems.Add(cardData.CardId, dbCardItem);
            }
        }
        
        public void SetDBCardItemViewInteractable(int cardId, bool interactable)
        {
            if (!m_dbCardItems.ContainsKey(cardId)) return;
            m_dbCardItems[cardId].SetButtonInteractable(interactable);
        }
    }
}
