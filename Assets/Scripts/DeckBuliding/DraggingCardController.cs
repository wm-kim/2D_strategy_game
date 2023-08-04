using System;
using System.Collections;
using System.Collections.Generic;
using Minimax.CoreSystems;
using Minimax.ScriptableObjects.Events.Primitives;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minimax
{
    public class DraggingCardController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject m_draggingCardPrefab = default;
        
        [Header("Listening To")]
        [SerializeField] private Vector2EventSO m_touchPositionEvent = default;
        
        [Header("Settings")]
        [SerializeField, Range(0f, 20f)] private float m_moveSpeed = 10f;

        private void Awake()
        {
            m_draggingCardPrefab.SetActive(false);
        }
        
        private void DisplayDraggingCard()
        {
            m_draggingCardPrefab.SetActive(true);
        }
        
        private void HideDraggingCard()
        {
            m_draggingCardPrefab.SetActive(false);
        }

        private void Update()
        {
            m_draggingCardPrefab.transform.position = 
                Vector3.Lerp(m_draggingCardPrefab.transform.position, m_touchPositionEvent.Value, Time.deltaTime * m_moveSpeed);
        }
    }
}
