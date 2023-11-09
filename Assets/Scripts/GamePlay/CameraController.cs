using System;
using Minimax.CoreSystems;
using Minimax.GamePlay.PlayerHand;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Utilities;
using Debug = Utilities.Debug;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Minimax.GamePlay
{
    public class CameraController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Camera m_camera;

        [SerializeField]
        private MyHandInteractionManager m_handInteraction;

        [Header("Settings")]
        [SerializeField]
        [Range(0, 2)]
        private float m_panSpeed = 1f;

        [SerializeField]
        [Tooltip("The time it takes for the camera to reach the target position")]
        private float m_panSmoothTime = 0.1f;

        [SerializeField]
        [Range(1, 2)]
        [Tooltip("The scale of the camera boundary relative to the map size")]
        private float m_bounadryScaleX = 1.2f;

        [SerializeField]
        [Range(1, 2)]
        [Tooltip("The scale of the camera boundary relative to the map size")]
        private float m_bounadryScaleY = 1.5f;

        public Camera Camera => m_camera;

        // This is for optimization purposes
        private float m_moveThreshold = 0.1f;

        private Vector3 m_currentFocusPosition;
        private Vector3 m_targetPosition;
        private Vector2 m_panSmoothVelocity;
        private Rect    m_panBound;

        private void OnEnable()
        {
            GlobalManagers.Instance.Input.OnTouch += MoveCamera;
        }

        private void OnDisable()
        {
            if (GlobalManagers.IsAvailable && GlobalManagers.Instance.Input != null)
                GlobalManagers.Instance.Input.OnTouch -= MoveCamera;
        }

        private void LateUpdate()
        {
            UpdateCameraTransform();
        }

        private void MoveCamera(EnhancedTouch.Touch touch)
        {
            // If the touch is not a move touch, do not proceed with camera movement
            if (touch.phase is not TouchPhase.Moved) return;

            // Check if touch is over a UI element
            if (EventSystem.current.IsPointerOverGameObject(touch.touchId))
                return;

            if (m_handInteraction.IsHovering && !m_handInteraction.IsSelecting) return;

            // If the player is selecting a card, do not proceed with camera movement
            if (m_handInteraction.IsSelecting) return;

            var touchDelta     = touch.delta * (m_camera.orthographicSize * m_panSpeed * 0.001f);
            var newPanPosition = m_targetPosition - new Vector3(touchDelta.x, touchDelta.y);

            // clamp the new pan position to the pan bounds
            newPanPosition.x = Mathf.Clamp(newPanPosition.x, m_panBound.xMin, m_panBound.xMax);
            newPanPosition.y = Mathf.Clamp(newPanPosition.y, m_panBound.yMin, m_panBound.yMax);

            m_targetPosition = newPanPosition;
        }


        public void SetCameraPositionAndBoundary(Vector3 center, Vector2 size)
        {
            Debug.Log($"SetCameraPositionAndBoundary: center: {center}, size: {size}");
            m_camera.transform.position = center;
            m_currentFocusPosition      = center;
            m_targetPosition            = center;
            m_panBound = new Rect(
                (center.x - size.x) * 0.5f * m_bounadryScaleX,
                center.y - size.y * 0.5f * m_bounadryScaleY,
                size.x * m_bounadryScaleX,
                size.y * m_bounadryScaleY);
        }


        private void UpdateCameraTransform()
        {
            // optimization technique
            if (Vector3.Distance(m_currentFocusPosition, m_targetPosition) < m_moveThreshold) return;

            m_currentFocusPosition = Vector2.SmoothDamp(m_currentFocusPosition, m_targetPosition,
                ref m_panSmoothVelocity, m_panSmoothTime);
            m_camera.transform.position = m_currentFocusPosition;
        }
    }
}