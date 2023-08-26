using System;
using DG.Tweening;
using Minimax.CoreSystems;
using Minimax.GamePlay.PlayerHand;
using Minimax.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Minimax
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private ClientPlayerHandManager m_clientPlayerHandManager;
        [SerializeField, Range(0, 2)] private float m_panSpeed = 1f;
        [SerializeField] private RectTransform m_panBounds;
        
        [SerializeField, Tooltip("The time it takes for the camera to reach the target position")] 
        private float m_panSmoothTime = 0.1f;
        private Camera m_camera;
        
        // This is for optimization purposes
        private float m_moveThreshold = 0.1f;
        
        private Vector3  m_currentFocusPosition;
        private Vector3  m_targetPosition;
        private Vector2 m_panSmoothVelocity;
        
        private void OnEnable()
        {
            GlobalManagers.Instance.Input.OnTouch += MoveCamera;
        }

        private void OnDisable()
        {
            GlobalManagers.Instance.Input.OnTouch -= MoveCamera;
        }
        
        void Start()
        {
            m_camera = Camera.main;
            
            m_currentFocusPosition = m_camera.transform.position;
            m_targetPosition = m_currentFocusPosition;
        }

        private void LateUpdate()
        {
            UpdateCameraTransform();
        }

        private void MoveCamera(EnhancedTouch.Touch touch)
        {
            // If the touch is not a move touch, do not proceed with camera movement
            if (touch.phase is not TouchPhase.Moved)
            {
                return;
            }
            
            // Check if touch is over a UI element
            if (EventSystem.current.IsPointerOverGameObject(touch.touchId))
            {
                // If the touch is over UI, do not proceed with camera movement
                return; 
            }
            
            if (m_clientPlayerHandManager.IsHovering && !m_clientPlayerHandManager.IsSelecting)
            {
                return;
            }
            
            // If the player is selecting a card, do not proceed with camera movement
            if (m_clientPlayerHandManager.IsSelecting)
            {
                return;
            }

            var touchDelta = touch.delta * (m_panSpeed);
            var newPanPosition = m_targetPosition - new Vector3(touchDelta.x, touchDelta.y);
            
            // clamp the new pan position to the pan bounds
            var rect = m_panBounds.rect;
            newPanPosition.x = Mathf.Clamp(newPanPosition.x, rect.xMin, rect.xMax);
            newPanPosition.y = Mathf.Clamp(newPanPosition.y, rect.yMin, rect.yMax);
            
            m_targetPosition = newPanPosition;
        }
        
        private void UpdateCameraTransform()
        {
            // optimization technique
            if(Vector3.Distance(m_currentFocusPosition, m_targetPosition) < m_moveThreshold) return;

            m_currentFocusPosition = Vector2.SmoothDamp(m_currentFocusPosition, m_targetPosition, ref m_panSmoothVelocity, m_panSmoothTime);
            m_camera.transform.position = m_currentFocusPosition;
        }
    }
}
