using DG.Tweening;
using Minimax.CoreSystems;
using Minimax.Utilities;
using UnityEngine;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Minimax.GamePlay.PlayerHand
{
    public class HoverState : HandCardSlotState
    {
        /// <summary>
        /// Represents the number of touches or contacts that have been made on the slot
        /// </summary>
        private int m_contactCount = 0;

        public HoverState(HandCardSlot slot)
        {
            m_slot = slot;
        } 
        public override void Enter()
        {
            // Reset the first touch flag
            m_contactCount = 0;
            
            // Subscribe to the input events
            var inputManager = GlobalManagers.Instance.Input;
            inputManager.OnTouch += CheckForSecondTouch;
            
            // If there is a card currently being hovered, then we need to stop hovering it
            m_slot.HandManager.HoverOffHoveringCard();
            m_slot.HandManager.HoveringIndex = m_slot.Index;

            // Set the card view as the last sibling in its parent to render it on top of the other cards
            m_slot.CardView.transform.SetParent(m_slot.HandManager.transform);
        }

        public override void Exit()
        {
            // Unsubscribe from the input events
            var inputManager = GlobalManagers.Instance.Input; 
            inputManager.OnTouch -= CheckForSecondTouch;
            
            // Reset the hovering index
            m_slot.HandManager.HoveringIndex = -1;
        }

        private void CheckForSecondTouch(Vector2 touchPosition, TouchPhase touchPhase)
        {
            DebugWrapper.Log($"Contact count: {m_contactCount}");
            bool isTouchBegan = touchPhase == TouchPhase.Began;
            var cardViewRect = m_slot.CardView.GetComponent<RectTransform>();
            bool isInsideCardDisplayMenu = RectTransformUtility.RectangleContainsScreenPoint(cardViewRect, touchPosition);
            if (isTouchBegan && isInsideCardDisplayMenu)
            {
                if (m_contactCount >= 1)
                {
                    m_slot.ChangeState(m_slot.DraggingState);
                }
                else
                {
                    m_contactCount++;
                }
            }
            if (touchPhase == TouchPhase.Ended)
            {
                m_contactCount++;
            }
        }
        
        public override void MoveCardView()
        {
            Vector3 targetPosition = new Vector3(m_slot.transform.position.x, 0, 0) +
                                     (Vector3)m_slot.HandCardSlotSettings.HoverOffset;

            if (Vector3.Distance(m_slot.CardView.transform.position, targetPosition) > m_positionThreshold)
            {
                Vector3 targetRotation = Vector3.zero;
                float targetScale = m_slot.HandCardSlotSettings.HoverScale;
                float duration = m_slot.HandCardSlotSettings.HoverDuration;

                m_slot.CardView.KillTweens();
                m_slot.CardView.PosTween = m_slot.CardView.transform.DOMove(targetPosition, duration);
                m_slot.CardView.RotTween = m_slot.CardView.transform.DORotate(targetRotation, duration);
                m_slot.CardView.ScaleTween = m_slot.CardView.transform.DOScale(targetScale, duration);
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