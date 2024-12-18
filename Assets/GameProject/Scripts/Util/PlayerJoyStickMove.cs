﻿using UnityEngine;
using UnityEngine.EventSystems;
using System;

#region 기존
// public class PlayerJoyStickMove : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
// {
//     [Header("UI Components")]
//     [SerializeField] private RectTransform lever;      // 조이스틱 레버
//     [SerializeField] private GameObject stickParent;  // 조이스틱 부모 오브젝트
//     [SerializeField] private Canvas uiCanvas;         // UI 캔버스

//     [Header("Settings")]
//     [SerializeField, Range(10f, 150f)] private float leverRange = 100f; // 레버 이동 범위

//     private RectTransform rectTransform;
//     private RectTransform areaTransform;
//     private Camera uiCam;

//     // 입력 벡터
//     private Vector2 inputVector;

//     // 입력 상태
//     private bool isInputActive = false;

//     // 이동 입력을 위한 이벤트
//     public event Action<Vector2> OnMoveInput;

//     private void Awake()
//     {
//         areaTransform = GetComponent<RectTransform>();
//         rectTransform = stickParent.GetComponent<RectTransform>();
//         uiCam = uiCanvas.worldCamera;

//         if (uiCam == null)
//         {
//             uiCam = Camera.main;
//         }

//         // 초기 상태 설정
//         stickParent.SetActive(false);
//         lever.anchoredPosition = Vector2.zero;
//     }

//     public void OnBeginDrag(PointerEventData eventData)
//     {
//         if (!CanMove())
//         {
//             isInputActive = false;
//             return;
//         }

//         if (!stickParent.activeSelf)
//             stickParent.SetActive(true);

//         Vector2 clickPos;
//         RectTransformUtility.ScreenPointToLocalPointInRectangle(
//             areaTransform,
//             eventData.position,
//             uiCam,
//             out clickPos
//         );

//         rectTransform.anchoredPosition = clickPos;

//         isInputActive = true;
//         UpdateJoystick(eventData);
//     }

//     public void OnDrag(PointerEventData eventData)
//     {
//         if (!isInputActive || !CanMove())
//         {
//             EndInput();
//             return;
//         }

//         UpdateJoystick(eventData);
//     }

//     public void OnEndDrag(PointerEventData eventData)
//     {
//         EndInput();
//     }

//     private void UpdateJoystick(PointerEventData eventData)
//     {
//         Vector2 clickPos;
//         if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
//             areaTransform,
//             eventData.position,
//             uiCam,
//             out clickPos))
//         {
//             return;
//         }

//         var inputDir = clickPos - rectTransform.anchoredPosition;
//         var clampedDir = inputDir.magnitude < leverRange ? inputDir : inputDir.normalized * leverRange;
//         // 레버 위치 업데이트
//         lever.anchoredPosition = clampedDir;

//         // 정규화된 입력 벡터 계산
//         inputVector = clampedDir / leverRange;

//         // 카메라 회전에 맞게 입력 벡터 조정
//         Vector3 worldDirection = Camera.main.transform.TransformDirection(new Vector3(inputVector.x, 0, inputVector.y));
//         Vector2 adjustedInput = new Vector2(worldDirection.x, worldDirection.z).normalized;

//         // 이동 입력 이벤트 호출
//         OnMoveInput?.Invoke(adjustedInput);
//     }

//     private void EndInput()
//     {
//         isInputActive = false;
//         stickParent.SetActive(false);
//         lever.anchoredPosition = Vector2.zero;

//         // 이동 중지 알림
//         OnMoveInput?.Invoke(Vector2.zero);
//     }

//     private bool CanMove()
//     {
//         // 이동 가능한 조건을 여기에 정의 (필요 시 수정)
//         // 예: 게임이 일시정지 상태가 아닌지, UI가 열려있지 않은지 등
//         return true;
//     }
// }

#endregion 

public class PlayerJoyStickMove : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI Components")]
    [SerializeField] private RectTransform lever;      // 조이스틱 레버
    [SerializeField] private GameObject stickParent;  // 조이스틱 부모 오브젝트
    [SerializeField] private Canvas uiCanvas;         // UI 캔버스

    [Header("Settings")]
    [SerializeField, Range(10f, 150f)] private float leverRange = 100f; // 레버 이동 범위

    private RectTransform rectTransform;
    private RectTransform areaTransform;
    // Screen Space - Overlay에서는 카메라 참조가 필요 없으므로 uiCam을 제거합니다.

    // 입력 벡터
    private Vector2 inputVector;

    // 입력 상태
    private bool isInputActive = false;

    // 이동 입력을 위한 이벤트
    public event Action<Vector2> OnMoveInput;

    private void Awake()
    {
        areaTransform = GetComponent<RectTransform>();
        rectTransform = stickParent.GetComponent<RectTransform>();
        // Screen Space - Overlay에서는 uiCam을 사용하지 않으므로 제거합니다.
        // uiCam = uiCanvas.worldCamera;

        // 초기 상태 설정
        stickParent.SetActive(false);
        lever.anchoredPosition = Vector2.zero;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!CanMove())
        {
            isInputActive = false;
            return;
        }

        if (!stickParent.activeSelf)
            stickParent.SetActive(true);

        Vector2 clickPos;
        // Screen Space - Overlay에서는 카메라가 없으므로 uiCam을 null로 전달합니다.
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            areaTransform,
            eventData.position,
            null, // uiCam을 null로 설정
            out clickPos
        );

        rectTransform.anchoredPosition = clickPos;

        isInputActive = true;
        UpdateJoystick(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isInputActive || !CanMove())
        {
            EndInput();
            return;
        }

        UpdateJoystick(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EndInput();
    }

    private void UpdateJoystick(PointerEventData eventData)
    {
        Vector2 clickPos;
        // Screen Space - Overlay에서는 카메라가 없으므로 uiCam을 null로 전달합니다.
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            areaTransform,
            eventData.position,
            null, // uiCam을 null로 설정
            out clickPos))
        {
            return;
        }

        var inputDir = clickPos - rectTransform.anchoredPosition;
        var clampedDir = inputDir.magnitude < leverRange ? inputDir : inputDir.normalized * leverRange;
        // 레버 위치 업데이트
        lever.anchoredPosition = clampedDir;

        // 정규화된 입력 벡터 계산
        inputVector = clampedDir / leverRange;

        // Camera 회전에 의존하지 않고 입력을 처리합니다.
        // 기존 코드:
        // Vector3 worldDirection = Camera.main.transform.TransformDirection(new Vector3(inputVector.x, 0, inputVector.y));
        // Vector2 adjustedInput = new Vector2(worldDirection.x, worldDirection.z).normalized;

        // 수정된 코드: 카메라 회전과 무관하게 입력 벡터를 직접 사용합니다.
        Vector2 adjustedInput = inputVector.normalized;

        // 이동 입력 이벤트 호출
        OnMoveInput?.Invoke(adjustedInput);
    }

    private void EndInput()
    {
        isInputActive = false;
        stickParent.SetActive(false);
        lever.anchoredPosition = Vector2.zero;

        // 이동 중지 알림
        OnMoveInput?.Invoke(Vector2.zero);
    }

    private bool CanMove()
    {
        // 이동 가능한 조건을 여기에 정의 (필요 시 수정)
        // 예: 게임이 일시정지 상태가 아닌지, UI가 열려있지 않은지 등
        return true;
    }
}