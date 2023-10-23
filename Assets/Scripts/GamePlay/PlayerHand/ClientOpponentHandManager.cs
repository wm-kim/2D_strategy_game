using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Minimax.GamePlay.CommandSystem;
using Minimax.GamePlay.PlayerHand;
using Minimax.UI.View.ComponentViews.GamePlay;
using Minimax.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minimax
{
    public class ClientOpponentHandManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private HandAnimationManager m_handAnimationManager;
        [SerializeField] private HandCardView m_cardPrefab;
        [SerializeField] private Transform m_cardParent;
        
        private List<HandCardView> m_handCards = new List<HandCardView>();
        
        /// <summary>
        /// for keeping track of the index of card uids in the opponent's hand.
        /// </summary>
        private List<int> m_cardUIDs = new List<int>();
        
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
                // TODO : animate the card revealing, just for now destroy it.
                Destroy(card.gameObject);
                m_handAnimationManager.UpdateAndTweenHand(m_handCards);
            }
            catch (Exception e)
            {
                DebugWrapper.LogError(e.Message);
            }
        }

        private HandCardView RemoveCard(int cardUID)
        { 
            int index = FindIndexOfCardUID(cardUID);
            var card = m_handCards[index];
            m_handCards.RemoveAt(index);
            m_cardUIDs.RemoveAt(index);
            return card;
        }

        private int FindIndexOfCardUID(int cardUID)
        {
            for (int i = 0; i < CardCount; i++)
            {
                if (m_cardUIDs[i] == cardUID)
                {
                    return i;
                }
            }
            
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
                DebugWrapper.LogWarning("상대방의 손패가 가득 찼습니다.");
                return false;
            }

            return true;
        }
    }
}
