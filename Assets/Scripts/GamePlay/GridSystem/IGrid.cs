using System.Collections.Generic;
using UnityEngine;

namespace Minimax.GamePlay.GridSystem
{
    public interface IGrid<T> where T : ICell
    {
        T[,] Cells { get; }

        int Width  { get; }
        int Height { get; }

        /// <summary>
        /// cell의 인접한 cell들을 반환합니다.
        /// </summary>
        List<T> GetNeighbors(T cell);

        List<T> GetPath(Vector2Int start, Vector2Int target);
    }
}