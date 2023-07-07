using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace WMK
{
    [DefaultExecutionOrder(-1)] // binding to a TMP_Text before data is set
    [RequireComponent(typeof(TMP_Text))]
    public abstract class DataTextView<T> : DataView<T> where T : IEquatable<T>
    {
        protected TMP_Text m_text;

        protected void Awake()
        {
            m_text = GetComponent<TMP_Text>();
        }
        
        protected override void UpdateView(T data)
        {
            m_text.text = data.ToString();
        }
    }
}
