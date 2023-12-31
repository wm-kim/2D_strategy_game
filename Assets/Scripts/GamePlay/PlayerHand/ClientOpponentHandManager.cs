using System;
using System.Collections.Generic;
using Minimax.Definitions;
using Minimax.UI.View.ComponentViews.GamePlay;
using UnityEngine;
using Utilities;
using Debug = Utilities.Debug;

namespace Minimax.GamePlay.PlayerHand
{
    public class ClientOpponentHandManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private HandAnimationManager m_handAnimationManager;

        [SerializeField]
        private HandCardView m_cardPrefab;

        [SerializeField]
        private Transform m_cardParent;

        private List<HandCardView> m_handCards = new();

        /// <summary>
        /// for keeping track of the index of card uids in the opponent's hand.
        /// </summary>
        private List<int> m_cardUIDs = new();

        public int CardCount => m_handCards.Count;

        public void AddInitialCardsAndTween(int[] cardUIDs)
        {
            foreach (var cardUID in cardUIDs) AddCard(cardUID);
            m_handAnimationManager.UpdateAndTweenHand(m_handCards);
        }

        public void AddCardAndTween(int cardUID)
        {
            AddCard(cardUID);
            m_handAnimationManager.UpdateAndTweenHand(m_handCards);
        }

        public void PlayCardAndTween(int cardUID)
        {
            try
            {
                var card = RemoveCard(cardUID);
                m_handAnimationManager.TweenOpponentCardReveal(cardUID, card);
                m_handAnimationManager.UpdateAndTweenHand(m_handCards);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        private HandCardView RemoveCard(int cardUID)
        {
            var index = FindIndexOfCardUID(cardUID);
            var card  = m_handCards[index];
            m_handCards.RemoveAt(index);
            m_cardUIDs.RemoveAt(index);
            return card;
        }

        private int FindIndexOfCardUID(int cardUID)
        {
            for (var i = 0; i < CardCount; i++)
                if (m_cardUIDs[i] == cardUID)
                    return i;

            throw new Exception($"Cannot find card with UID {cardUID} in opponent's hand.");
        }

        private void AddCard(int cardUID)
        {
            if (!CheckMaxHandCardCount()) return;

            var handCardView = Instantiate(m_cardPrefab, m_cardParent);
            m_handAnimationManager.SetInitialTransform(handCardView);

            handCardView.CreateClientCardAndSetVisual(cardUID);
            m_cardUIDs.Add(cardUID);
            m_handCards.Add(handCardView);
        }

        private bool CheckMaxHandCardCount()
        {
            if (CardCount >= Define.MaxHandCardCount)
            {
                Debug.LogWarning("상대방의 손패가 가득 찼습니다.");
                return false;
            }

            return true;
        }
    }
}