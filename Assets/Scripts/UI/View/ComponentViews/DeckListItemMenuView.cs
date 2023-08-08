using System;
using Minimax.CoreSystems;
using Minimax.UI;
using UnityEngine;
using UnityEngine.UI;
using Minimax.Utilities;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Minimax.UI.View.ComponentViews
{
    public class DeckListItemMenuView : MonoBehaviour
    {
        [SerializeField] private DeckBuildingViewManager m_deckBuildingViewManager;
        
        [Header("Inner References")]
        [SerializeField] private RectTransform m_deckListItemMenu;
        [SerializeField] private Button m_deleteFromDeckButton;
        
        [SerializeField, ReadOnly] private UIVisibleState m_visibleState = UIVisibleState.Undefined;
        
        [SerializeField] private float m_xOffset = 30f;
        private DeckListItemView m_deckListItemView;
        
        private void Start()
        {
            m_deckListItemMenu.gameObject.SetActive(false);
            GlobalManagers.Instance.Input.OnTouch += OnTouchOutside;
            m_deleteFromDeckButton.onClick.AddListener(OnDeleteFromDeckButtonClicked);
        }

        private void OnTouchOutside(Vector2 touchPosition, TouchPhase touchPhase)
        {
            if (m_visibleState == UIVisibleState.Appeared)
            {
                bool isBeginOrMovedTouch = touchPhase == TouchPhase.Began || touchPhase == TouchPhase.Moved;
                bool isOutsideCardDisplayMenu = !RectTransformUtility.RectangleContainsScreenPoint(m_deckListItemMenu, touchPosition);
                if (isBeginOrMovedTouch && isOutsideCardDisplayMenu)
                {
                    Hide();
                }
            }
        }
        
        public void Show(DeckListItemView deckListItemView)
        {
            // Cache deckListItemView to be used later for removing from deck list
            m_deckListItemView = deckListItemView;
            
            // Set Menu Position
            var deckListItemTransform = deckListItemView.GetComponent<RectTransform>();

            var boundaryRect = m_deckBuildingViewManager.DeckListView.GetComponent<RectTransform>().GetWorldRect();
            var boundaryPos = m_deckBuildingViewManager.DeckListView.transform.position;
            
            var deckListItemPos = deckListItemTransform.position;
            var deckListItemRect = deckListItemTransform.GetWorldRect();
            var deckListItemMenuRect = m_deckListItemMenu.GetWorldRect();
            
            var dbCardItemMenuPosY = deckListItemPos.y;
            
            float clampPosY = Mathf.Clamp(dbCardItemMenuPosY, 
                boundaryPos.y - boundaryRect.height / 2f + deckListItemMenuRect.height / 2f,
                boundaryPos.y + boundaryRect.height / 2f - deckListItemMenuRect.height / 2f);
            
            var deckListItemMenuPosX = deckListItemPos.x - (deckListItemRect.width + deckListItemMenuRect.width) / 2f - m_xOffset;
            
            m_deckListItemMenu.position = new Vector3(
                deckListItemMenuPosX,
                clampPosY,
                0f);
            
            m_deckListItemMenu.gameObject.SetActive(true);
            m_visibleState = UIVisibleState.Appeared;
        }

        private void Hide()
        {
            m_deckListItemMenu.gameObject.SetActive(false);
            m_visibleState = UIVisibleState.Disappeared;
        }

        private void OnDeleteFromDeckButtonClicked()
        {
            var cardId = m_deckListItemView.CardData.CardId;
            m_deckBuildingViewManager.DeckListView.RemoveCardFromDeckList(cardId);
            Hide();
         
            m_deckBuildingViewManager.DBCardScrollView.SetDBCardItemViewInteractable(cardId, true);
        }
    }
}
