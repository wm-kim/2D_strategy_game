using System;
using UnityEngine;

namespace WMK
{
    public abstract class Data<T> : ScriptableObject where T : IEquatable<T>
    {
        private T m_value;
        public event Action<T> OnValueChanged;
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
    }
}

