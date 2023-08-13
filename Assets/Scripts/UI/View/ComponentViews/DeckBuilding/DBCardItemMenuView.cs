using Minimax.CoreSystems;
using Minimax.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Minimax.UI.View.ComponentViews.DeckBuilding
{
    public class DBCardItemMenuView : StatefulUIView
    {
        [SerializeField] private DeckBuildingManager m_deckBuildingManager;
        
        [Header("Inner References")]
        [SerializeField] private RectTransform m_dbCardItemMenu = default;
        [SerializeField] private TextMeshProUGUI m_cardNameText = default;
        [SerializeField] private Button m_addToDeckButton = default;
        
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
            m_deckBuildingManager.DeckListView.AddCardToDeckList(m_dbCardItem.CardData);
            m_dbCardItem.SetButtonInteractable(false);
            StartHide();
        }

        protected override void Show(float transitionDuration = 0)
        {
            m_dbCardItem = m_deckBuildingManager.SelectedDBCardItemView;
            
            // Set Menu Position
            var dbCardItemTransform = m_dbCardItem.GetComponent<RectTransform>();

            var boundaryRect = m_deckBuildingManager.DBCardScrollView.GetComponent<RectTransform>().GetWorldRect();
            var boundaryPos = m_deckBuildingManager.DBCardScrollView.transform.position;
            
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
            SetAppearedState();
            
            // Set Card Name Text
            m_cardNameText.text = $"{m_dbCardItem.CardData.CardName}";
        }
        
        protected override void Hide(float transitionDuration = 0)
        {
            m_dbCardItemMenu.gameObject.SetActive(false);
            SetDisappearedState();
        }
        
        private void OnTouchOutside(Vector2 touchPosition, TouchPhase touchPhase)
        {
            if (m_currentState == UIVisibleState.Appeared)
            {
                bool isBeginOrMovedTouch = touchPhase == TouchPhase.Began || touchPhase == TouchPhase.Moved;
                bool isOutsideCardDisplayMenu = !RectTransformUtility.RectangleContainsScreenPoint(m_dbCardItemMenu, touchPosition);
                if (isBeginOrMovedTouch && isOutsideCardDisplayMenu)
                {
                    StartHide();
                }
            }
        }
    }
}
