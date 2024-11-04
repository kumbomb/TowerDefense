using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 경로 캐시를 관리하는 클래스
/// </summary>
public class PathCache
{
    private Dictionary<(GridCell, GridCell), List<GridCell>> cache = new Dictionary<(GridCell, GridCell), List<GridCell>>();

    public List<GridCell> GetPath(GridCell start, GridCell end)
    {
        if (cache.TryGetValue((start, end), out List<GridCell> path))
        {
            return path;
        }
        return null;
    }

    public void AddPath(GridCell start, GridCell end, List<GridCell> path)
    {
        if (!cache.ContainsKey((start, end)))
        {
            cache.Add((start, end), path);
        }
    }

    public void Clear()
    {
        cache.Clear();
    }
}
