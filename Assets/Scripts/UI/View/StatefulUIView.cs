using Minimax.PropertyDrawer;
using UnityEngine;

namespace Minimax.UI.View
{
    public abstract class StatefulUIView : MonoBehaviour
    {
        [SerializeField] [ReadOnly] protected UIVisibleState m_currentState = UIVisibleState.Undefined;

        public UIVisibleState CurrentState => m_currentState;

        public virtual void StartShow(float transitionDuration = 0.0f)
        {
            if (IsAppearingOrAppeared()) return;
            m_currentState = UIVisibleState.Appearing;
            Show(transitionDuration);
        }

        public virtual void StartHide(float transitionDuration = 0.0f)
        {
            if (IsDisappearingOrDisappeared()) return;
            m_currentState = UIVisibleState.Disappearing;
            Hide(transitionDuration);
        }

        // Derived classes should override these methods for custom show/hide logic
        /// <summary>
        /// Show the view. Derived classes should override this method for custom show logic.
        /// </summary>
        protected abstract void Show(float transitionDuration = 0.0f);

        /// <summary>
        /// Hide the view. Derived classes should override this method for custom hide logic.
        /// </summary>
        protected abstract void Hide(float transitionDuration = 0.0f);

        // These methods are for derived classes to call when their show/hide processes are complete
        protected void SetAppearedState()
        {
            m_currentState = UIVisibleState.Appeared;
        }

        protected void SetDisappearedState()
        {
            m_currentState = UIVisibleState.Disappeared;
        }

        // Helper methods for checking state
        private bool IsAppearingOrAppeared()
        {
            return m_currentState == UIVisibleState.Appearing || m_currentState == UIVisibleState.Appeared;
        }

        private bool IsDisappearingOrDisappeared()
        {
            return m_currentState == UIVisibleState.Disappearing || m_currentState == UIVisibleState.Disappeared;
        }
    }
}