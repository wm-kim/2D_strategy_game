using DG.Tweening;
using UnityEngine;

namespace Minimax.GamePlay.PlayerHand
{
    public class DefaultState : HandCardSlotState
    {
        public DefaultState(HandCardSlot slot) => m_slot = slot;

        public override void Enter()
        {
            m_slot.CardView.transform.SetParent(m_slot.transform);
        }

        public override void Exit() { }

        public override void MoveCardView()
        {
            Vector3 targetPosition = m_slot.transform.position;

            if (Vector3.Distance(m_slot.CardView.transform.position, targetPosition) > m_positionThreshold)
            {
                Vector3 targetRotation = m_slot.transform.eulerAngles;
                float targetScale = 1f;
                float duration = m_slot.HandCardSlotSettings.DropDownDuration;
                
                m_slot.CardView.KillTweens();
                m_slot.CardView.PosTween = m_slot.CardView.transform.DOMove(targetPosition, duration);
                m_slot.CardView.RotTween = m_slot.CardView.transform.DORotate(targetRotation, duration);
                m_slot.CardView.ScaleTween = m_slot.CardView.transform.DOScale(targetScale, duration);
            }
        }

        public override void OnPointerEnter()
        {
            // if the card is not selected, then we can hover it
            if (m_slot.HandManager.SelectedIndex == -1)
                m_slot.ChangeState(m_slot.HoverState);
        }

        public override void OnPointerExit() { }

        public override void OnPointerDown() { }
        
        public override void OnPointerUp() { }
    }
}