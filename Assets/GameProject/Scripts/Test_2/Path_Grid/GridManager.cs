using UnityEngine;
using BaseStruct;
using BaseEnum;
using System;
using System.Collections.Generic;

public class GridManager : Singleton<GridManager>
{
    [SerializeField] public int gridWidth;
    [SerializeField] public int gridHeight;
    [SerializeField] public float cellSize;
    [SerializeField] public GameObject cellPrefab; // 셀 프리팹
    [SerializeField] public Vector3 gridOrigin = Vector3.zero; // 그리드 시작 위치 설정

    [NonSerialized] public GridCell[,] gridCells;   //실제 사용 그리드 영역

    // 그리드 데이터를 직렬화하기 위한 리스트
    [SerializeField] public List<GridCell> serializedGridCells = new List<GridCell>();

    // 경로 찾기 인스턴스
    private AStarPathfinder pathfinder;

    #region 그리드 셀 관련 정보
    // 목표 지점
    [SerializeField] GridCell goalCell = null;
    [SerializeField] GridCell playerSpawnCell = null;
    [SerializeField] List<GridCell> monSpawnCell = new List<GridCell>();

    // 장애물이 변경될 때 발생하는 이벤트
    public event System.Action OnGridChanged;
    #endregion

    #region 에디터용 기능   
    // 커스텀 에디터에서 그리드를 만들었다면, Start에서 그리드를 생성하지 않음
    [SerializeField] public bool gridCreatedInEditor = false;
    #endregion

    void Awake()
    {
        // 게임 시작 시 serializedGridCells를 이용하여 gridCells를 복원
        if (gridCreatedInEditor && (gridCells == null || gridCells.Length == 0))
        {
            LoadGridFromSerializedData();
        }
    }

    void Start()
    {
        if (gridCreatedInEditor)
        {
            if (gridCells == null || gridCells.Length == 0)
            {
                LoadGridFromSerializedData();
            }
            InstantiateCellsInScene();
        }
        else
        {
            CreateGrid();
        }

        // Init Cell => 목표 지점 / 몬스터 스폰 포인트 
        InitCellData();

        // 경로 찾기 인스턴스 초기화
        pathfinder = new AStarPathfinder(this);

        // 그리드가 변경될 때 캐시를 초기화하도록 이벤트 구독
        OnGridChanged += HandleGridChanged;
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
        OnGridChanged -= HandleGridChanged;
    }

