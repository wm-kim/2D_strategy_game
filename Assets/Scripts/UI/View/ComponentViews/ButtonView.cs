using UnityEngine;
using UnityEngine.UI;

namespace Minimax.UI.View.ComponentViews
{
    [RequireComponent(typeof(Button))]
    public abstract class ButtonView : MonoBehaviour
    {
        [SerializeField]
        protected float m_duration;

        public Button Button
        {
            get => m_button ??= GetComponent<Button>();
            protected set => m_button = value;
        }

        private Button m_button = null;

        public abstract void SetVisualActive(bool active, bool isImmediate = false);
    }
}