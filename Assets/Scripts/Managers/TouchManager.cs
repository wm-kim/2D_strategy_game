using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WMK
{
    public class TouchManager : MonoBehaviour
    {
        private PlayerInput m_playerInput;

        private InputAction m_touchPositionAction;
        private InputAction m_touchPressAction;

        private void Awake()
        {
            m_playerInput = GetComponent<PlayerInput>();
            m_touchPositionAction = m_playerInput.actions["TouchPosition"];
            m_touchPressAction = m_playerInput.actions["TouchPress"];
        }

        private void OnEnable()
        {
            m_touchPositionAction.performed += TouchPressed;
        }
        
        private void OnDisable()
        {
            m_touchPositionAction.performed -= TouchPressed;
        }
        
        private void TouchPressed(InputAction.CallbackContext context)
        {
            
        }
    }
}