    public void CreateGrid()
    {
        // 기존 그리드가 존재하면 삭제
        ClearGrid();

        gridCells = new GridCell[gridWidth, gridHeight];

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Vector3 cellPosition = gridOrigin + new Vector3(x * cellSize, 0, y * cellSize);

                // GridCell 생성
                GridCell cell = new GridCell(cellPosition, new Coord(x, y, cellPosition.x, cellPosition.z));

                gridCells[x, y] = cell;
            }
        }

        // 직렬화된 데이터를 업데이트
        SaveGridToSerializedData();

        monSpawnCell.Clear();
    }

    // 그리드 상태를 직렬화하여 저장
    public void SaveGridToSerializedData()
    {
        serializedGridCells.Clear();

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                GridCell cell = gridCells[x, y];
                serializedGridCells.Add(cell);
            }
        }
    }

    // 직렬화된 데이터를 바탕으로 그리드를 복구
    public void LoadGridFromSerializedData()
    {
        if (serializedGridCells.Count == 0)
        {
            Debug.LogWarning("Serialized grid cells are empty!");
            return;
        }

        gridCells = new GridCell[gridWidth, gridHeight];

        foreach (var cellData in serializedGridCells)
        {
            int x = cellData.index.x;
            int y = cellData.index.y;

            if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
            {
                gridCells[x, y] = cellData;
            }
            else
            {
                Debug.LogError($"Cell index out of bounds: x={x}, y={y}");
            }
        }
    }

    public void InstantiateCellsInScene()
    {
        ClearGrid();

        if (gridCells == null || gridCells.Length == 0)
        {
            Debug.LogError("gridCells is null or empty!");
            return;
        }

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                GridCell cell = gridCells[x, y];

                if (cell == null)
                {
                    Debug.LogError($"gridCells[{x},{y}] is null!");
                    continue;
                }

                Vector3 cellPosition = gridOrigin + new Vector3(x * cellSize, 0, y * cellSize);

                // 셀 GameObject 생성
                GameObject cellObj = Instantiate(cellPrefab, cellPosition, Quaternion.identity);
                cellObj.transform.name = $"cell_{x}x{y}";
                cellObj.transform.localScale = new Vector3(cellSize, 1, cellSize);
                cellObj.transform.parent = this.transform;

                cell.cellObject = cellObj;
                cell.SetCellObject(cellObj);

                // 셀 상태에 따른 색상 업데이트
                cell.UpdateGridColor();
            }
        }
    }

    // 몬스터 스폰 셀이나 목표 도달 셀 설정
    void InitCellData()
    {
        if (monSpawnCell != null)
            monSpawnCell.Clear();

        foreach (var cellData in serializedGridCells)
        {
            if (cellData.PlaceState == PLACEMENTSTATE.ENDPOINT)
            {
                goalCell = cellData;
            }
            else if (cellData.PlaceState == PLACEMENTSTATE.MONSTERSPAWN)
            {
                monSpawnCell.Add(cellData);
            }
            else if (cellData.PlaceState == PLACEMENTSTATE.STARTPOINT)
            {
                playerSpawnCell = cellData;
            }
        }
    }

    // 몬스터 스폰 가능 셀 리턴
    public List<GridCell> GetMonsterSpawnCellList()
    {
        return monSpawnCell;
    }

    // 몬스터 목표 지점 셀 리턴
    public GridCell GetGoalCell()
    {
        return goalCell;
    }

    // 플레이어 캐릭터 생성 셀 리턴
    public GridCell GetPlayerSpawnCell()
    {
        return playerSpawnCell;
    }

    #region 도우미 메서드

    /// <summary>
    /// 월드 위치에 해당하는 GridCell을 가져옵니다.
    /// </summary>
    public GridCell GetCellFromWorldPosition(Vector3 worldPosition)
    {
        float xOffset = worldPosition.x - gridOrigin.x;
        float yOffset = worldPosition.z - gridOrigin.z;

        int x = Mathf.RoundToInt(xOffset / cellSize);
        int y = Mathf.RoundToInt(yOffset / cellSize);

        if (IsWithinGrid(x, y))
        {
            return gridCells[x, y];
        }

        return null;
    }

    /// <summary>
    /// 주어진 좌표가 그리드 범위 내에 있는지 확인합니다.
    /// </summary>
    public bool IsWithinGrid(int x, int y)
    {
        return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
    }

    /// <summary>
    /// 주어진 셀의 모든 이웃 셀을 가져옵니다 (대각선 포함, 대각선 이동 조건 적용).
    /// </summary>
    public List<GridCell> GetNeighbors(GridCell cell)
    {
        List<GridCell> neighbors = new List<GridCell>();

        int x = cell.index.x;
        int y = cell.index.y;

        // 방향: 북, 북동, 동, 남동, 남, 남서, 서, 북서
        int[] dx = { -1, -1, 0, 1, 1, 1, 0, -1 };
        int[] dy = { 1, 0, -1, 0, -1, -1, 1, 1 };

        for (int i = 0; i < dx.Length; i++)
        {
            int newX = x + dx[i];
            int newY = y + dy[i];

            if (IsWithinGrid(newX, newY))
            {
                bool isDiagonal = dx[i] != 0 && dy[i] != 0;
                if (isDiagonal)
                {
                    // 대각선 이동인 경우, 해당 방향의 좌우 셀을 확인
                    int adjacent1X = x + dx[i];
                    int adjacent1Y = y;
                    int adjacent2X = x;
                    int adjacent2Y = y + dy[i];

                    bool adj1Walkable = false;
                    bool adj2Walkable = false;

                    if (IsWithinGrid(adjacent1X, adjacent1Y))
                    {
                        adj1Walkable = gridCells[adjacent1X, adjacent1Y].IsWalkable();
                    }

                    if (IsWithinGrid(adjacent2X, adjacent2Y))
                    {
                        adj2Walkable = gridCells[adjacent2X, adjacent2Y].IsWalkable();
                    }

                    // 좌우 셀 중 하나라도 이동 가능해야 대각선 이동 허용
                    if (adj1Walkable || adj2Walkable)
                    {
                        GridCell neighbor = gridCells[newX, newY];
                        neighbors.Add(neighbor);
                    }
                }
                else
                {
                    // 직교 이동인 경우, 그대로 추가
                    GridCell neighbor = gridCells[newX, newY];
                    neighbors.Add(neighbor);
                }
            }
        }

        return neighbors;
    }

    #endregion

    /// <summary>
    /// A* 경로 찾기 인스턴스를 통해 경로를 요청합니다.
    /// </summary>
    /// <param name="startCell">시작 GridCell</param>
    /// <param name="endCell">목표 GridCell</param>
    /// <returns>경로를 나타내는 GridCell 리스트 또는 null</returns>
    public List<GridCell> FindPath(GridCell startCell, GridCell endCell)
    {
        if (pathfinder == null)
        {
            pathfinder = new AStarPathfinder(this);
        }
        return pathfinder.FindPath(startCell, endCell);
    }

    public List<GridCell> FindPathWithTemporaryPlacement(GridCell startCell, GridCell endCell, List<GridCell> blockedCells)
    {
        if (pathfinder == null)
        {
            pathfinder = new AStarPathfinder(this);
        }
        return pathfinder.FindPathWithBlockedCells(startCell, endCell, blockedCells);
    }

    // 그리드 변경 이벤트를 트리거하는 메서드
    public void TriggerGridChanged()
    {
        OnGridChanged?.Invoke();
        pathfinder.ClearCache(); // 그리드 변경 시 경로 캐시 초기화
    }

    // 그리드가 변경될 때 캐시를 무효화하는 메서드
    private void HandleGridChanged()
    {
        if (pathfinder != null)
        {
            pathfinder.ClearCache();
        }
    }

    // 그리드를 제거하는 함수
    public void ClearGrid()
    {
        if (gridCells != null)
        {
            foreach (var cell in gridCells)
            {
                if (cell != null && cell.cellObject != null)
                {
                    DestroyImmediate(cell.cellObject);
                    cell.cellObject = null;
                }
            }
        }
        if (transform.childCount > 0)
        {
            int maxCount = transform.childCount;
            for (int i = maxCount - 1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}
