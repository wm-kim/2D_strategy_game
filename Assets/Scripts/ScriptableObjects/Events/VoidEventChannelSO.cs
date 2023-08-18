using Minimax.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Minimax.ScriptableObjects.Events
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Events/VoidEvent")]
    public class VoidEventSO : ScriptableObject
    {
        public UnityEvent OnEventRaised;

        public void RaiseEvent()
        {
            if (OnEventRaised != null)
            {
                DebugWrapper.Instance.Log("VoidEventSO: " + name + " was raised.");
                OnEventRaised.Invoke();
            }
        }
    }
}
