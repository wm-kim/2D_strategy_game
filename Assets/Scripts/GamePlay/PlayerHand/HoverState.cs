using DG.Tweening;
using Minimax.Utilities;
using UnityEngine;

namespace Minimax.GamePlay.PlayerHand
{
    public class HoverState : HandCardSlotState
    {
        private ClientPlayerHandManager m_handManager;

        public HoverState(HandCardSlot slot) => m_slot = slot;

        public override void Enter()
        {
            m_slot.HandManager.HoverOffHoveringCard();
            m_slot.HandManager.HoveringIndex = m_slot.Index;

            // Set the card view as the last sibling in its parent to render it on top of the other cards
            m_slot.CardView.transform.SetParent(m_slot.HandManager.transform);
        }

        public override void Exit()
        {
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

        public override void OnPointerEnter() { }

        public override void OnPointerExit() { }

        public override void OnPointerDown()
        {
            m_slot.ChangeState(m_slot.DraggingState);
        }
        
        public override void OnPointerUp() { }
    }
}