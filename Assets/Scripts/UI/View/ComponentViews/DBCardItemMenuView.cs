using System;
using System.Collections;
using System.Collections.Generic;
using Minimax.CoreSystems;
using Minimax.ScriptableObjects.CardDatas;
using Minimax.ScriptableObjects.Events;
using Minimax.ScriptableObjects.Events.Primitives;
using Minimax.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Minimax.UI.View.ComponentViews
{
    public class DBCardItemMenuView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform m_dbCardItemMenu = default;
        [SerializeField] private TextMeshProUGUI m_cardNameText = default;
        [SerializeField] private Button m_addToDeckButton = default;
        
        [Header("Boundary Rect")]
        [SerializeField] private RectTransform m_boundaryRect = default;
        
        [Header("Listening To")]
        [SerializeField] private DBCardItemMenuEventSO m_dbCardItemMenuEvent = default;
        
        [Header("Broadcasting On")]
        [SerializeField] private IntEventSO m_addToDeckButtonClickedEvent = default;

        [SerializeField, ReadOnly] private UIVisibleState m_visibleState = UIVisibleState.Undefined;
        
        [SerializeField] private float m_xOffset = 30f;
        private DBCardItemView m_dbCardItem; 
        
        private void Start()
        {
            m_dbCardItemMenu.gameObject.SetActive(false);
            GlobalManagers.Instance.Input.OnTouch += OnTouchOutside;
            
            m_dbCardItemMenuEvent.OnEventRaised.AddListener(OnDBCardItemMenuEventRaised);
            m_addToDeckButton.onClick.AddListener(OnAddToDeckButtonClicked);
        }
        
        private void OnDBCardItemMenuEventRaised(DBCardItemView dbCardItem, bool isShow)
        {
            if (isShow) Show(dbCardItem);
            else Hide();
        }
        
        private void OnAddToDeckButtonClicked()
        {
            m_addToDeckButtonClickedEvent.RaiseEvent(m_dbCardItem.CardData.CardId);
            m_dbCardItem.Button.interactable = false;
            Hide();
        }

        public void Show(DBCardItemView dbCardItem)
        {
            m_dbCardItem = dbCardItem;
            
            // Set Menu Position
            var dbCardItemTransform = dbCardItem.GetComponent<RectTransform>();
            
            var boundaryPos = m_boundaryRect.position;
            var boundaryRect = m_boundaryRect.rect;
            
            var dbCardItemPos = dbCardItemTransform.position;
            var dbCardItemRect = dbCardItemTransform.rect;
            var dbCardItemMenuRect = m_dbCardItemMenu.rect;
            
            var dbCardItemMenuPosY = dbCardItemPos.y + (dbCardItemRect.height - dbCardItemMenuRect.height) / 2f;
            var dbCardItemMenuPosXRight = dbCardItemPos.x + (dbCardItemRect.width + dbCardItemMenuRect.width) / 2f + m_xOffset;
            var dbCardItemMenuPosXLeft = dbCardItemPos.x - (dbCardItemRect.width + dbCardItemMenuRect.width) / 2f - m_xOffset;
            
            float clampPosY = Mathf.Clamp(dbCardItemMenuPosY, 
                boundaryPos.y - boundaryRect.height / 2f + dbCardItemMenuRect.height / 2f,
                boundaryPos.y + boundaryRect.height / 2f - dbCardItemMenuRect.height / 2f);
            
            var dbCardItemMenuPosX = (dbCardItemMenuPosXRight > boundaryPos.x + (boundaryRect.width - dbCardItemMenuRect.width) / 2f) ? 
                dbCardItemMenuPosXLeft : dbCardItemMenuPosXRight;
            
            m_dbCardItemMenu.position = new Vector3(
                dbCardItemMenuPosX,
                 clampPosY,
                0f);
      
            m_dbCardItemMenu.gameObject.SetActive(true);
            m_visibleState = UIVisibleState.Appeared;
            
            // Set Card Name Text
            m_cardNameText.text = $"{m_dbCardItem.CardData.CardName}";
        }
        
        public void Hide()
        {
            m_dbCardItemMenu.gameObject.SetActive(false);
            m_visibleState = UIVisibleState.Disappeared;
        }
        
        private void OnTouchOutside(Vector2 touchPosition, TouchPhase touchPhase)
        {
            if (m_visibleState == UIVisibleState.Appeared)
            {
                bool isBeginOrMovedTouch = touchPhase == TouchPhase.Began || touchPhase == TouchPhase.Moved;
                bool isOutsideCardDisplayMenu = !RectTransformUtility.RectangleContainsScreenPoint(m_dbCardItemMenu, touchPosition);
                if (isBeginOrMovedTouch && isOutsideCardDisplayMenu)
                {
                    Hide();
                }
            }
        }
    }
}
