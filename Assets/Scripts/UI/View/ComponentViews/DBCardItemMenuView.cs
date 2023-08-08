using Minimax.CoreSystems;
using Minimax.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Minimax.UI.View.ComponentViews
{
    public class DBCardItemMenuView : MonoBehaviour
    {
        [SerializeField] private DeckBuildingViewManager m_deckBuildingViewManager;
        
        [Header("Inner References")]
        [SerializeField] private RectTransform m_dbCardItemMenu = default;
        [SerializeField] private TextMeshProUGUI m_cardNameText = default;
        [SerializeField] private Button m_addToDeckButton = default;
        
        [SerializeField, ReadOnly] private UIVisibleState m_visibleState = UIVisibleState.Undefined;
        
        [SerializeField] private float m_xOffset = 30f;
        private DBCardItemView m_dbCardItem;
        
        private void Start()
        {
            m_dbCardItemMenu.gameObject.SetActive(false);
            GlobalManagers.Instance.Input.OnTouch += OnTouchOutside;
            m_addToDeckButton.onClick.AddListener(OnAddToDeckButtonClicked);
        }
        
        private void OnAddToDeckButtonClicked()
        {
            m_deckBuildingViewManager.DeckListView.AddCardToDeckList(m_dbCardItem.CardData);
            m_dbCardItem.SetButtonInteractable(false);
            Hide();
        }

        public void Show(DBCardItemView dbCardItem)
        {
            // Cache DBCardItem to be used later for disable interactable when added to deck list
            m_dbCardItem = dbCardItem;
            
            // Set Menu Position
            var dbCardItemTransform = m_dbCardItem.GetComponent<RectTransform>();

            var boundaryRect = m_deckBuildingViewManager.DBCardScrollView.GetComponent<RectTransform>().GetWorldRect();
            var boundaryPos = m_deckBuildingViewManager.DBCardScrollView.transform.position;
            
            var dbCardItemPos = dbCardItemTransform.position;
            var dbCardItemRect = dbCardItemTransform.GetWorldRect();
            var dbCardItemMenuRect = m_dbCardItemMenu.GetWorldRect();
            
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
        
        private void Hide()
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
