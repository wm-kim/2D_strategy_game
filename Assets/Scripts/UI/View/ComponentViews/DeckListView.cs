using System;
using System.Collections.Generic;
using Minimax.ScriptableObjects;
using Minimax.ScriptableObjects.CardDatas;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minimax
{
    public class DeckListView : MonoBehaviour
    {
        [SerializeField] private DeckBuildingManager m_deckBuildingManager;
        private DeckDataSO m_deckDataSO;
        
        [SerializeField] private DeckListItemView m_deckListItemPrefab;
        public Transform m_deckListItemParent;
        
        private Dictionary<int, DeckListItemView> m_deckListItemViews = new Dictionary<int, DeckListItemView>();

        private void Start()
        {
            m_deckDataSO = m_deckBuildingManager.DeckDataSO;
            m_deckDataSO.Init();
        }

        public void AddCardToDeckList(CardBaseData cardData)
        {
            if (m_deckDataSO.ContainsCard(cardData.CardId)) return;
            m_deckDataSO.AddCard(cardData);
            
            // Instantiate DeckListItemView
            var deckListItemView = Instantiate(m_deckListItemPrefab, m_deckListItemParent);
            deckListItemView.Init(cardData, m_deckBuildingManager);
            m_deckListItemViews.Add(cardData.CardId, deckListItemView);
        }
        
        public void RemoveCardFromDeckList(int cardId)
        {
            if (!m_deckDataSO.ContainsCard(cardId)) return;
            m_deckDataSO.RemoveCard(cardId);
            
            // Destroy DeckListItemView
            Destroy(m_deckListItemViews[cardId].gameObject);
            m_deckListItemViews.Remove(cardId);
        }
        
        private void UpdateListView()
        {
        }
    }
}
