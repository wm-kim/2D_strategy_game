using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minimax
{
    public class CardListView : UIFader
    {
        [Header("References")]
        [SerializeField] private CardDBManager m_cardDBManager;
        [SerializeField] private CardVisual m_cardListItemViewPrefab;
        [SerializeField] private Transform m_parentTransform;

        private List<CardVisual> m_cardVisuals = new List<CardVisual>();
        
        /// <summary>
        /// Initializes the card list view with the given card ids.
        /// </summary>
        public void Init(List<int> cardIds)
        {
            foreach (var cardId in cardIds)
            {
                var cardData = m_cardDBManager.GetCardData(cardId);
                var cardVisual = Instantiate(m_cardListItemViewPrefab, m_parentTransform);
                cardVisual.Init(cardData);
                m_cardVisuals.Add(cardVisual);
            }
        }
    }
}
