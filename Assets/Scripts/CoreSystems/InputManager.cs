using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Minimax.CoreSystems
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private Image m_touchImage;

        [SerializeField] [Range(0f, 1f)] private float m_doubleTouchDelay   = 0.5f;
        [SerializeField] [Range(0f, 3f)] private float m_longTouchThreshold = 1f;

        // 이벤트를 public으로 열고 event 키워드를 사용합니다.
        public event Action<EnhancedTouch.Touch> OnTouch;
        public event Action<Vector2>             OnDoubleTouch;
        public event Action<Vector2>             OnLongTouch;
        public event Action<Vector2>             OnTap;
        public event Action                      OnBackButton;

        private float m_lastTouchTime;
        private float m_touchStartTime;
        private bool  m_longTouchTriggered;

        // 터치를 지원하도록 하는 메소드
        private void OnEnable()
        {
            EnhancedTouch.EnhancedTouchSupport.Enable();
        }

        // 터치 지원을 끄는 메소드
        private void OnDisable()
        {
            EnhancedTouch.EnhancedTouchSupport.Disable();
        }

        // 터치가 있는지 확인
        private bool IsTouching()
        {
            return EnhancedTouch.Touch.activeFingers.Count > 0;
        }

        // 터치 이미지 위치 설정
        private void SetTouchImagePosition(Vector2 position)
        {
            m_touchImage.transform.position = position;
        }

        private void Update()
        {
            if (IsTouching())
            {
                var activeTouch = EnhancedTouch.Touch.activeTouches[0];
                HandleTouch(activeTouch);
            }

            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                OnBackButton?.Invoke();
        }

        // 터치를 처리하는 별도의 메소드
        private void HandleTouch(EnhancedTouch.Touch activeTouch)
        {
            switch (activeTouch.phase)
            {
                case TouchPhase.Began:
                    OnTouchBegan(activeTouch);
                    break;
                case TouchPhase.Ended:
                    OnTouchEnded(activeTouch);
                    break;
            }

            // 롱 터치 체크
            CheckForLongTouch(activeTouch);

            // 터치 이미지 위치 설정
            SetTouchImagePosition(activeTouch.screenPosition);

            // 일반 터치 이벤트 발생
            OnTouch?.Invoke(activeTouch);
        }

        private void OnTouchBegan(EnhancedTouch.Touch activeTouch)
        {
            // 더블 탭 검출을 위한 시작 시간 설정
            m_touchStartTime     = Time.time;
            m_longTouchTriggered = false;

            if (Time.time - m_lastTouchTime < m_doubleTouchDelay)
            {
                OnDoubleTouch?.Invoke(activeTouch.screenPosition);
                // 연속 더블 탭 방지
                m_lastTouchTime = 0f;
            }
        }

        private void OnTouchEnded(EnhancedTouch.Touch activeTouch)
        {
            m_lastTouchTime = Time.time;
            if (activeTouch.isTap) OnTap?.Invoke(activeTouch.screenPosition);
        }

        // 롱 터치를 체크하는 메소드
        private void CheckForLongTouch(EnhancedTouch.Touch activeTouch)
        {
            if (!m_longTouchTriggered &&
                (activeTouch.phase == TouchPhase.Stationary || activeTouch.phase == TouchPhase.Moved))
                if (Time.time - m_touchStartTime > m_longTouchThreshold)
                {
                    OnLongTouch?.Invoke(activeTouch.screenPosition);
                    // 멀티 롱 터치 발생 방지
                    m_longTouchTriggered = true;
                }
        }
    }
}