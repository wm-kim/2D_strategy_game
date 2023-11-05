using Minimax.CoreSystems;
using UnityEngine;
using UnityEngine.Events;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Minimax.UI
{
    public class TouchOutsideRectTransform : MonoBehaviour
    {
        [SerializeField] private Camera        m_camera;
        [SerializeField] private RectTransform m_targetRectTransform;
        public                   UnityEvent    OnClickOutside;

        private void OnEnable()
        {
            GlobalManagers.Instance.Input.OnTouch += OnTouch;
        }

        private void OnDisable()
        {
            GlobalManagers.Instance.Input.OnTouch -= OnTouch;
        }

        private void OnTouch(EnhancedTouch.Touch touch)
        {
            if (touch.phase == TouchPhase.Began)
            {
                var touchPosition = touch.screenPosition;
                if (!RectTransformUtility.RectangleContainsScreenPoint(m_targetRectTransform, touchPosition, m_camera))
                    OnClickOutside?.Invoke();
            }
        }
    }
}