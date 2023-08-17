using System;
using Minimax.ScriptableObjects.Events;
using Minimax.ScriptableObjects.Events.Primitives;
using Minimax.Utilities;
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

        [SerializeField, Range(0f, 1f)] 
        [Tooltip("The time between touches to register a double touch")]
        private float m_doubleTouchDelay  = 0.5f;
        
        [SerializeField, Range(0f, 3f)]
        [Tooltip("Duration for touch to be considered as long touch")]
        private float m_longTouchThreshold = 1f;
        
        public Action<EnhancedTouch.Touch> OnTouch { get; set; }
        public Action<Vector2> OnDoubleTouch { get; set; }
        public Action<Vector2> OnLongTouch { get; set; }
        public Action OnBackButton { get; set; }
        
        private float m_lastTouchTime;
        private float m_touchStartTime;
        private bool m_longTouchTriggered;

        private void OnEnable()
        {
            EnhancedTouch.EnhancedTouchSupport.Enable();
        }
        
        private void OnDisable()
        {
            EnhancedTouch.EnhancedTouchSupport.Disable();
        }
        
        private bool IsTouching()
        {
            return EnhancedTouch.Touch.activeFingers.Count > 0;
        }
        
        private void SetTouchImagePosition(Vector2 position)
        {
            m_touchImage.transform.position = position;
        }

        private void Update()
        {
            if (IsTouching())
            {
                var activeTouch = EnhancedTouch.Touch.activeTouches[0];
                
                switch (activeTouch.phase)
                {
                    case TouchPhase.Began:
                        m_touchStartTime = Time.time;
                        m_longTouchTriggered = false;
                        
                        if (Time.time - m_lastTouchTime < m_doubleTouchDelay)
                        {
                            // Double touch Detection, prioritize double touch over single touch
                            OnDoubleTouch?.Invoke(activeTouch.screenPosition);
                            // for preventing consecutive double touches
                            m_lastTouchTime = 0f;
                        }
                        break;
                    case TouchPhase.Ended:
                        m_lastTouchTime = Time.time;
                        if (activeTouch.isTap) 
                        { 
                            // Single Tap Detection
                        }
                        break;
                }
                
                // Check for long touch outside of the switch-case
                if (!m_longTouchTriggered && activeTouch.phase is TouchPhase.Stationary or TouchPhase.Moved) 
                {
                    if (Time.time - m_touchStartTime > m_longTouchThreshold) 
                    {
                        OnLongTouch?.Invoke(activeTouch.screenPosition);
                        // prevent multiple invocations
                        m_longTouchTriggered = true; 
                    }
                }
                
                SetTouchImagePosition(activeTouch.screenPosition);
                OnTouch?.Invoke(activeTouch);
            }
            
            // mobile back button
            if (Keyboard.current.escapeKey.wasPressedThisFrame) OnBackButton?.Invoke();
        }
    }
}
