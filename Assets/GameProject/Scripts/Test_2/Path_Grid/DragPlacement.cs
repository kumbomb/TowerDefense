using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using BaseEnum;

public class DragPlacement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Object to Drag")]
    public GameObject objectPrefab;     // 실제 드래그할 오브젝트 

    [Header("Grid Overlay Prefab")]
    public GameObject gridOverlayPrefab; // Inspector에서 할당할 그리드 오버레이 프리팹

    private GameObject draggingObject;
    private GridManager gridManager;

    private List<GameObject> currentOverlays = new List<GameObject>(); // 현재 임시 오버레이 리스트

    private PlaceableObj placeableObject;   // 배치할 오브젝트의 정보
    private GameObject previewCell;
    private Vector2Int objectSizeInCells;

    public LayerMask groundLayer; // Inspector에서 Ground 레이어를 할당

    void Start()
    {
        gridManager = GridManager.Instance;
        if (gridManager == null)
        {
            Debug.LogError("GridManager.Instance가 null입니다. GridManager가 씬에 존재하는지 확인하세요.");
        }
        if (gridOverlayPrefab == null)
        {
            Debug.LogError("GridOverlayPrefab이 할당되지 않았습니다.");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (draggingObject != null)
        {
            //Debug.LogWarning("이미 드래그 중인 오브젝트가 있습니다.");
            return;
        }

        // Instantiate the dragging object
        draggingObject = Instantiate(objectPrefab);
        placeableObject = draggingObject.GetComponent<PlaceableObj>();

        if (placeableObject == null)
        {
            //Debug.LogError("PlaceableObj 스크립트가 오브젝트에 추가되어 있지 않습니다.");
            Destroy(draggingObject);
            return;
        }


        // 오브젝트의 크기가 설정되어 있는지 확인
        if (placeableObject.size == Vector2Int.zero)
        {
            //Debug.LogError("PlaceableObj의 size 값이 설정되어 있지 않습니다.");
            Destroy(draggingObject);
            return;
        }


        previewCell = Instantiate(placeableObject.previewCell);

        // 오브젝트의 실제 크기를 기반으로 셀 단위 크기를 계산
        objectSizeInCells = new Vector2Int(
            Mathf.CeilToInt(placeableObject.size.x / gridManager.cellSize),
            Mathf.CeilToInt(placeableObject.size.y / gridManager.cellSize)
        );

        // 초기 임시 오버레이 생성
        UpdateOverlays();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggingObject == null || placeableObject == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, gridManager.gridOrigin);
        float distance;

        if (plane.Raycast(ray, out distance))
        {
            Vector3 point = ray.GetPoint(distance);
            Vector3 localPoint = point - gridManager.gridOrigin;

            int x = Mathf.FloorToInt(localPoint.x / gridManager.cellSize);
            int y = Mathf.FloorToInt(localPoint.z / gridManager.cellSize);

            x -= Mathf.FloorToInt(objectSizeInCells.x / 2f);
            y -= Mathf.FloorToInt(objectSizeInCells.y / 2f);

            bool isOutOfGrid = false;
            if (x < 0 || y < 0 || x + objectSizeInCells.x > gridManager.gridWidth || y + objectSizeInCells.y > gridManager.gridHeight)
            {
                isOutOfGrid = true;
            }

            if (!isOutOfGrid)
            {
                x = Mathf.Clamp(x, 0, gridManager.gridWidth - objectSizeInCells.x);
                y = Mathf.Clamp(y, 0, gridManager.gridHeight - objectSizeInCells.y);
            }

            GridCell currentCell = null;
            if (!isOutOfGrid)
            {
                currentCell = gridManager.gridCells[x, y];
            }

            Vector3 snappedPosition = gridManager.gridOrigin + new Vector3(
                (x + (objectSizeInCells.x / 2f)) * gridManager.cellSize,
                0,
                (y + (objectSizeInCells.y / 2f)) * gridManager.cellSize
            );
            draggingObject.transform.position = snappedPosition;

            bool canPlace = false;
            bool isBlockingPath = false;
            if (!isOutOfGrid)
            {
                canPlace = CanPlaceObjectAt(x, y, objectSizeInCells);
                if (canPlace)
                {
                    // 실제로 그리드를 변경하지 않고, 배치가 모든 몬스터의 경로를 막지 않는지 검사
                    isBlockingPath = IsPlacementBlockingAllMonsterPaths(x, y, objectSizeInCells, true);
                }
            }

            // 임시 오버레이 업데이트 (배치 가능 및 경로 차단 여부 반영)
            UpdateOverlays(canPlace && !isBlockingPath, isOutOfGrid);

            // 경로 차단 여부에 따른 색상 처리 추가
            if (isBlockingPath)
            {
                foreach (var overlay in currentOverlays)
                {
                    SpriteRenderer renderer = overlay.GetComponent<SpriteRenderer>();
                    if (renderer != null)
                    {
                        renderer.color = Color.red;
                    }
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggingObject != null)
        {
            Vector3 objPosition = draggingObject.transform.position;
            Vector2Int gridPosition = new Vector2Int(
                Mathf.FloorToInt((objPosition.x - gridManager.gridOrigin.x) / gridManager.cellSize),
                Mathf.FloorToInt((objPosition.z - gridManager.gridOrigin.z) / gridManager.cellSize)
            );

            bool canPlace = CanPlaceObjectAt(gridPosition.x, gridPosition.y, objectSizeInCells);
            bool isBlockingPath = false;

            if (canPlace)
            {
                // 배치가 모든 몬스터의 경로를 막지 않는지 최종 검사
                isBlockingPath = IsPlacementBlockingAllMonsterPaths(gridPosition.x, gridPosition.y, objectSizeInCells,true);
            }

            if (canPlace && !isBlockingPath)
            {
                // 배치 가능, 실제로 배치
                PlaceObject(gridPosition.x, gridPosition.y);
            }
            else
            {
                if (isBlockingPath)
                {
                    Debug.LogWarning("배치하려는 오브젝트가 모든 몬스터의 이동 경로를 막습니다. 배치를 취소합니다.");
                }
                Destroy(draggingObject);
            }

            // 임시 오버레이 제거
            foreach (var overlay in currentOverlays)
            {
                Destroy(overlay);
            }
            currentOverlays.Clear();

            draggingObject = null;
            placeableObject = null;
        }
    }

    /// <summary>
    /// 설치 가능 여부를 검사합니다.
    /// </summary>
    bool CanPlaceObjectAt(int x, int y, Vector2Int size)
    {
        for (int i = x; i < x + size.x; i++)
        {
            for (int j = y; j < y + size.y; j++)
            {
                if (i < 0 || i >= gridManager.gridWidth || j < 0 || j >= gridManager.gridHeight)
                    return false;

                if (!gridManager.gridCells[i, j].IsCanPlace())
                    return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 배치하려는 오브젝트가 모든 몬스터의 이동 경로를 막는지 검사합니다.
    /// </summary>
    bool IsPlacementBlockingAllMonsterPaths(int x, int y, Vector2Int size, bool isTemporary)
    {
        List<GridCell> placementCells = new List<GridCell>();
        for (int i = x; i < x + size.x; i++)
        {
            for (int j = y; j < y + size.y; j++)
            {
                if (i >= 0 && i < gridManager.gridWidth && j >= 0 && j < gridManager.gridHeight)
                {
                    GridCell cell = gridManager.gridCells[i, j];
                    if (isTemporary)
                    {
                        placementCells.Add(cell);
                    }
                }
            }
        }

        // 임시 배치 시 GridChanged 이벤트를 트리거하지 않음
        bool allPathsAvailable = true;
        GridCell goalCell = gridManager.GetGoalCell();
        List<GridCell> monsterSpawnCells = gridManager.GetMonsterSpawnCellList();

        // placeableObject의 PLACEMENTTYPE이 Floor인 경우 몬스터가 이동 가능하다고 간주
        if (placeableObject != null && placeableObject.placeType == PLACEMENTTYPE.FLOOR)
        {
            allPathsAvailable = true;
        }
        else
        {
            foreach (var spawnCell in monsterSpawnCells)
            {
                if (spawnCell == null || goalCell == null)
                    continue;

                List<GridCell> path = gridManager.FindPathWithTemporaryPlacement(spawnCell, goalCell, placementCells);
                if (path == null || path.Count == 0)
                {
                    allPathsAvailable = false;
                    break;
                }
            }
        }

        // 배치 가능 여부 반환 (모든 경로가 차단되지 않은 경우에만 true 반환)
        return !allPathsAvailable;
    }

    /// <summary>
    /// 임시 오버레이를 업데이트하여 시각적으로 표시합니다.
    /// </summary>
    void UpdateOverlays(bool canPlace = true, bool isOutOfGrid = false)
    {
        // 기존 오버레이 제거
        foreach (var overlay in currentOverlays)
        {
            Destroy(overlay);
        }
        currentOverlays.Clear();

        if (draggingObject != null)
        {
            Vector3 objPosition = draggingObject.transform.position;
            Vector2Int gridPosition = new Vector2Int(
                Mathf.FloorToInt((objPosition.x - gridManager.gridOrigin.x) / gridManager.cellSize),
                Mathf.FloorToInt((objPosition.z - gridManager.gridOrigin.z) / gridManager.cellSize)
            );
             
            bool overallCanPlace = canPlace && !isOutOfGrid;

            for (int i = 0; i < objectSizeInCells.x; i++)
            {
                for (int j = 0; j < objectSizeInCells.y; j++)
                {
                    int gridX = gridPosition.x + i;
                    int gridY = gridPosition.y + j;

                    bool cellOutOfGrid = gridX < 0 || gridX >= gridManager.gridWidth ||
                                         gridY < 0 || gridY >= gridManager.gridHeight;

                    bool cellCanPlace = false;
                    if (!cellOutOfGrid)
                    {
                        cellCanPlace = gridManager.gridCells[gridX, gridY].IsCanPlace();
                    }

                    Color overlayColor;
                    if (cellOutOfGrid || !cellCanPlace)
                    {
                        overlayColor = Color.red;
                        overallCanPlace = false;
                    }
                    else
                    {
                        overlayColor = Color.green;
                    }

                    if (!cellOutOfGrid)
                    {
                        Vector3 overlayPosition = gridManager.gridOrigin + new Vector3(
                            gridX * gridManager.cellSize,
                            0.01f, // 약간 위로 띄워서 드래그 오브젝트와 겹치지 않도록 함
                            gridY * gridManager.cellSize
                        );

                        Quaternion rotation = gridOverlayPrefab.transform.rotation;

                        GameObject overlay = Instantiate(gridOverlayPrefab, overlayPosition, rotation);
                        SpriteRenderer renderer = overlay.GetComponent<SpriteRenderer>();
                        if (renderer != null)
                        {
                            renderer.color = overlayColor;
                        }
                        else
                        {
                            Debug.LogWarning("GridOverlayPrefab에 SpriteRenderer가 없습니다.");
                        }

                        currentOverlays.Add(overlay);
                    }
                    else
                    {
                        Vector3 overlayPosition = gridManager.gridOrigin + new Vector3(
                            gridX * gridManager.cellSize,
                            0.01f,
                            gridY * gridManager.cellSize
                        );

                        Quaternion rotation = gridOverlayPrefab.transform.rotation;

                        GameObject overlay = Instantiate(gridOverlayPrefab, overlayPosition, rotation);
                        SpriteRenderer renderer = overlay.GetComponent<SpriteRenderer>();
                        if (renderer != null)
                        {
                            renderer.color = Color.red;
                        }
                        else
                        {
                            Debug.LogWarning("GridOverlayPrefab에 SpriteRenderer가 없습니다.");
                        }

                        currentOverlays.Add(overlay);
                    }
                }
            }

            if (!canPlace || isOutOfGrid)
            {
                // 이미 개별 셀에서 빨간색으로 표시되므로 추가 작업 불필요
            }
            else if (!canPlace)
            {
                // 전체 배치가 불가능한 경우, 전체 오버레이를 빨간색으로 변경
                foreach (var overlay in currentOverlays)
                {
                    SpriteRenderer renderer = overlay.GetComponent<SpriteRenderer>();
                    if (renderer != null)
                    {
                        renderer.color = Color.red;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 오브젝트를 그리드에 배치합니다.
    /// </summary>
    void PlaceObject(int x, int y)
    {
        Debug.Log("Object placed successfully.");

        for (int i = x; i < x + objectSizeInCells.x; i++)
        {
            for (int j = y; j < y + objectSizeInCells.y; j++)
            {
                if (i >= 0 && i < gridManager.gridWidth && j >= 0 && j < gridManager.gridHeight)
                {
                    GridCell cell = gridManager.gridCells[i, j];
                    cell.placedObject = draggingObject;
                    cell.PlaceState = PLACEMENTSTATE.IMPLACABLE;
                    cell.PlaceType = placeableObject.placeType;
                    cell.SetWalkable(cell.PlaceType == PLACEMENTTYPE.FLOOR 
                        || cell.PlaceType == PLACEMENTTYPE.NONE);
                }
            }
        }

        gridManager.TriggerGridChanged();
    }
}
