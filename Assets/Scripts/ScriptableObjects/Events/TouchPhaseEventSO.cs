using Minimax.ScriptableObjects.Events.Primitives;
using Minimax.Utilities;
using UnityEngine;
using UnityEngine.Events;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Minimax.ScriptableObjects.Events
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Events/TouchPhaseEvent")]
    public class TouchPhaseEventSO : ScriptableObject
    {
        public TouchPhase Value { get; private set; }

        public UnityEvent<TouchPhase> OnEventRaised;
        
        public void RaiseEvent(TouchPhase value)
        {
            Value = value;
            if (OnEventRaised != null)
                OnEventRaised.Invoke(value);
            else
            {
                DebugWrapper.LogWarning("TouchPhaseEventSO: " + name + " has no listeners!");
            }
        }
    }
}