using Minimax.CoreSystems;
using Minimax.DeckBuliding;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Minimax.UI.View.ComponentViews.DeckBuilding
{
    public class DeckListItemMenuView : StatefulUIView
    {
        [Header("References")]
        [SerializeField]
        private Camera m_mainCamera;

        [SerializeField]
        private DeckBuildingManager m_deckBuildingManager;

        [Header("Inner References")]
        [SerializeField]
        private RectTransform m_deckListItemMenu;

        [SerializeField]
        private Button m_deleteFromDeckButton;

        [SerializeField]
        private float m_xOffset = 30f;

        private DeckListItemView m_deckListItemView;

        private void Start()
        {
            m_deckListItemMenu.gameObject.SetActive(false);
            GlobalManagers.Instance.Input.OnTouch += OnTouchOutside;
            m_deleteFromDeckButton.onClick.AddListener(OnDeleteFromDeckButtonClicked);
        }

        private void OnTouchOutside(EnhancedTouch.Touch touch)
        {
            if (m_currentState == UIVisibleState.Appeared)
            {
                var isBeginOrMovedTouch = touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved;
                var isOutsideCardDisplayMenu =
                    !RectTransformUtility.RectangleContainsScreenPoint(m_deckListItemMenu, touch.screenPosition,
                        m_mainCamera);
                if (isBeginOrMovedTouch && isOutsideCardDisplayMenu) StartHide();
            }
        }

        protected override void Show(float transitionDuration = 0.0f)
        {
            // Cache deckListItemView to be used later for removing from deck list
            m_deckListItemView = m_deckBuildingManager.SelectedDeckListItemView;

            // Set Menu Position
            var deckListItemTransform = m_deckListItemView.GetComponent<RectTransform>();

            var boundaryRect = m_deckBuildingManager.DeckListView.GetComponent<RectTransform>().GetWorldRect();
            var boundaryPos  = m_deckBuildingManager.DeckListView.transform.position;

            var deckListItemPos      = deckListItemTransform.position;
            var deckListItemRect     = deckListItemTransform.GetWorldRect();
            var deckListItemMenuRect = m_deckListItemMenu.GetWorldRect();

            var dbCardItemMenuPosY = deckListItemPos.y;

            var clampPosY = Mathf.Clamp(dbCardItemMenuPosY,
                boundaryPos.y - boundaryRect.height / 2f + deckListItemMenuRect.height / 2f,
                boundaryPos.y + boundaryRect.height / 2f - deckListItemMenuRect.height / 2f);

            var deckListItemMenuPosX = deckListItemPos.x - (deckListItemRect.width + deckListItemMenuRect.width) / 2f -
                                       m_xOffset;

            m_deckListItemMenu.position = new Vector3(
                deckListItemMenuPosX,
                clampPosY,
                0f);

            m_deckListItemMenu.gameObject.SetActive(true);
            SetAppearedState();
        }

        protected override void Hide(float transitionDuration = 0.0f)
        {
            m_deckListItemMenu.gameObject.SetActive(false);
            SetDisappearedState();
        }

        private void OnDeleteFromDeckButtonClicked()
        {
            var cardId = m_deckListItemView.CardData.CardId;
            m_deckBuildingManager.DeckListView.RemoveCardFromDeckList(cardId);
            StartHide();

            m_deckBuildingManager.DBCardScrollView.SetDBCardItemViewInteractable(cardId, true);
        }
    }
}