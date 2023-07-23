using UnityEngine;
using UnityEngine.Events;

namespace Minimax
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Events/VoidEvent")]
    public class VoidEventSO : ScriptableObject
    {
        public UnityEvent OnEventRaised;

        public void RaiseEvent()
        {
            if (OnEventRaised != null)
                OnEventRaised.Invoke();
        }
    }
}
