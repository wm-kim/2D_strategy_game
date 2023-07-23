using Minimax.ScriptableObjects.Events.Primitives;
using UnityEngine;

namespace Minimax.UI.Model
{
    [CreateAssetMenu(menuName = "ScriptableObjects/PageModels/MenuPageSO")]
    public class MenuPageSO : ScriptableObject
    {
        public StringEventSO GameTitle;
        public StringEventSO GameVersion;
    }
}