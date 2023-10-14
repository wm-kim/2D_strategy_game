using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Minimax.Utilities;
using UnityEngine;
using UnityEngine.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Minimax.GamePlay.GridSystem
{
  public enum GridRotation
  {
    Default,
    Rotate90,
    Rotate180,
    Rotate270,
  }
  
  /// <summary>
  /// Contains the logic for the grid system, have collection of cells
  /// </summary>
  public class ClientIsoGrid : IGrid<ClientCell>
  {
    public ClientCell[,] Cells { get; private set; }
    public int Width => m_width;
    public int Height => m_height;
    
    /// <summary>
    /// 그리드 셀의 값이 변경되었을 때 발생하는 이벤트
    /// </summary>
    public event Action<int, int> OnGridCellChanged;
    
    /// <summary>
    /// 그리드의 회전이 변경되었을 때 발생하는 이벤트
    /// </summary>
    public event Action<GridRotation> OnGridRotationChanged;

    /// <summary>
    /// 그리드의 크기를 반환합니다. 단위는 unity unit입니다.
    /// </summary>
    public Vector2 GetSize() => new(m_width * m_cellSize.x * 0.5f, m_height * m_cellSize.y * 0.5f);

    private int m_width;
    private int m_height;
    private Vector3 m_cellSize;
  
    private IPathFinding<ClientCell> m_pathFinding;
    private Vector2 m_originPos;
    private GridRotation m_rotation = GridRotation.Default;
   
    public ClientIsoGrid(int width, int height, PathFinding pathfinding, Vector3 cellSize, Vector2 originPos, 
      Func<ClientIsoGrid, int, int, ClientCell> createGridObject, GridRotation rotation = GridRotation.Default)
    {
      m_width = width;
      m_height = height;
      m_pathFinding = PathFindingFactory.Create<ClientCell>(pathfinding);
      m_cellSize = cellSize;
      m_originPos = originPos;

      Cells = new ClientCell[width, height];

      for (int x = 0; x < Cells.GetLength(0); x++)
      {
        for (int y = 0; y < Cells.GetLength(1); y++)
        {
          Cells[x, y] = createGridObject(this, x, y);
        }
      }
      
      m_rotation = rotation;
      
  #if UNITY_EDITOR
      m_debugTexts = new TextMesh[width, height];
  #endif
    }
    
    /// <summary>
    /// x, y 좌표를 회전시킨 좌표를 반환합니다.
    /// </summary>
    private Vector2Int GetRotatedCoord(int x, int y)
    {
      switch (m_rotation)
      {
        case GridRotation.Default:
          return new Vector2Int(x, y);
        case GridRotation.Rotate90:
          return new Vector2Int(m_width - y - 1, x);
        case GridRotation.Rotate180:
          return new Vector2Int(m_width - x - 1, m_height - y - 1);
        case GridRotation.Rotate270:
          return new Vector2Int(y, m_height - x - 1);
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
    
    /// <summary>
    /// Method that can be used to set the rotation of the grid on runtime
    /// </summary>
    public void SetRotation(GridRotation rotation)
    {
      m_rotation = rotation;
      for (int x = 0; x < Cells.GetLength(0); x++)
      {
        for (int y = 0; y < Cells.GetLength(1); y++)
        {
          Cells[x, y].transform.position = GetWorldPosFromCoord(x, y);
        }
      }
      DebugWrapper.Log($"Grid Rotation Changed to {rotation}");
      OnGridRotationChanged?.Invoke(rotation);
      
#if UNITY_EDITOR
      UpdateDebugText();
#endif
    }

#if UNITY_EDITOR
    private TextMesh[,] m_debugTexts;
    
    public void DisplayDebugCoords(bool display)
    {
      if (m_debugTexts == null) return;
      for (int x = 0; x < Cells.GetLength(0); x++)
      {
        for (int y = 0; y < Cells.GetLength(1); y++)
        {
          m_debugTexts[x, y].gameObject.SetActive(display);
        }
      }
    }

    /// <summary>
    /// 디버그 목적으로 그리드 셀 중앙에 텍스트를 표시합니다.
    /// </summary>
    public void GenerateDebugCoord(Transform parent, float textScale)
    {
      for (int x = 0; x < Cells.GetLength(0); x++)
      {
        for (int y = 0; y < Cells.GetLength(1); y++)
        {
          // seems like cell unit is half of the unity unit
          var text = DebugWrapper.CreateText($"{x}, {y}", parent, 
            GetWorldPosFromCoord(x, y), textScale, Define.MapOverlay);
          text.name = $"({x}, {y})";
          m_debugTexts[x, y] = text;
        }
      }
    }
    
    private void UpdateDebugText()
    {
      // check if debug text is generated
      if (m_debugTexts == null) return;
      
      for (int x = 0; x < Cells.GetLength(0); x++)
      {
        for (int y = 0; y < Cells.GetLength(1); y++)
        {
          Vector2Int rotatedCoord = GetRotatedCoord(x, y);
          Vector2 worldPos = GetWorldPosFromCoord(rotatedCoord.x, rotatedCoord.y);
          m_debugTexts[x, y].transform.position = new Vector3(worldPos.x, worldPos.y, 0);
          m_debugTexts[x, y].text = $"{rotatedCoord.x}, {rotatedCoord.y}";
        }
      }
    }
#endif

    /// <summary>
    /// grid x, y 좌표가 grid 내에 있는지 확인합니다.
    /// </summary>
    private bool IsWithinGridBounds(int x, int y)
    {
      return x >= 0 && y >= 0 && x < m_width && y < m_height;
    }
  
    /// <summary>
    /// cartesian 좌표를 isometric 좌표로 변환합니다.
    /// </summary>
    private Vector2 CartesianToIso(float x, float y)
    {
      return new Vector2(
        (x - y),
        (x + y) * (m_cellSize.y / m_cellSize.x)
      );
    }

    /// <summary>
    /// isometric 좌표를 cartesian 좌표로 변환합니다.
    /// </summary>
    private Vector2 IsoToCartesian(float x, float y)
    {
      return new Vector2(
        (y * (m_cellSize.x / m_cellSize.y) + x) / 2,
        (y * (m_cellSize.x / m_cellSize.y) - x) / 2
      );
    }

    /// <summary>
    /// cartesian 좌표를 world isometric 좌표로 변환합니다.
    /// </summary>
    private Vector2 CartesianToWorldIso(float x, float y)
    {
      Vector2 xyIsometric = CartesianToIso(x, y);
      return xyIsometric + m_originPos;
    }
        
    /// <summary>
    /// 그리드의 중심에 해당하는 world cartesian 좌표를 반환합니다.
    /// </summary>
    public Vector2 GetGridCenterPos()
    {
      float centerX = m_width * 0.5f;
      float centerY = m_height * 0.5f;
      // seems like cell unit is half of the unity unit
      var factor = 0.5f * m_cellSize.x;
      return CartesianToWorldIso(centerX * factor, centerY * factor);
    }

    /// <summary>
    /// world isometric 좌표를 grid x, y 좌표로 변환합니다.
    /// </summary>
    private Vector2Int WorldIsoToGridCoord(Vector2 worldIsoMetricPos)
    {
      var isoMetricPos = worldIsoMetricPos - m_originPos;
      var cartesianPos = IsoToCartesian(isoMetricPos.x, isoMetricPos.y);
      var coord =  new Vector2Int
      (
        Mathf.FloorToInt(cartesianPos.x / (m_cellSize.x * 0.5f)),
        Mathf.FloorToInt(cartesianPos.y / (m_cellSize.x * 0.5f))
      );
      return GetRotatedCoord(coord.x, coord.y);
    }
    
    public Vector2 GetWorldPosFromCoord(int x, int y)
    {
      Vector2Int rotatedCoord = GetRotatedCoord(x, y);
      // seems like cell unit is half of the unity unit
      var factor = 0.5f * m_cellSize.x;
      return CartesianToWorldIso((rotatedCoord.x + 0.5f) * factor, (rotatedCoord.y + 0.5f) * factor);
    }

    /// <summary>
    /// world isometric 좌표에 있는 grid cell을 반환합니다.
    /// </summary>
    public bool TryGetGridCellFromWorldIso(Vector2 worldIsoPos, out ClientCell gridClientCell)
    {
      Vector2Int xy = WorldIsoToGridCoord(worldIsoPos);
      return GetGridCellFromGridCoord(xy.x, xy.y, out gridClientCell);
    }

    /// <summary>
    /// grid x, y 좌표에 있는 grid cell을 반환합니다.
    /// </summary>
    public bool GetGridCellFromGridCoord(int x, int y, out ClientCell gridClientCell)
    {
      if (IsWithinGridBounds(x, y))
      {
        gridClientCell = Cells[x, y];
        return true;
      }
      else
      {
        gridClientCell = null;
        return false;
      }
    }

    /// <summary>
    /// world isometric 좌표에 있는 grid cell의 값을 value로 설정합니다.
    /// </summary>
    public void SetGridCellFromWorldIso(Vector2 worldIsoPos, ClientCell value)
    {
      Vector2Int xy = WorldIsoToGridCoord(worldIsoPos);
      SetGridCellFromGridCoord(xy.x, xy.y, value);
    }

    /// <summary>
    /// grid x, y 좌표에 있는 grid cell을 value로 설정합니다.
    /// </summary>
    public void SetGridCellFromGridCoord(int x, int y, ClientCell value)
    {
      if (IsWithinGridBounds(x, y))
      {
        Cells[x, y] = value;
        TriggerCellChanged(x, y);
      }
      else
      {
        throw new ArgumentOutOfRangeException("x,y", "Index out of grid bounds");
      }
    }
    
    private void TriggerCellChanged(int x, int y)
    {
      OnGridCellChanged?.Invoke(x, y);
    }
    
    /// <summary>
    /// Returns the neighbors of a cell.
    /// </summary>
    public List<ClientCell> GetNeighbors(ClientCell cell)
    {
      List<ClientCell> neighbors = new List<ClientCell>();
      
      int[] dx = { 0, 1, -1, 0 };
      int[] dy = { 1, 0, 0, -1 };
      
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
    
    public List<ClientCell> GetPath(Vector2Int start, Vector2Int target)
    {
      return m_pathFinding.FindPath(Cells[start.x, start.y], Cells[target.x, target.y], this);
    }
    
    public List<ClientCell> GetReachableCells(ClientCell startCell, int range)
    { 
      // BFS를 위한 큐와 거리 맵을 초기화합니다.
      Queue<ClientCell> queue = new Queue<ClientCell>();
      queue.Enqueue(startCell);
      Dictionary<ClientCell, int> distances = new Dictionary<ClientCell, int>();
      distances[startCell] = 0;

      // 결과를 담을 리스트를 초기화합니다.
      List<ClientCell> reachableCells = new List<ClientCell>();

      while (queue.Count > 0)
      {
        ClientCell currentCell = queue.Dequeue();

        // 현재 셀의 이웃을 검사합니다.
        foreach (ClientCell neighbor in GetNeighbors(currentCell))
        {
          // 아직 방문하지 않은 이웃 셀이거나 이동 가능한 셀이면 큐에 추가합니다.
          if (!distances.ContainsKey(neighbor) && neighbor.IsWalkable)
          {
            queue.Enqueue(neighbor);
            distances[neighbor] = distances[currentCell] + 1;

            // 최대 이동 거리 내에 있는 경우 결과 목록에 추가합니다.
            if (distances[neighbor] <= range)
            {
              reachableCells.Add(neighbor);
            }
          }
        }
      }

      return reachableCells;
    }
  }
}