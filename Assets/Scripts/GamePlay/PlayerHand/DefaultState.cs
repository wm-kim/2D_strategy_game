using DG.Tweening;
using UnityEngine;

namespace Minimax.GamePlay.PlayerHand
{
    public class DefaultState : HandCardSlotState
    {
        public DefaultState(HandCardSlot slot)
        {
            m_slot = slot;
        }

        public override void Enter()
        {
            SetCardViewParent();
        }

        public override void Exit()
        {
        }

        private void SetCardViewParent()
        {
            m_slot.HandCardView.transform.SetParent(m_slot.transform);
        }

        public override void MoveCardView()
        {
            if (IsAboveThresholdDistance()) TweenCardViewToSlotTransform();
        }

        private bool IsAboveThresholdDistance()
        {
            var targetPosition = m_slot.transform.position;
            return Vector3.Distance(m_slot.HandCardView.transform.position, targetPosition) > m_positionThreshold;
        }

        private void TweenCardViewToSlotTransform()
        {
            var slotTransform  = m_slot.transform;
            var targetPosition = slotTransform.position;
            var targetRotation = slotTransform.eulerAngles;
            var targetScale    = 1f;
            var duration       = m_slot.HandCardSlotSettings.DropDownDuration;

            m_slot.HandCardView.StartMoveTween(targetPosition, duration);
            m_slot.HandCardView.StartRotTween(targetRotation, duration);
            m_slot.HandCardView.StartScaleTween(targetScale, duration);
        }

        public override void OnPointerEnter()
        {
            // if the card is not selected(not dragging), then we can hover it
            if (!m_slot.MyHandInteraction.IsSelecting)
                m_slot.ChangeState(m_slot.HoverState);
        }

        public override void OnPointerExit()
        {
        }

        public override void OnPointerDown()
        {
        }

        public override void OnPointerUp()
        {
        }
    }
}