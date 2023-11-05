using UnityEngine;

namespace Minimax.ScriptableObjects.Events.DataEvents
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