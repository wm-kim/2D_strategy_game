using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace Minimax.GamePlay.GridSystem
{
    public class AStarPathFinding<T> : IPathFinding<T> where T : ICell
    {
        public List<T> FindPath(T start, T target, IGrid<T> grid)
        {
            // g cost : start cell에서 현재 cell까지의 이동 비용
            // h cost : 현재 cell에서 target cell까지의 추정 이동 비용 (휴리스틱, underestimate)
            // f cost : g cost + h cost, choose the lowest f cost than choose the lowest h cost

            Debug.Log($"FindPath from {start.Coord} to {target.Coord}");
            var toSearch  = new List<T>();
            var processed = new HashSet<T>();

            // 모든 셀에 대한 g, h, f 값을 저장
            var gCosts = new Dictionary<T, int>();
            var hCosts = new Dictionary<T, int>();
            var fCosts = new Dictionary<T, int>();

            // 부모 노드 저장
            var cameFrom = new Dictionary<T, T>();

            toSearch.Add(start);
            gCosts[start] = 0;
            hCosts[start] = start.GetDistance(target);
            fCosts[start] = hCosts[start];

            while (toSearch.Count > 0)
            {
                // f cost가 가장 낮은 셀을 선택
                var current = toSearch[0];
                for (var i = 1; i < toSearch.Count; i++)
                    if (fCosts[toSearch[i]] < fCosts[current] ||
                        (fCosts[toSearch[i]] == fCosts[current] && hCosts[toSearch[i]] < hCosts[current]))
                        current = toSearch[i];

                // 선택된 셀을 열린 셀 리스트에서 제거하고 닫힌 셀 리스트에 추가
                processed.Add(current);
                toSearch.Remove(current);

                // 선택된 셀이 target 셀이면 경로를 반환
                if (current.Equals(target)) return ReconstructPath(cameFrom, current);

                // 선택된 셀의 인접한 셀들을 탐색
                foreach (var neighbor in grid.GetNeighbors(current).Where(neighbor => neighbor.IsWalkable &&
                             !processed.Contains(neighbor)))
                {
                    var inToSearch     = toSearch.Contains(neighbor);
                    var tentativeGCost = gCosts[current] + neighbor.GetDistance(current);

                    // 인접한 셀이 열린 셀 리스트에 없거나, 새로운 경로가 더 짧을 경우
                    if (!inToSearch || tentativeGCost < gCosts[neighbor])
                    {
                        // 부모 노드를 현재 셀로 설정
                        cameFrom[neighbor] = current;

                        // g, h, f cost를 갱신
                        gCosts[neighbor] = tentativeGCost;
                        hCosts[neighbor] = neighbor.GetDistance(target);
                        fCosts[neighbor] = gCosts[neighbor] + hCosts[neighbor];

                        // 열린 셀 리스트에 추가
                        if (!inToSearch) toSearch.Add(neighbor);
                    }
                }
            }

            // 경로가 없을 경우 빈 리스트 반환
            Debug.Log($"No path found from {start.Coord} to {target.Coord}");
            return new List<T>();
        }

        private List<T> ReconstructPath(Dictionary<T, T> cameFrom, T current)
        {
            var path = new List<T> { current };

            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Add(current);
            }

            // 시작 위치 제거
            path.RemoveAt(path.Count - 1);

            // 역순으로 반환
            path.Reverse();
            return path;
        }
    }
}