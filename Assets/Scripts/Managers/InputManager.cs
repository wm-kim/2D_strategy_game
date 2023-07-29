using Minimax.ScriptableObjects.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

namespace Minimax.Managers
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private Image m_touchImage;

        [Header("Broadcasting on")] 
        [SerializeField] private VoidEventSO m_mobileBackButtonEvent;

        private void OnEnable()
        {
            EnhancedTouch.EnhancedTouchSupport.Enable();
        }
        
        private void OnDisable()
        {
            EnhancedTouch.EnhancedTouchSupport.Disable();
        }

        private void Update()
        {
            if (EnhancedTouch.Touch.activeTouches.Count == 1)
            {
                EnhancedTouch.Touch activeTouch = EnhancedTouch.Touch.activeFingers[0].currentTouch;
                m_touchImage.transform.position = activeTouch.screenPosition;
                // Debug.Log($"Phase: {activeTouch.phase} | Position: {activeTouch.startScreenPosition}");
            }
            
            // mobile back button
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                m_mobileBackButtonEvent?.RaiseEvent();
            }
        }
    }
}
