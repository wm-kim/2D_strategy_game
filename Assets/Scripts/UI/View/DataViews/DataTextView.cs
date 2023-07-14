using System;
using TMPro;
using UnityEngine;

namespace WMK
{
    // Setting initial value of data by text component
    // before any other component tries to access it
    [DefaultExecutionOrder(-1), RequireComponent(typeof(TMP_Text))]
    public abstract class DataTextView<T> : DataView<T> where T : IEquatable<T>
    {
        protected TMP_Text m_text;
        
        protected virtual void Awake()
        {
            m_text = GetComponent<TMP_Text>();
            SetInitialValue(m_text.text);
        }
        
        protected abstract void SetInitialValue(string value);
    }
}
