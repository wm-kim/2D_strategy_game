using System;
using Minimax.ScriptableObjects.Events.Primitives;
using UnityEngine;

namespace Minimax.UI.View.DataViews
{
    /// <summary>
    /// Base class for all DataViews.
    /// Responsible for updating the view when the data changes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DataView<T> : MonoBehaviour where T : IEquatable<T>
    {
        [SerializeField] protected DataEventSO<T> m_data;
   
        private void OnEnable()
        {
            m_data.OnValueChanged.AddListener(UpdateView);
            UpdateView(m_data.Value);
        }
        private void OnDisable()
        {
            m_data.OnValueChanged.RemoveListener(UpdateView);
        }

        protected abstract void UpdateView(T value);
    }
}