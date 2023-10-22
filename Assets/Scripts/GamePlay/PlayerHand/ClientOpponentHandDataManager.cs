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
    public class ClientOpponentHandDataManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private HandAnimationManager m_handAnimationManager;
        [SerializeField] private HandCardView m_cardPrefab;
        [SerializeField] private Transform m_cardParent;
        
        [BoxGroup("Card Settings")] [SerializeField, Tooltip("카드 처음 생성 위치")]
        private Vector3 m_cardInitialPosition = new Vector3(0, 0, 0);
        
        [BoxGroup("Card Settings")] [SerializeField, Tooltip("카드 처음 생성 회전값")]
        private Vector3 m_cardInitialRotation = new Vector3(0, 0, 0);
        
        [BoxGroup("Animation Settings")] [SerializeField, Tooltip("슬롯 정렬 애니메이션의 시간")]
        private float m_tweenDuration = 0.5f;
        
        private List<HandCardView> m_handCards = new List<HandCardView>();
        
        /// <summary>
        /// for keeping track of the index of card uids in the opponent's hand.
        /// </summary>
        private List<int> m_cardUIDs = new List<int>();
        
        
        private List<Vector3> m_handCardPositions = new List<Vector3>();
        private List<Quaternion> m_handCardRotations = new List<Quaternion>();
        
        public int CardCount => m_handCards.Count;

        private void Awake()
        {
            // Memory Allocation
            for (int i = 0; i < Define.MaxHandCardCount; i++)
            {
                m_handCardPositions.Add(Vector3.zero);
                m_handCardRotations.Add(Quaternion.identity);
            }
        }

        public void AddInitialCardsAndTween(int[] cardUIDs)
        {
            foreach (var cardUID in cardUIDs)
            {
                AddCard(cardUID);
            }
            
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
            if (CardCount >= Define.MaxHandCardCount)
            {
                DebugWrapper.LogWarning("상대방의 손패에 카드를 추가하려고 했지만, 손패가 꽉 찼습니다.");
                return;
            }
            
            var card = Instantiate(m_cardPrefab, m_cardParent.transform.position + m_cardInitialPosition,
                Quaternion.Euler(m_cardInitialRotation), m_cardParent);
            
            card.Init(cardUID);
            m_cardUIDs.Add(cardUID);
            m_handCards.Add(card);
        }
    }
}
