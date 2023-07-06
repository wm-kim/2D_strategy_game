using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WMK
{
    public class DataTextViewString : DataTextView<string>
    {
        private void Awake()
        {
            Bind();
            InitData(m_text.text);
        }
        
        protected override void InitData(string data) => m_data.Value = data;
    }
}
