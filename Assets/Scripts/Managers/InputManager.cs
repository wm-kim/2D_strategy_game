using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

namespace WMK
{
    [DefaultExecutionOrder(-1)]
    public class InputManager : MonoBehaviour
    {
        public Action<Vector2, float> OnTouchStarted;
        public Action<Vector2, float> OnTouchEnded;
        
        private TouchControls m_touchControls;
        private void Awake()
        {
            m_touchControls = new TouchControls();
        }
        
        private void OnEnable() 
        {
            m_touchControls.Enable();
            TouchSimulation.Enable();
            UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += FingerDown;

        }
        
        private void OnDisable() 
        {
            m_touchControls.Disable();
            TouchSimulation.Disable();
            UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= FingerDown;
        }

        private void Start()
        {
            m_touchControls.Touch.TouchPress.started += ctx => StartTouch(ctx);
            m_touchControls.Touch.TouchPress.canceled += ctx => EndTouch(ctx);

        }
        
        private void StartTouch(InputAction.CallbackContext ctx)
        {
            Debug.Log("Touch started" + m_touchControls.Touch.TouchPosition.ReadValue<Vector2>());
            OnTouchStarted?.Invoke(m_touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)ctx.startTime);
        }  
        
        private void EndTouch(InputAction.CallbackContext ctx)
        {
            Debug.Log("Touch ended" + m_touchControls.Touch.TouchPosition.ReadValue<Vector2>());
            OnTouchEnded?.Invoke(m_touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)ctx.time);
        }
        
        private void FingerDown(Finger finger)
        {
            OnTouchStarted?.Invoke(finger.screenPosition, (float)Time.time);
        }

        private void Update()
        {
            Debug.Log(UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches);
            foreach (var touch in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches)
            {
                Debug.Log(touch.phase == UnityEngine.InputSystem.TouchPhase.Began);
            }
        }
    }
}
