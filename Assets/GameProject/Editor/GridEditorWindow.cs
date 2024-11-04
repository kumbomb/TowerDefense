using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using BaseEnum;
using BaseStruct;

public class GridEditorWindow : EditorWindow
{
    // 기존 변수들
    private int gridWidth = 10;
    private int gridHeight = 10;
    private float cellSize = 1f; // 실제 월드 공간에서의 셀 크기
    private float cellSizeInEditor = 32f; // 에디터 윈도우에서의 셀 크기 (픽셀 단위)

    // 그리드 시작 위치 변수 추가
    private Vector3 gridOrigin = Vector3.zero;

    private GridCell[,] gridCells;
    private PLACEMENTSTATE selectedState = PLACEMENTSTATE.NONE;
    private Vector2 scrollPosition;

    // 드래그 선택을 위한 변수들
    private bool isDragging = false;
    private Vector2 dragStartPos;
    private Vector2 dragCurrentPos;
    private Rect dragRect;

    [MenuItem("Window/Custom/Grid Editor")]
    public static void ShowWindow()
    {
        GetWindow<GridEditorWindow>("Grid Editor");
    }

    private void OnEnable()
    {
        CreateGrid();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("그리드 설정", EditorStyles.boldLabel);
        gridWidth = EditorGUILayout.IntField("그리드 너비", gridWidth);
        gridHeight = EditorGUILayout.IntField("그리드 높이", gridHeight);
        cellSize = EditorGUILayout.FloatField("셀 크기 (월드 단위)", cellSize);
        cellSizeInEditor = EditorGUILayout.FloatField("셀 크기 (에디터 px)", cellSizeInEditor);

        // 그리드 시작 위치 입력 필드 추가
        gridOrigin = EditorGUILayout.Vector3Field("그리드 시작 위치", gridOrigin);

        if (GUILayout.Button("그리드 생성"))
        {
            CreateGrid();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("셀 상태 설정", EditorStyles.boldLabel);
        selectedState = (PLACEMENTSTATE)EditorGUILayout.EnumPopup("선택된 상태", selectedState);

        EditorGUILayout.Space();

        if (GUILayout.Button("씬에 그리드 적용"))
        {
            ApplyGridToScene();
        }

        EditorGUILayout.Space();

        // 그리드 표시 및 마우스 이벤트 처리
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        Rect gridRect = GUILayoutUtility.GetRect(gridWidth * cellSizeInEditor, gridHeight * cellSizeInEditor);

        // 마우스 이벤트 처리
        ProcessMouseEvents(Event.current, gridRect);

        DrawGrid(gridRect);

        EditorGUILayout.EndScrollView();

        // 드래그 영역 그리기
        if (isDragging)
        {
            DrawDragRectangle();
            Repaint();
        }
    }

    private void ProcessMouseEvents(Event e, Rect gridRect)
    {
        Vector2 localMousePosition = e.mousePosition;

        // 마우스가 그리드 영역 안에 있는지 확인
        if (!gridRect.Contains(localMousePosition))
        {
            return;
        }

        // 그리드 내에서의 마우스 위치 계산
        Vector2 mousePositionInGrid = localMousePosition - gridRect.position;

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            isDragging = true;
            dragStartPos = mousePositionInGrid;
            dragCurrentPos = mousePositionInGrid;
            e.Use();
        }

        if (e.type == EventType.MouseDrag && e.button == 0 && isDragging)
        {
            dragCurrentPos = mousePositionInGrid;
            e.Use();
        }

        if (e.type == EventType.MouseUp && e.button == 0 && isDragging)
        {
            isDragging = false;
            SelectCellsInDragArea();
            e.Use();
        }
    }

    private void DrawDragRectangle()
    {
        // 드래그 영역 계산
        dragRect = GetDragRect(dragStartPos, dragCurrentPos);

        // 드래그 영역을 그리기 위해 스크롤 위치를 고려
        Rect drawRect = new Rect(
            dragRect.x + scrollPosition.x,
            dragRect.y + scrollPosition.y + 150, // 150은 그리드 영역 위에 있는 GUI 요소들의 높이 합입니다. 필요에 따라 조정하세요.
            dragRect.width,
            dragRect.height
        );

        EditorGUI.DrawRect(drawRect, new Color(0, 0.5f, 1, 0.25f));
    }

    private Rect GetDragRect(Vector2 start, Vector2 end)
    {
        float x = Mathf.Min(start.x, end.x);
        float y = Mathf.Min(start.y, end.y);
        float width = Mathf.Abs(start.x - end.x);
        float height = Mathf.Abs(start.y - end.y);
        return new Rect(x, y, width, height);
    }

