using System;
using UnityEngine;
using UnityEngine.Events;
using Utilities;
using Debug = Utilities.Debug;

namespace Minimax.ScriptableObjects.Events.DataEvents
{
    public abstract class DataEventSO<T> : ScriptableObject where T : IEquatable<T>
    {
        private T m_value;

        // 값이 바뀔 때마다 호출되는 이벤트
        public UnityEvent<T> OnValueChanged;

        // 값이 바뀌던 말던 호출되는 이벤트
        public UnityEvent<T> OnEventRaised;

        public virtual T Value
        {
            get => m_value;
            set
            {
                if (m_value != null && m_value.Equals(value)) return;
                m_value = value;
                OnValueChanged?.Invoke(value);
            }
        }

        public void RaiseEvent(T value)
        {
            Value = value;
            if (OnEventRaised != null) OnEventRaised.Invoke(value);
            else
                Debug.LogWarning($"DataEventSO: " + name + " has no listeners!");
        }
    }
}