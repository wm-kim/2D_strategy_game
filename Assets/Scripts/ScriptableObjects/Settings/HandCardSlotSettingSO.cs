using UnityEngine;
using UnityEngine.Serialization;

namespace Minimax.ScriptableObjects.Settings
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings/HandCardSlotSettingSO")]
    public class HandCardSlotSettingSO : ScriptableObject
    {
        public Vector2 HoverOffset = new Vector2(0, 50f);
        public float HoverScale = 1.2f;
        public float HoverDuration = 0.1f;
        public float DropDownDuration = 0.1f;
        public Vector2 DraggingOffset = new Vector2(0, -50f);
        public float DraggingSpeed = 10f;
        public float DraggingTweenDuration = 0.1f;
        
    }
}