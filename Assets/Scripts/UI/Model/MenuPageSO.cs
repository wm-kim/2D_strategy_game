using UnityEngine;
using UnityEngine.Serialization;

namespace WMK
{
    [CreateAssetMenu(menuName = "ScriptableObjects/PageModels/MenuPageSO")]
    public class MenuPageSO : ScriptableObject
    {
        public StringEventSO GameTitle;
        public StringEventSO GameVersion;
    }
}