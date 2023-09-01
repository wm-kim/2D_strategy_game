using System;
using System.Diagnostics;
using Minimax.Utilities;
using UnityEngine;
using UnityEngine.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Minimax.GamePlay.GridSystem
{
  /// <summary>
  /// Contains the logic for the grid system, have collection of cells
  /// </summary>
  public class Grid<TGridCell>  
  {
    /// <summary>
    /// 그리드 값이 변경되었을 때 발생하는 이벤트
    /// </summary>
    public event Action<int, int> OnGridValueChanged;

    private int m_width;
    private int m_height;
    private Vector3 m_cellSize;
    
    public int Width => m_width;
    public int Height => m_height;
    
    public Vector2 GetSize() => 
      new Vector2(m_width * m_cellSize.x * 0.5f, m_height * m_cellSize.y * 0.5f);
    
    private TGridCell[,] m_cells;
    private Vector2 m_originPos;

    public Grid(int width, int height, Vector3 cellSize, Vector2 originPos,
      Func<Grid<TGridCell>, int, int, TGridCell> createGridObject)
    {
      m_width = width;
      m_height = height;
      m_cellSize = cellSize;
      m_originPos = originPos;

      m_cells = new TGridCell[width, height];

      for (int x = 0; x < m_cells.GetLength(0); x++)
      {
        for (int y = 0; y < m_cells.GetLength(1); y++)
        {
          m_cells[x, y] = createGridObject(this, x, y);
        }
      }
    }

#region Debug
    /// <summary>
    /// 디버그 목적으로 그리드 셀 중앙에 텍스트를 표시합니다.
    /// </summary>
    public void DebugCoord(Transform parent, float textScale)
    {
      for (int x = 0; x < m_cells.GetLength(0); x++)
      {
        for (int y = 0; y < m_cells.GetLength(1); y++)
        {
          // seems like cell unit is half of the unity unit
          var text = DebugWrapper.CreateText($"{x}, {y}", parent, 
            GetWorldPosFromCoord(x, y), textScale, Define.MapOverlay);
          text.name = $"({x}, {y})";
        }
      }
    }
#endregion

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
    /// world isometric 좌표를 grid x, y 좌표로 변환합니다.
    /// </summary>
    private Vector2Int WorldIsoToGridCoord(Vector2 worldIsoMetricPos)
    {
      var isoMetricPos = worldIsoMetricPos - m_originPos;
      var cartesianPos = IsoToCartesian(isoMetricPos.x, isoMetricPos.y);
      return new Vector2Int
      (
        Mathf.FloorToInt(cartesianPos.x / (m_cellSize.x * 0.5f)),
        Mathf.FloorToInt(cartesianPos.y / (m_cellSize.x * 0.5f))
      );
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
    
    public Vector2 GetWorldPosFromCoord(int x, int y)
    {
      var factor = 0.5f * m_cellSize.x;
      return CartesianToWorldIso((x + 0.5f) * factor, (y + 0.5f) * factor);
    }

    /// <summary>
    /// world isometric 좌표에 있는 grid cell을 반환합니다.
    /// </summary>
    public bool TryGetGridCellFromWorldIso(Vector2 worldIsoPos, out TGridCell gridCell)
    {
      Vector2Int xy = WorldIsoToGridCoord(worldIsoPos);
      return GetGridCellFromGridCoord(xy.x, xy.y, out gridCell);
    }

    /// <summary>
    /// grid x, y 좌표에 있는 grid cell을 반환합니다.
    /// </summary>
    public bool GetGridCellFromGridCoord(int x, int y, out TGridCell gridCell)
    {
      if (IsWithinGridBounds(x, y))
      {
        gridCell = m_cells[x, y];
        return true;
      }
      else
      {
        gridCell = default(TGridCell);
        return false;
      }
    }

    /// <summary>
    /// world isometric 좌표에 있는 grid cell의 값을 value로 설정합니다.
    /// </summary>
    public void SetGridCellFromWorldIso(Vector2 worldIsoPos, TGridCell value)
    {
      Vector2Int xy = WorldIsoToGridCoord(worldIsoPos);
      SetGridCellFromGridCoord(xy.x, xy.y, value);
    }

    /// <summary>
    /// grid x, y 좌표에 있는 grid cell을 value로 설정합니다.
    /// </summary>
    public void SetGridCellFromGridCoord(int x, int y, TGridCell value)
    {
      if (IsWithinGridBounds(x, y))
      {
        m_cells[x, y] = value;
        TriggerObjectChanged(x, y);
      }
      else
      {
        throw new ArgumentOutOfRangeException("x,y", "Index out of grid bounds");
      }
    }
    
    private void TriggerObjectChanged(int x, int y)
    {
        OnGridValueChanged?.Invoke(x, y);
    }
  }
}