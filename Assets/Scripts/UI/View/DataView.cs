using System;
using UnityEngine;

namespace WMK
{
    // View needs to be bind first before the view model is set or updated
    [DefaultExecutionOrder(-1)]
    public abstract class DataView<T> : MonoBehaviour where T : IEquatable<T>
    {
        [SerializeField] protected Data<T> m_data;

        private void OnEnable()
        {
            m_data.OnValueChanged += UpdateView;
            UpdateView(m_data.Value);
        }
        private void OnDisable()
        {
            m_data.OnValueChanged -= UpdateView;
        }
        
        protected abstract void UpdateView(T s);
    }
}