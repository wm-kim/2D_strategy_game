using System.Collections.Generic;
using Minimax.ScriptableObjects;
using Minimax.ScriptableObjects.CardDatas;
using Minimax.ScriptableObjects.Events.Primitives;
using UnityEngine;

namespace Minimax
{
    public class DeckListView : MonoBehaviour
    {
        [SerializeField] private CardDBManager m_cardDBManager;
        [SerializeField] private DeckDataSO m_deckDataSO;
        
        public GameObject m_deckListItemPrefab;
        public Transform m_deckListItemParent;
        
        [Header("Listening To")]
        [SerializeField] private IntEventSO m_onCardAddedToDeckEvent;

        private SortedDictionary<int, CardBaseData> m_deckList = new SortedDictionary<int, CardBaseData>();

        private void Start()
        {
            m_onCardAddedToDeckEvent.OnEventRaised.AddListener(AddCardToDeckList);
        }

        private void AddCardToDeckList(int cardID)
        {
            var cardData = m_cardDBManager.GetCardData(cardID);
            if (m_deckList.ContainsKey(cardData.CardId)) return;
            m_deckDataSO.AddCard(cardData);
            var deckListItem = Instantiate(m_deckListItemPrefab, m_deckListItemParent).GetComponent<DeckListItemView>();
            deckListItem.SetData(cardData);
        }
        
        private void RemoveCardFromDeckList(CardBaseData cardData)
        {
            if (!m_deckList.ContainsKey(cardData.CardId)) return;
            m_deckList.Remove(cardData.CardId);
            
        }
        
        private void UpdateListView()
        {
        }
    }
}
