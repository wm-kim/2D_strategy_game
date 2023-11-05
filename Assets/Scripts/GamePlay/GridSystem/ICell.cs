using UnityEngine;

namespace Minimax.GamePlay.GridSystem
{
    public interface ICell
    {
        Vector2Int Coord      { get; }
        bool       IsWalkable { get; }
        int        GetDistance(ICell other);
    }
}