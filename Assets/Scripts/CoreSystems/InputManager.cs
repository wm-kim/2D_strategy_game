using System;
using Minimax.ScriptableObjects.Events;
using Minimax.ScriptableObjects.Events.Primitives;
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
        
        [Header("Broadcasting on")] 
        [SerializeField] private TouchPhaseEventSO m_touchPhaseEvent;
        [SerializeField] private Vector2EventSO m_touchPositionEvent;
        [SerializeField] private VoidEventSO m_mobileBackButtonEvent;

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
            return EnhancedTouch.Touch.activeTouches.Count > 0;
        }
        
        private Vector2 GetTouchPosition(EnhancedTouch.Touch activeTouch)
        {
            // Raise touch position event
            m_touchPositionEvent.RaiseEvent(activeTouch.screenPosition);
            return activeTouch.screenPosition;
        }
        
        private void SetTouchImagePosition(Vector2 position)
        {
            m_touchImage.transform.position = position;
        }

        private void Update()
        {
            if (IsTouching())
            {
                
                var activeTouch = EnhancedTouch.Touch.activeFingers[0].currentTouch;
                SetTouchImagePosition(GetTouchPosition(activeTouch));
                
                // Raise touch phase event
                m_touchPhaseEvent.RaiseEvent(activeTouch.phase);
            }
            
            // mobile back button
            if (Keyboard.current.escapeKey.wasPressedThisFrame) m_mobileBackButtonEvent.RaiseEvent();
            
            
        }
    }
}
