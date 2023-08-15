using UnityEngine;

namespace Minimax.GamePlay.PlayerHand
{
    public abstract class HandCardSlotState
    {
        protected HandCardSlot m_slot;
        
        // this is for optimization, so that we don't have to tween every frame
        protected const float m_positionThreshold = 0.1f;
        
        /// <summary>
        /// 상태 진입 시 실행되는 메서드
        /// </summary>
        public abstract void Enter();
        
        /// <summary>
        /// 상태 종료 시 실행되는 메서드
        /// </summary>
        public abstract void Exit();
        
        /// <summary>
        /// 카드 뷰 이동
        /// </summary>
        public abstract void MoveCardView();
        
        /// <summary>
        /// 마우스가 올라갔을 때 실행되는 메서드
        /// OnPointerDown 보다 항상 먼저 실행됨
        /// </summary>
        public abstract void OnPointerEnter();
        
        /// <summary>
        /// 마우스가 내려갔을 때 실행되는 메서드
        /// </summary>
        public abstract void OnPointerExit();
        
        /// <summary>
        /// 마우스를 눌렀을 때 실행되는 메서드
        /// </summary>
        public abstract void OnPointerDown();
        
        /// <summary>
        /// 마우스를 뗐을 때 실행되는 메서드
        /// </summary>
        public abstract void OnPointerUp();
    }
}