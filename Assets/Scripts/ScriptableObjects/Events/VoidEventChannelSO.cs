using UnityEngine;
using UnityEngine.Events;
using Utilities;
using Debug = Utilities.Debug;

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
                Debug.Log("VoidEventSO: " + name + " was raised.");
                OnEventRaised.Invoke();
            }
        }
    }
}