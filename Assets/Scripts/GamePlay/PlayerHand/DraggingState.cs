using DG.Tweening;
using Minimax.CoreSystems;
using UnityEngine;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Minimax.GamePlay.PlayerHand
{
    public class DraggingState : HandCardSlotState
    {
        public DraggingState(HandCardSlot slot) => m_slot = slot;
        
        public override void Enter()
        {
            m_slot.HandManager.SelectCard(m_slot.Index);
            GlobalManagers.Instance.Input.OnTouch += MoveCardViewToTouchPosition;
        }

        public override void Exit()
        {
            GlobalManagers.Instance.Input.OnTouch -= MoveCardViewToTouchPosition;
        }
        
        private void MoveCardViewToTouchPosition(EnhancedTouch.Touch touch)
        {
            if (touch.phase is TouchPhase.Moved or TouchPhase.Began or TouchPhase.Stationary)
            {
                Vector3 targetPosition = new Vector3(touch.screenPosition.x, touch.screenPosition.y, 0) 
                                         + (Vector3) m_slot.HandCardSlotSettings.DraggingOffset;

                if (Vector3.Distance(m_slot.CardView.transform.position, targetPosition) > m_positionThreshold)
                {
                    m_slot.CardView.KillTweens();
                    
                    m_slot.CardView.transform.position = 
                        Vector3.Lerp(m_slot.CardView.transform.position, targetPosition, 
                            Time.deltaTime * m_slot.HandCardSlotSettings.DraggingSpeed);

                    m_slot.CardView.RotTween = m_slot.CardView.transform.DORotate(Vector3.zero,
                        m_slot.HandCardSlotSettings.DraggingTweenDuration);
                    
                    m_slot.CardView.ScaleTween = m_slot.CardView.transform.DOScale(1f,
                        m_slot.HandCardSlotSettings.DraggingTweenDuration);
                }
            }
            if (touch.phase == TouchPhase.Ended)
            {
                m_slot.HandManager.DeselectCard();
                m_slot.ChangeState(m_slot.DefaultState);
            }
        }

        public override void MoveCardView()
        {
            // This can be left empty if you're controlling the card view via the OnTouch event
        }

        public override void OnPointerEnter()
        {
        }

        public override void OnPointerExit()
        {
            
        }

        public override void OnPointerDown()
        {
        }
        
        public override void OnPointerUp()
        {
            m_slot.HandManager.DeselectCard();
            m_slot.ChangeState(m_slot.DefaultState);
        }
    }
}