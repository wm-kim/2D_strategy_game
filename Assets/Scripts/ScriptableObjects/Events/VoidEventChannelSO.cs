using System;
using UnityEngine;
using UnityEngine.Events;

namespace WMK
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
