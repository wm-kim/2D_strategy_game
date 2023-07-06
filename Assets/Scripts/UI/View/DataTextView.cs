using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace WMK
{
    [RequireComponent(typeof(TMP_Text))]
    public abstract class DataTextView<T> : DataView<T> where T : IEquatable<T>
    {
        protected TMP_Text m_text;

        protected void Bind()
        {
            m_text = GetComponent<TMP_Text>();
        }
        
        protected override void UpdateView(T data)
        {
            if (m_text == null) Bind();
            m_text.text = data.ToString();
        }
    }
}
