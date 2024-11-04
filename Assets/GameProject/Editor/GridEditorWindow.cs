using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using BaseEnum;
using BaseStruct;

public class GridEditorWindow : EditorWindow
{
    // ���� ������
    private int gridWidth = 10;
    private int gridHeight = 10;
    private float cellSize = 1f; // ���� ���� ���������� �� ũ��
    private float cellSizeInEditor = 32f; // ������ �����쿡���� �� ũ�� (�ȼ� ����)

    // �׸��� ���� ��ġ ���� �߰�
    private Vector3 gridOrigin = Vector3.zero;

    private GridCell[,] gridCells;
    private PLACEMENTSTATE selectedState = PLACEMENTSTATE.NONE;
    private Vector2 scrollPosition;

    // �巡�� ������ ���� ������
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
        EditorGUILayout.LabelField("�׸��� ����", EditorStyles.boldLabel);
        gridWidth = EditorGUILayout.IntField("�׸��� �ʺ�", gridWidth);
        gridHeight = EditorGUILayout.IntField("�׸��� ����", gridHeight);
        cellSize = EditorGUILayout.FloatField("�� ũ�� (���� ����)", cellSize);
        cellSizeInEditor = EditorGUILayout.FloatField("�� ũ�� (������ px)", cellSizeInEditor);

        // �׸��� ���� ��ġ �Է� �ʵ� �߰�
        gridOrigin = EditorGUILayout.Vector3Field("�׸��� ���� ��ġ", gridOrigin);

        if (GUILayout.Button("�׸��� ����"))
        {
            CreateGrid();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("�� ���� ����", EditorStyles.boldLabel);
        selectedState = (PLACEMENTSTATE)EditorGUILayout.EnumPopup("���õ� ����", selectedState);

        EditorGUILayout.Space();

        if (GUILayout.Button("���� �׸��� ����"))
        {
            ApplyGridToScene();
        }

        EditorGUILayout.Space();

        // �׸��� ǥ�� �� ���콺 �̺�Ʈ ó��
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        Rect gridRect = GUILayoutUtility.GetRect(gridWidth * cellSizeInEditor, gridHeight * cellSizeInEditor);

        // ���콺 �̺�Ʈ ó��
        ProcessMouseEvents(Event.current, gridRect);

        DrawGrid(gridRect);

        EditorGUILayout.EndScrollView();

        // �巡�� ���� �׸���
        if (isDragging)
        {
            DrawDragRectangle();
            Repaint();
        }
    }

    private void ProcessMouseEvents(Event e, Rect gridRect)
    {
        Vector2 localMousePosition = e.mousePosition;

        // ���콺�� �׸��� ���� �ȿ� �ִ��� Ȯ��
        if (!gridRect.Contains(localMousePosition))
        {
            return;
        }

        // �׸��� �������� ���콺 ��ġ ���
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
        // �巡�� ���� ���
        dragRect = GetDragRect(dragStartPos, dragCurrentPos);

        // �巡�� ������ �׸��� ���� ��ũ�� ��ġ�� ���
        Rect drawRect = new Rect(
            dragRect.x + scrollPosition.x,
            dragRect.y + scrollPosition.y + 150, // 150�� �׸��� ���� ���� �ִ� GUI ��ҵ��� ���� ���Դϴ�. �ʿ信 ���� �����ϼ���.
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

        // �׸��� ���� �� ���� ���� �˻�
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
                // ���� ���������� ��ġ ���
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

        // �׸��� �׸���
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

                // �� ��� �׸���
                EditorGUI.DrawRect(cellRect, GetCellColor(cell.PlaceState));

                // �� �׵θ� �׸���
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
        // ������ GridManager�� ã�ų� ���� ����
        GridManager gridManager = FindObjectOfType<GridManager>();
        if (gridManager == null)
        {
            GameObject gridManagerObject = new GameObject("GridManager");
            gridManager = gridManagerObject.AddComponent<GridManager>();
        }

        // �׸��� ������ ����
        gridManager.gridWidth = gridWidth;
        gridManager.gridHeight = gridHeight;
        gridManager.cellSize = cellSize; // ���� ���� ���������� �� ũ��
        gridManager.gridOrigin = gridOrigin; // �����Ϳ��� �Է¹��� �׸��� ���� ��ġ

        // �� ������ ���� (������Ʈ�� �°� ��� ����)
        if (gridManager.cellPrefab == null)
        {
            gridManager.cellPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/YourCellPrefab.prefab");
        }

        // gridCells�� GridManager�� ����
        gridManager.gridCells = gridCells;

        // serializedGridCells ������Ʈ
        gridManager.serializedGridCells = new List<GridCell>();
        foreach (var cell in gridCells)
        {
            gridManager.serializedGridCells.Add(cell);
        }

        // gridCreatedInEditor �÷��� ����
        gridManager.gridCreatedInEditor = true;

        // ���� �� ����
        gridManager.InstantiateCellsInScene();

        // ���� ���� ����
        EditorUtility.SetDirty(gridManager);
    }
}
