using System;
using System.Collections.Generic;
using Minimax.UI.View.ComponentViews.GamePlay;
using UnityEngine;

namespace Minimax
{
    public class ClientPlayerHandManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CardView m_cardPrefab;
        [SerializeField] private Transform m_cardParent;
        
        [Header("Settings")]
        [SerializeField, Tooltip("카드가 놓일 곡선의 반지름")]
        [Range(0, 10000)]
        float m_curvRadius = 2000f;
        [SerializeField, Tooltip("카드가 놓일 곡선의 각도")]
        [Range(0, 360)]
        float m_curvAngle = 30f;
        [SerializeField, Tooltip("카드가 놓일 곡선의 중심, m_cardParent를 기준으로 한다.")]
        Vector2 m_curvCenter = new Vector2(0, -200);
        [Header("Rotation Base")]
        [SerializeField, Tooltip("카드가 놓일 곡선의 각도")]
        [Range(0, 360)]
        float m_baseRotation = 0f;
        
        
        private List<CardView> m_cardList = new List<CardView>();

        private void Start()
        {
            AddCardFromDeck(10);
        }

        // 아직 덱이 구현이 안되어 있어서 임시로 만들어 놓은 함수
        public void AddCardFromDeck(int num)
        {   
            for (int i = 0; i < num; i++)
            {
                CardView card = Instantiate(m_cardPrefab, m_cardParent);
                m_cardList.Add(card);
            }
        }
        
        /// <summary>
        /// 카드의 위치를 업데이트한다. 카드 손패에 추가되거나 제거되었을 때 마다 호출해야 한다.
        /// </summary>
        private void UpdateCardPosition()
        {
            float cardNum = m_cardList.Count;
            if (cardNum <= 1) return; // Avoid division by zero if only one card.
            
            float cardAngle = m_curvAngle / (cardNum - 1);
            float cardAngleOffset = (cardNum - 1) * cardAngle / 2;
            
            for (int i = 0; i < cardNum; i++)
            {
                float angle = cardAngle * i - cardAngleOffset;
                float radian = angle * Mathf.Deg2Rad;
                float baseRadian = m_baseRotation * Mathf.Deg2Rad;
                float x = m_curvCenter.x + m_curvRadius * Mathf.Sin(baseRadian + radian);
                float y = m_curvCenter.y +  m_curvRadius * Mathf.Cos(baseRadian + radian);
                
                m_cardList[i].transform.localPosition = new Vector3(x, y, 0);
                
                // Rotate the card such that its end points to the curve's center point
                Vector2 directionToCenter = m_curvCenter - new Vector2(x, y);
                // Subtracting 90 degrees to align with the vertical
                float rotationAngle = Mathf.Atan2(directionToCenter.y, directionToCenter.x) * Mathf.Rad2Deg - 90f;  
                m_cardList[i].transform.localRotation = Quaternion.Euler(0, 0, rotationAngle);
            }
        }

        private void Update()
        {
            UpdateCardPosition();
        }
    }
}
