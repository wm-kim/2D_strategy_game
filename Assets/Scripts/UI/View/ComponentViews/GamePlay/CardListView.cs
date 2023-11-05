using System.Collections.Generic;
using Minimax.GamePlay.Card;
using Minimax.GamePlay.PlayerDeck;
using UnityEngine;

namespace Minimax.UI.View.ComponentViews.GamePlay
{
    public class CardListView : UIFader
    {
        [Header("References")] [SerializeField]
        private ClientMyDeckManager m_clientPlayerDeckManager;

        [SerializeField] private CardVisual m_cardListItemViewPrefab;
        [SerializeField] private Transform  m_parentTransform;

        private Dictionary<int, CardVisual> m_cardVisuals = new();

        public void Init(int[] cardUIds)
        {
            foreach (var cardId in cardUIds)
            {
                var cardVisual = Instantiate(m_cardListItemViewPrefab, m_parentTransform);
                var cardData   = ClientCard.CardsCreatedThisGame[cardId].Data;
                m_cardVisuals.Add(cardId, cardVisual);
                cardVisual.Init(cardData);
            }
        }

        private void Start()
        {
            m_clientPlayerDeckManager.OnCardRemovedFromDeck += OnCardRemovedFromDeck;
        }

        private void OnCardRemovedFromDeck(int cardUID)
        {
            // remove the card from the list and destroy it
            var cardVisual = m_cardVisuals[cardUID];
            m_cardVisuals.Remove(cardUID);
            Destroy(cardVisual.gameObject);
        }
    }
}