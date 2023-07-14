using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WMK
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Events/Primitives/BoolEvent")]
    public class BoolEventSO : DataEventSO<bool>
    {
        public void Toggle()
        {
            Value = !Value;
        }
    }
}
