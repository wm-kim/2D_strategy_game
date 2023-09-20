using System.Collections.Generic;
using UnityEngine;

namespace Minimax.GamePlay.GridSystem
{
    public class ServerGrid : IGrid<ServerCell>
    {
        public ServerCell[,] Cells { get; }
        public int Width { get; }
        public int Height { get; }
        
        private IPathFinding<ServerCell> m_pathfinding;

        public ServerGrid(int width, int height, PathFinding pathfinding)
        {
            Width = width;
            Height = height;
            m_pathfinding = PathFindingFactory.Create<ServerCell>(pathfinding);

            Cells = new ServerCell[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0 ; y < height; y++)
                {
                    Cells[x, y] = new ServerCell(x, y);
                }
            }
        }
        
        /// <summary>
        /// grid x, y 좌표가 grid 내에 있는지 확인합니다.
        /// </summary>
        private bool IsWithinGridBounds(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }
        
        public List<ServerCell> GetPath(Vector2Int start, Vector2Int target)
        {
            return m_pathfinding.FindPath(Cells[start.x, start.y], Cells[target.x, target.y], this);
        }
        
        public List<ServerCell> GetNeighbors(ServerCell cell)
        {
            List<ServerCell> neighbors = new List<ServerCell>();
            
            int[] dx = { 0, 0, -1, 1 };
            int[] dy = { -1, 1, 0, 0 };
            
            for (int i = 0; i < dx.Length; i++)
            {
                int nx = cell.Coord.x + dx[i];
                int ny = cell.Coord.y + dy[i];

                if (IsWithinGridBounds(nx, ny))
                {
                    neighbors.Add(Cells[nx, ny]);
                }
            }
            
            return neighbors;
        }
    }
}