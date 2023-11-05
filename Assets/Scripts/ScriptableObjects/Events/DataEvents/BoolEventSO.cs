using UnityEngine;

namespace Minimax.ScriptableObjects.Events.Primitives
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