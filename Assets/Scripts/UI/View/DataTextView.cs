using System;
using TMPro;
using UnityEngine;

namespace WMK
{
    [RequireComponent(typeof(TMP_Text))]
    public abstract class DataTextView<T> : DataView<T> where T : IEquatable<T>
    {
        protected TMP_Text m_text;
        
        protected virtual void Awake()
        {
            m_text = GetComponent<TMP_Text>();
        }
    }
}
