using System.Collections.Generic;

namespace Minimax.GamePlay.GridSystem
{
    /// <summary>
    /// Interface for path finding strategies
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPathFinding<T> where T : ICell
    {
        List<T> FindPath(T start, T target, IGrid<T> grid);
    }

    /// <summary>
    /// PathFinding strategies, for sending client which path finding strategy is being used in the game.
    /// </summary>
    public enum PathFinding
    {
        AStar
    }

    /// <summary>
    /// Factory for creating path finding strategies
    /// </summary>
    public static class PathFindingFactory
    {
        public static IPathFinding<T> Create<T>(PathFinding pathFinding) where T : ICell
        {
            switch (pathFinding)
            {
                case PathFinding.AStar:
                    return new AStarPathFinding<T>();
                default:
                    return null;
            }
        }
    }
}