using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace WMK
{
    public abstract class DataView<T> : MonoBehaviour where T : IEquatable<T>
    {
        [SerializeField] protected Data<T> m_data;
        void OnEnable()
        {
            m_data.OnValueChanged += UpdateView;
            UpdateView(m_data.Value);
        }
        void OnDisable()
        {
            m_data.OnValueChanged -= UpdateView;
        }
        
        protected abstract void InitData(T data);
        protected abstract void UpdateView(T s);
    }
}