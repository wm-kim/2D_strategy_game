using UnityEngine;
using UnityEngine.Serialization;

namespace Minimax.ScriptableObjects.Settings
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings/HandCardSlotSettingSO")]
    public class HandCardSlotSettingSO : ScriptableObject
    {
        // These values are relative to canvas size. (camara orthographic size = 1)
        public Vector2 HoverOffset    = new(0, 0.18f);
        public Vector2 DraggingOffset = new(0, 0f);

        public float DraggingSpeed         = 0.01f;
        public float HoverScale            = 1.2f;
        public float HoverDuration         = 0.1f;
        public float DropDownDuration      = 0.1f;
        public float DraggingTweenDuration = 0.1f;
    }
}