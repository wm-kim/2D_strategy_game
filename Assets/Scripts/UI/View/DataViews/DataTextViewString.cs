using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WMK
{
    public class DataTextViewString : DataTextView<string>
    {
        protected override void UpdateView(string value) => m_text.text = value;

        protected override void SetInitialValue(string value) => m_data.Value = value;

    }
}
