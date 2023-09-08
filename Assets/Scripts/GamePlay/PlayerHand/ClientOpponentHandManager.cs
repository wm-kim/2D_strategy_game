using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Minimax.GamePlay.CommandSystem;
using Minimax.GamePlay.INetworkSerialize;
using Minimax.UI.View.ComponentViews.GamePlay;
using Minimax.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minimax
{
    public class ClientOpponentHandManager : MonoBehaviour
    {
        [BoxGroup("References")] [SerializeField]
        private HandCardView m_cardPrefab;

        [BoxGroup("References")] [SerializeField]
        private Transform m_cardParent;
        
        [BoxGroup("Card Settings")] [SerializeField, Tooltip("카드가 놓일 곡선의 반지름")] [Range(0, 10000)]
        private float m_curvRadius = 2000f;

        [BoxGroup("Card Settings")] [SerializeField, Tooltip("카드가 놓일 곡선의 각도")] [Range(0, 360)]
        private float m_curvAngle = 30f;

        [BoxGroup("Card Settings")] [SerializeField, Tooltip("카드가 놓일 곡선의 중심, m_cardParent를 기준으로 한다.")]
        Vector2 m_curvCenter = new Vector2(0, -200);

        [BoxGroup("Card Settings")] [SerializeField, Tooltip("카드가 놓일 곡선의 각도")] [Range(0, 360)]
        private float m_baseRotation = 0f;

        [BoxGroup("Card Settings")] [SerializeField, Tooltip("카드 사이의 최대 각도")] [Range(0, 30)]
        private float m_maxBetweenAngle = 3f;
        
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
            
            UpdateCardTransforms();
            TweenHandCards();
        }
        
        public void AddCardAndTween(int cardUID)
        {
            AddCard(cardUID);
            UpdateCardTransforms();
            TweenHandCards();
        }

        public void PlayCardAndTween(int cardUID)
        {
            try 
            {
                var card = RemoveCard(cardUID);
                // TODO : animate the card revealing, just for now destroy it.
                Destroy(card.gameObject);
                
                UpdateCardTransforms();
                TweenHandCards();
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

        private void UpdateCardTransforms()
        {
            float cardAngle = CardCount <= 1 ? 0 : m_curvAngle / (CardCount - 1);  // Adjust for a single card.
            cardAngle = Mathf.Min(cardAngle, m_maxBetweenAngle);

            float cardAngleOffset = (CardCount - 1) * cardAngle / 2;

            for (int i = 0; i < CardCount; i++)
            {
                float angle = cardAngle * i - cardAngleOffset;
                float radian = angle * Mathf.Deg2Rad;
                float baseRadian = m_baseRotation * Mathf.Deg2Rad;
                float x = m_curvCenter.x + m_curvRadius * Mathf.Sin(baseRadian + radian);
                float y = m_curvCenter.y + m_curvRadius * Mathf.Cos(baseRadian + radian);

                m_handCardPositions[i] = new Vector3(x, y, 0);

                // Rotate the card such that its end points to the curve's center point
                Vector2 directionToCenter = m_curvCenter - new Vector2(x, y);
                // Subtracting 90 degrees to align with the vertical
                float rotationAngle = Mathf.Atan2(directionToCenter.y, directionToCenter.x) * Mathf.Rad2Deg + 90f;

                m_handCardRotations[i] = Quaternion.Euler(0, 0, rotationAngle);
            }
        }
        
        private void TweenHandCards()
        {
            Sequence sequence = DOTween.Sequence();
            
            for (int i = 0; i < CardCount; i++)
            {
                m_handCards[i].KillTweens();
                m_handCards[i].PosTween = m_handCards[i].transform.DOLocalMove(m_handCardPositions[i], m_tweenDuration);
                m_handCards[i].RotTween = m_handCards[i].transform.DOLocalRotateQuaternion(m_handCardRotations[i], m_tweenDuration);
                
                sequence.Join(m_handCards[i].PosTween);
                sequence.Join(m_handCards[i].RotTween);
            }
            
            sequence.Play();
            sequence.OnComplete(Command.ExecutionComplete);
        }
    }
}
