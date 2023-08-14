using UnityEngine;

namespace Minimax.GamePlay.PlayerHand
{
    public abstract class HandCardSlotState
    {
        protected HandCardSlot m_slot;
        
        // this is for optimization, so that we don't have to tween every frame
        protected const float m_positionThreshold = 0.1f;
        
        public abstract void Enter(); // 상태 진입 시 실행되는 메서드
        public abstract void Exit(); // 상태 종료 시 실행되는 메서드
        public abstract void MoveCardView(); // 카드 뷰 이동
        public abstract void OnPointerEnter(); // 마우스가 올라갔을 때 실행되는 메서드
        public abstract void OnPointerExit(); // 마우스가 내려갔을 때 실행되는 메서드
        public abstract void OnPointerDown(); // 마우스를 눌렀을 때 실행되는 메서드
        public abstract void OnPointerUp(); // 마우스를 뗐을 때 실행되는 메서드
    }
}