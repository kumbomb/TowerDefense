//using UnityEngine;
//using UnityEditor;
//using System.Collections.Generic;
//using BaseEnum;

//[CustomEditor(typeof(GridManager))]
//public class GridManagerEditor : Editor
//{
//    private bool isSelecting = false;
//    private Vector3 dragStartPos, dragEndPos;
//    private PLACEMENTSTATE selectedState = PLACEMENTSTATE.NONE;

//    public override void OnInspectorGUI()
//    {
//        // 기본 인스펙터 드로우
//        DrawDefaultInspector();

//        GridManager gridManager = (GridManager)target;

//        // 그리드 생성 버튼
//        if (GUILayout.Button("Generate Grid"))
//        {
//            gridManager.CreateGrid();
//            gridManager.gridCreatedInEditor = true;  // 에디터에서 그리드를 만들었다는 플래그 설정
//        }

//        // 그리드 제거 버튼
//        if (GUILayout.Button("Clear Grid"))
//        {
//            gridManager.ClearGrid();
//            gridManager.gridCreatedInEditor = false;  // 에디터에서 그리드를 삭제
//        }

//        // 셀 상태를 변경할 수 있는 옵션
//        GUILayout.Label("Change Selected Cells State:");
//        selectedState = (PLACEMENTSTATE)EditorGUILayout.EnumPopup("Select State", selectedState);

//        // 셀 선택 및 상태 업데이트
//        if (GUILayout.Button("Select Cells"))
//        {
//            isSelecting = true; // 셀 선택 모드 시작
//        }

//        if (isSelecting)
//        {
//            if (GUILayout.Button("Finish Selection"))
//            {
//                isSelecting = false;
//                gridManager.UpdateSelectedCells(gridManager.selectedCells, selectedState); // 선택된 셀들 업데이트
//                gridManager.ClearSelectedCells();
//            }
//        }

//        // 셀 선택 초기화 버튼
//        if (GUILayout.Button("Clear Selected Cells"))
//        {
//            gridManager.ClearSelectedCells();
//        }
//    }

//    void OnSceneGUI()
//    {
//        GridManager gridManager = (GridManager)target;

//        if (isSelecting)
//        {
//            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

//            Event e = Event.current;

//            if (e.type == EventType.MouseDown && e.button == 0)
//            {
//                // 드래그 시작 지점 설정
//                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
//                RaycastHit hit;

//                if (Physics.Raycast(ray, out hit))
//                {
//                    dragStartPos = hit.point;
//                }
//            }

//            if (e.type == EventType.MouseDrag && e.button == 0)
//            {
//                // 드래그 끝 지점 업데이트
//                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
//                RaycastHit hit;

//                if (Physics.Raycast(ray, out hit))
//                {
//                    dragEndPos = hit.point;
//                    gridManager.SelectCellsInArea(dragStartPos, dragEndPos);
//                }
//                e.Use();
//            }
//        }
//    }
//}