    private void SelectCellsInDragArea()
    {
        Rect selectionRect = GetDragRect(dragStartPos, dragCurrentPos);

        // 그리드 내의 각 셀에 대해 검사
        for (int y = gridHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Rect cellRect = new Rect(
                    x * cellSizeInEditor,
                    (gridHeight - 1 - y) * cellSizeInEditor,
                    cellSizeInEditor,
                    cellSizeInEditor);

                if (selectionRect.Overlaps(cellRect))
                {
                    GridCell cell = gridCells[x, y];
                    cell.PlaceState = selectedState;
                }
            }
        }

        Repaint();
    }

    private void CreateGrid()
    {
        gridCells = new GridCell[gridWidth, gridHeight];

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                // 월드 공간에서의 위치 계산
                Vector3 position = gridOrigin + new Vector3(x * cellSize, 0, y * cellSize);
                Coord coord = new Coord(x, y, position.x, position.z);
                gridCells[x, y] = new GridCell(position, coord);
            }
        }
    }

    private void DrawGrid(Rect gridRect)
    {
        if (gridCells == null)
            return;

        // 그리드 그리기
        for (int y = gridHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                GridCell cell = gridCells[x, y];
                Rect cellRect = new Rect(
                    gridRect.x + x * cellSizeInEditor,
                    gridRect.y + (gridHeight - 1 - y) * cellSizeInEditor,
                    cellSizeInEditor,
                    cellSizeInEditor);

                // 셀 배경 그리기
                EditorGUI.DrawRect(cellRect, GetCellColor(cell.PlaceState));

                // 셀 테두리 그리기
                Handles.color = Color.black;
                Handles.DrawLine(new Vector3(cellRect.x, cellRect.y), new Vector3(cellRect.x + cellRect.width, cellRect.y));
                Handles.DrawLine(new Vector3(cellRect.x, cellRect.y), new Vector3(cellRect.x, cellRect.y + cellRect.height));
                if (x == gridWidth - 1)
                {
                    Handles.DrawLine(new Vector3(cellRect.x + cellRect.width, cellRect.y), new Vector3(cellRect.x + cellRect.width, cellRect.y + cellRect.height));
                }
                if (y == 0)
                {
                    Handles.DrawLine(new Vector3(cellRect.x, cellRect.y + cellRect.height), new Vector3(cellRect.x + cellRect.width, cellRect.y + cellRect.height));
                }
            }
        }
    }

    private Color GetCellColor(PLACEMENTSTATE state)
    {
        switch (state)
        {
            case PLACEMENTSTATE.NONE:
                return Color.white;
            case PLACEMENTSTATE.PLACABLE:
                return Color.green;
            case PLACEMENTSTATE.IMPLACABLE:
                return Color.red;
            case PLACEMENTSTATE.MONSTERSPAWN:
                return Color.yellow;
            case PLACEMENTSTATE.ENDPOINT:
                return Color.magenta;
            case PLACEMENTSTATE.STARTPOINT:
                return Color.cyan;
            case PLACEMENTSTATE.HIDDEN:
                return new Color(1, 1, 1, 0.5f);
            default:
                return Color.white;
        }
    }

    private void ApplyGridToScene()
    {
        // 씬에서 GridManager를 찾거나 새로 생성
        GridManager gridManager = FindObjectOfType<GridManager>();
        if (gridManager == null)
        {
            GameObject gridManagerObject = new GameObject("GridManager");
            gridManager = gridManagerObject.AddComponent<GridManager>();
        }

        // 그리드 데이터 설정
        gridManager.gridWidth = gridWidth;
        gridManager.gridHeight = gridHeight;
        gridManager.cellSize = cellSize; // 실제 월드 공간에서의 셀 크기
        gridManager.gridOrigin = gridOrigin; // 에디터에서 입력받은 그리드 시작 위치

        // 셀 프리팹 설정 (프로젝트에 맞게 경로 수정)
        if (gridManager.cellPrefab == null)
        {
            gridManager.cellPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/YourCellPrefab.prefab");
        }

        // gridCells를 GridManager에 전달
        gridManager.gridCells = gridCells;

        // serializedGridCells 업데이트
        gridManager.serializedGridCells = new List<GridCell>();
        foreach (var cell in gridCells)
        {
            gridManager.serializedGridCells.Add(cell);
        }

        // gridCreatedInEditor 플래그 설정
        gridManager.gridCreatedInEditor = true;

        // 씬에 셀 생성
        gridManager.InstantiateCellsInScene();

        // 변경 사항 저장
        EditorUtility.SetDirty(gridManager);
    }
}
