using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder
{
    private GridManager gridManager;
    private PathCache pathCache;

    public AStarPathfinder(GridManager gridManager)
    {
        this.gridManager = gridManager;
        this.pathCache = new PathCache(); // 기본 최대 크기 사용
    }

    /// <summary>
    /// A* 알고리즘을 사용하여 시작 셀에서 목표 셀까지의 경로를 찾습니다.
    /// </summary>
    public List<GridCell> FindPath(GridCell startCell, GridCell endCell)
    {
        if (startCell == null || endCell == null || !endCell.IsWalkable())
            return null;

        // 캐시에서 경로 확인
        List<GridCell> cachedPath = pathCache.GetPath(startCell, endCell);
        if (cachedPath != null)
        {
            return cachedPath;
        }

        // 경로 계산
        PriorityQueue<GridCell> openSet = new PriorityQueue<GridCell>();
        HashSet<GridCell> closedSet = new HashSet<GridCell>();

        startCell.G = 0;
        startCell.H = GetHeuristic(startCell, endCell);
        startCell.Parent = null;

        openSet.Enqueue(startCell, startCell.F);

        while (openSet.Count > 0)
        {
            GridCell current = openSet.Dequeue();

            if (current == endCell)
            {
                List<GridCell> path = ReconstructPath(current);
                pathCache.AddPath(startCell, endCell, path);
                return path;
            }

            closedSet.Add(current);

            foreach (GridCell neighbor in gridManager.GetNeighbors(current))
            {
                if (!neighbor.IsWalkable() || closedSet.Contains(neighbor))
                    continue;

                float tentativeG = current.G + GetDistance(current, neighbor);

                bool inOpenSet = openSet.Contains(neighbor);

                if (!inOpenSet || tentativeG < neighbor.G)
                {
                    neighbor.G = tentativeG;
                    neighbor.H = GetHeuristic(neighbor, endCell);
                    neighbor.Parent = current;

                    if (!inOpenSet)
                    {
                        openSet.Enqueue(neighbor, neighbor.F);
                    }
                }
            }
        }

        // 경로를 찾지 못함
        return FindPathInternal(startCell, endCell, new List<GridCell>());
    }

    /// <summary>
    /// 목표 셀에서 시작 셀까지의 경로를 복원합니다.
    /// </summary>
    private List<GridCell> ReconstructPath(GridCell endCell)
    {
        List<GridCell> path = new List<GridCell>();
        GridCell current = endCell;

        while (current != null)
        {
            path.Add(current);
            current = current.Parent;
        }

        path.Reverse();
        return path;
    }

    /// <summary>
    /// 두 셀 사이의 휴리스틱 (유클리드 거리)을 계산합니다.
    /// </summary>
    private float GetHeuristic(GridCell a, GridCell b)
    {
        return Vector3.Distance(a.position, b.position);
    }

    /// <summary>
    /// 두 인접한 셀 사이의 이동 비용을 계산합니다.
    /// </summary>
    private float GetDistance(GridCell a, GridCell b)
    {
        // 대각선 이동 비용
        if (a.index.x != b.index.x && a.index.y != b.index.y)
            return Mathf.Sqrt(2);
        // 직교 이동 비용
        return 1f;
    }

    /// <summary>
    /// 캐시를 초기화합니다. (그리드 변경 시 호출)
    /// </summary>
    public void ClearCache()
    {
        pathCache.Clear();
    }
    public List<GridCell> FindPathWithBlockedCells(GridCell startCell, GridCell endCell, List<GridCell> blockedCells)
    {
        return FindPathInternal(startCell, endCell, blockedCells);
    }

    private List<GridCell> FindPathInternal(GridCell startCell, GridCell endCell, List<GridCell> blockedCells)
    {
        if (startCell == null || endCell == null || !endCell.IsWalkable() || blockedCells.Contains(endCell))
            return null;

        // 기존 경로 찾기 로직에 임시로 차단된 셀을 고려
        foreach (var cell in blockedCells)
        {
            cell.SetWalkable(false);
        }

        // 경로 계산
        PriorityQueue<GridCell> openSet = new PriorityQueue<GridCell>();
        HashSet<GridCell> closedSet = new HashSet<GridCell>();

        startCell.G = 0;
        startCell.H = GetHeuristic(startCell, endCell);
        startCell.Parent = null;

        openSet.Enqueue(startCell, startCell.F);

        while (openSet.Count > 0)
        {
            GridCell current = openSet.Dequeue();

            if (current == endCell)
            {
                List<GridCell> path = ReconstructPath(current);
                // 차단된 셀 원래 상태로 복구
                foreach (var cell in blockedCells)
                {
                    cell.SetWalkable(true);
                }
                return path;
            }

            closedSet.Add(current);

            foreach (GridCell neighbor in gridManager.GetNeighbors(current))
            {
                if (!neighbor.IsWalkable() || closedSet.Contains(neighbor) || blockedCells.Contains(neighbor))
                    continue;

                float tentativeG = current.G + GetDistance(current, neighbor);

                bool inOpenSet = openSet.Contains(neighbor);

                if (!inOpenSet || tentativeG < neighbor.G)
                {
                    neighbor.G = tentativeG;
                    neighbor.H = GetHeuristic(neighbor, endCell);
                    neighbor.Parent = current;

                    if (!inOpenSet)
                    {
                        openSet.Enqueue(neighbor, neighbor.F);
                    }
                }
            }
        }

        // 차단된 셀 원래 상태로 복구
        foreach (var cell in blockedCells)
        {
            cell.SetWalkable(true);
        }

        // 경로를 찾지 못함
        return null;
    }
}
