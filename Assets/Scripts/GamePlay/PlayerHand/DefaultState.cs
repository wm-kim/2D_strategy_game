using DG.Tweening;
using Minimax.Utilities;
using UnityEngine;

namespace Minimax.GamePlay.PlayerHand
{
    public class DefaultState : HandCardSlotState
    {
        public DefaultState(HandCardSlot slot) => m_slot = slot;

        public override void Enter()
        {
            m_slot.HandManager.ReleaseSelectingCard();
            m_slot.HandCardView.transform.SetParent(m_slot.transform);
        }

        public override void Exit() { }

        public override void MoveCardView()
        {
            Vector3 targetPosition = m_slot.transform.position;

            if (Vector3.Distance(m_slot.HandCardView.transform.position, targetPosition) > m_positionThreshold)
            {
                Vector3 targetRotation = m_slot.transform.eulerAngles;
                float targetScale = 1f;
                float duration = m_slot.HandCardSlotSettings.DropDownDuration;
                
                m_slot.HandCardView.KillTweens();
                m_slot.HandCardView.PosTween = m_slot.HandCardView.transform.DOMove(targetPosition, duration);
                m_slot.HandCardView.RotTween = m_slot.HandCardView.transform.DORotate(targetRotation, duration);
                m_slot.HandCardView.ScaleTween = m_slot.HandCardView.transform.DOScale(targetScale, duration);
            }
        }

        public override void OnPointerEnter()
        {
            // if the card is not selected(not dragging), then we can hover it
            if (!m_slot.HandManager.IsSelecting)
                m_slot.ChangeState(m_slot.HoverState);
        }

        public override void OnPointerExit() { }

        public override void OnPointerDown()
        {
        }
        
        public override void OnPointerUp() { }
    }
}