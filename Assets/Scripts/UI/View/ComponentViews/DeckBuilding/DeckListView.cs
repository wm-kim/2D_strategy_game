using System.Collections.Generic;
using Minimax.ScriptableObjects;
using Minimax.ScriptableObjects.CardDatas;
using UnityEngine;

namespace Minimax.UI.View.ComponentViews.DeckBuilding
{
    public class DeckListView : MonoBehaviour
    {
        [SerializeField] private DeckBuildingManager m_deckBuildingManager;

        private DeckListSO m_deckListSO;
        
        [SerializeField] private DeckListItemView m_deckListItemPrefab;
        public Transform m_deckListItemParent;
        
        private Dictionary<int, DeckListItemView> m_deckListItemViews = new Dictionary<int, DeckListItemView>();

        private void Start()
        {
            m_deckListSO = m_deckBuildingManager.DeckListSO;
            m_deckListSO.Init();
        }

        public void AddCardToDeckList(CardBaseData cardData)
        {
            if (m_deckListSO.ContainsCard(cardData.CardId)) return;
            m_deckListSO.AddCard(cardData);
            
            // Instantiate DeckListItemView
            var deckListItemView = Instantiate(m_deckListItemPrefab, m_deckListItemParent);
            deckListItemView.Init(cardData, m_deckBuildingManager);
            m_deckListItemViews.Add(cardData.CardId, deckListItemView);
        }
        
        public void RemoveCardFromDeckList(int cardId)
        {
            if (!m_deckListSO.ContainsCard(cardId)) return;
            m_deckListSO.RemoveCard(cardId);
            
            // Destroy DeckListItemView
            Destroy(m_deckListItemViews[cardId].gameObject);
            m_deckListItemViews.Remove(cardId);
        }
        
        
        private void UpdateListView()
        {
        }
    }
}
