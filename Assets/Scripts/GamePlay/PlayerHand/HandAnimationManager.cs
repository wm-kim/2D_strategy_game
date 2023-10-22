using System;
using System.Collections.Generic;
using DG.Tweening;
using Minimax.GamePlay.CommandSystem;
using Minimax.Utilities;
using UnityEngine;

namespace Minimax.GamePlay.PlayerHand
{
    public class HandAnimationManager : MonoBehaviour
    {
        [Header("Hand Curve Settings")]
        [SerializeField, Tooltip("카드가 놓일 곡선의 반지름")] [Range(0, 10000)]
        private float m_curvRadius = 2000f;
        [SerializeField, Tooltip("카드가 놓일 곡선의 각도")] [Range(0, 360)]
        private float m_curvAngle = 30f;
        [SerializeField, Tooltip("카드가 놓일 곡선의 중심, m_cardParent를 기준으로 한다.")]
        Vector2 m_curvCenter = new Vector2(0, -200);
        [SerializeField, Tooltip("카드가 놓일 곡선의 각도")] [Range(0, 360)]
        private float m_baseRotation = 0f;
        [SerializeField, Tooltip("카드 사이의 최대 각도")] [Range(0, 30)]
        private float m_maxBetweenAngle = 3f;
        [SerializeField, Tooltip("카드 처음 생성 위치")]
        private Vector3 m_cardInitialPosition = new Vector3(0, 0, 0);
        [SerializeField, Tooltip("카드 처음 생성 회전값")]
        private Vector3 m_cardInitialRotation = new Vector3(0, 0, 0);
        
        [Header("Animation Settings")]
        [SerializeField, Tooltip("슬롯 정렬 애니메이션의 시간")]
        private float m_tweenDuration = 0.5f;
        
        private List<Vector3> m_slotPositionList = new List<Vector3>();
        private List<Quaternion> m_slotRotationList = new List<Quaternion>();

        private void Awake()
        {
            AllocateSlotTransforms();
        }
        
        private void AllocateSlotTransforms()
        {
            m_slotPositionList.Clear();
            m_slotRotationList.Clear();
            
            for (int i = 0; i < Define.MaxHandCardCount; i++)
            {
                m_slotPositionList.Add(Vector3.zero);
                m_slotRotationList.Add(Quaternion.identity);
            }
        }

        public void UpdateAndTweenHand<T>(List<T> slots) where T : TweenableItem
        {
            UpdateCardTransforms(slots.Count);
            TweenHand(slots);
        }
        
        private void UpdateCardTransforms(int cardCount)
        {
            float cardAngle = cardCount <= 1 ? 0 : m_curvAngle / (cardCount - 1);  // Adjust for a single card.
            cardAngle = Mathf.Min(cardAngle, m_maxBetweenAngle);

            float cardAngleOffset = (cardCount - 1) * cardAngle / 2;

            for (int i = 0; i < cardCount; i++)
            {
                float angle = cardAngle * i - cardAngleOffset;
                float radian = angle * Mathf.Deg2Rad;
                float baseRadian = m_baseRotation * Mathf.Deg2Rad;
                float x = m_curvCenter.x + m_curvRadius * Mathf.Sin(baseRadian + radian);
                float y = m_curvCenter.y + m_curvRadius * Mathf.Cos(baseRadian + radian);

                m_slotPositionList[i] = new Vector3(x, y, 0);

                // Rotate the card such that its end points to the curve's center point
                Vector2 directionToCenter = m_curvCenter - new Vector2(x, y);
                // Subtracting 90 degrees to align with the vertical
                float rotationAngle = Mathf.Atan2(directionToCenter.y, directionToCenter.x) * Mathf.Rad2Deg + 90f;

                m_slotRotationList[i] = Quaternion.Euler(0, 0, rotationAngle);
            }
        }

        private void TweenHand<T>(List<T> slots) where T : TweenableItem
        {
            // sequence auto play when it get out of scope just like IDisposable
            Sequence sequence = DOTween.Sequence();
            
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].StartLocalPosTween(m_slotPositionList[i], m_tweenDuration);
                slots[i].StartLocalRotQuaternionTween(m_slotRotationList[i], m_tweenDuration);
                
                sequence.Join(slots[i].PosTween);
                sequence.Join(slots[i].RotTween);
            }

            sequence.Play();
            sequence.OnComplete(Command.ExecutionComplete);
        }
    }
}