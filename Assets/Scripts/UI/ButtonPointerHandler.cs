using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Minimax.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonPointerHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public UnityEvent onPointerDown;
        public UnityEvent onPointerUp;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // ignore if button not interactable
            if (!_button.interactable) return;

            onPointerDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onPointerUp?.Invoke();
        }
    }
}