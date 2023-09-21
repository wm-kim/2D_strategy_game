using DG.Tweening;
using Minimax.CoreSystems;
using Minimax.Utilities;
using UnityEngine;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Minimax.GamePlay.PlayerHand
{
    public class HoverState : HandCardSlotState
    {
        /// <summary>
        /// Represents the number of touches or contacts that have been made on the slot
        /// </summary>
        private int m_contactCount = 0;

        // caching variables
        private Camera m_camera;
        private float m_frustumSize = 0;
        private float m_zdepth = 0;

        public HoverState(HandCardSlot slot)
        {
            m_slot = slot;
        } 
        
        public override void Enter()
        {
            // Reset the first touch flag
            m_contactCount = 0;
            CalculateFrustum();
            
            // Subscribe to the input events
            var inputManager = GlobalManagers.Instance.Input;
            inputManager.OnTouch += CheckForSecondTouch;
            
            // If there is a card currently being hovered, then we need to stop hovering it
            m_slot.HandManager.HoverOffHoveringCard();
            m_slot.HandManager.HoverCard(m_slot.Index);

            // Set the card view as the last sibling in its parent to render it on top of the other cards
            m_slot.HandCardView.transform.SetParent(m_slot.HandManager.CardParent);
        }

        public override void Exit()
        {
            // Unsubscribe from the input events
            var inputManager = GlobalManagers.Instance.Input;
            inputManager.OnTouch -= CheckForSecondTouch;
            
            // Reset the hovering index
            m_slot.HandManager.UnHoverCard();
        }

        private void CheckForSecondTouch(EnhancedTouch.Touch touch)
        {
            var cardViewRect = m_slot.HandCardView.GetComponent<RectTransform>();
            var camera = m_slot.HandManager.Canvas.worldCamera;
            bool isInsideCardDisplayMenu =
                RectTransformUtility.RectangleContainsScreenPoint(cardViewRect, touch.screenPosition,
                    camera);

            if (!isInsideCardDisplayMenu)
            {
                m_slot.ChangeState(m_slot.DefaultState);
                return;
            }

            var currentSection = GlobalManagers.Instance.ServiceLocator.GetService<SectionDivider>().CurrentSection;
            
            bool isTouchBegan = touch.phase == TouchPhase.Began;
            bool isTouchMoved = touch.phase == TouchPhase.Moved;
            bool isTouchEnded = touch.phase == TouchPhase.Ended;
            bool isMyHandSection = currentSection == SectionDivider.Section.MyHand;
            bool isMyTurn = TurnManager.Instance.IsMyTurn;
            
            if (isTouchBegan || isTouchEnded)
            {
                m_contactCount++;
                
                if (m_contactCount >= 2 && isMyTurn)
                {
                    m_slot.ChangeState(m_slot.DraggingState);
                }
            }
            else if (isTouchMoved && !isMyHandSection && isMyTurn)
            {
                m_slot.ChangeState(m_slot.DraggingState);
            }
        }
        
        /// <summary>
        /// Calculates the clipping size of the canvas
        /// </summary>
        private void CalculateFrustum()
        {
            if (m_camera == null)
            {
                var canvas = m_slot.HandManager.Canvas;
                m_zdepth = canvas.transform.position.z;
                m_camera = canvas.worldCamera;
                var planeDistance =  canvas.planeDistance;
                m_frustumSize = m_camera.CalculateFrustumSize(planeDistance).y * 0.5f;
            }
        }
        
        public override void MoveCardView()
        {
            var cameraBottomY = m_camera.transform.position.y - m_frustumSize;
            Vector3 targetPosition = new Vector3(m_slot.transform.position.x, cameraBottomY, m_zdepth) +
                                     (Vector3)(m_slot.HandCardSlotSettings.HoverOffset * m_frustumSize);
                                     
            if (Vector3.Distance(m_slot.HandCardView.transform.position, targetPosition) > m_positionThreshold)
            {
                Vector3 targetRotation = Vector3.zero;
                float targetScale = m_slot.HandCardSlotSettings.HoverScale;
                float duration = m_slot.HandCardSlotSettings.HoverDuration;

                m_slot.HandCardView.KillTweens();
                m_slot.HandCardView.PosTween = m_slot.HandCardView.transform.DOMove(targetPosition, duration);
                m_slot.HandCardView.RotTween = m_slot.HandCardView.transform.DORotate(targetRotation, duration);
                m_slot.HandCardView.ScaleTween = m_slot.HandCardView.transform.DOScale(targetScale, duration);
            }
        }

        public override void OnPointerEnter()
        {
        }

        public override void OnPointerExit()
        {
            
        }

        public override void OnPointerDown()
        {
            // This should be empty as we're now controlling the state transition with the CheckForSecondTouch method
        }

        public override void OnPointerUp()
        {
        }
    }
}