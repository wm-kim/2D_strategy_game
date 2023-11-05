using UnityEngine;
using UnityEngine.Events;
using Utilities;

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
                DebugWrapper.Log("VoidEventSO: " + name + " was raised.");
                OnEventRaised.Invoke();
            }
        }
    }
}