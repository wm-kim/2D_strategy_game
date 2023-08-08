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
        
        public Action<Vector2, TouchPhase> OnTouch { get; set; }
        public Action OnBackButton { get; set; }

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
                var activeTouch = EnhancedTouch.Touch.activeFingers[0].currentTouch;
                SetTouchImagePosition(activeTouch.screenPosition);
                OnTouch?.Invoke(activeTouch.screenPosition, activeTouch.phase);
            }
            
            // mobile back button
            if (Keyboard.current.escapeKey.wasPressedThisFrame) OnBackButton?.Invoke();
        }
    }
}
