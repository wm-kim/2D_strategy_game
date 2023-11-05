using UnityEngine;
using UnityEngine.UI;

namespace Minimax.UI.View.ComponentViews
{
    public abstract class ButtonView : MonoBehaviour
    {
        [SerializeField] protected float  m_duration;
        public abstract            Button Button { get; }

        public abstract void SetVisualActive(bool active, bool isImmediate = false);
    }
}