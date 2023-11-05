using UnityEngine;

namespace Minimax.GamePlay.GridSystem
{
    public enum GridRotation
    {
        Default,
        Rotate90,
        Rotate180,
        Rotate270
    }

    // Extensions
    public static class GridRotationExtensions
    {
        public static GridRotation GetRelativeRotation(this GridRotation sourceRotation, GridRotation targetRotation)
        {
            return (GridRotation)(Mathf.Abs((int)sourceRotation - (int)targetRotation) % 4);
        }
    }
}