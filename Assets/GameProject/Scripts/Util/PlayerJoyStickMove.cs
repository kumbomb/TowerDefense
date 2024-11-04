using UnityEngine;
using UnityEngine.EventSystems;
using System;

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
    private Camera uiCam;

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
        uiCam = uiCanvas.worldCamera;

        if (uiCam == null)
        {
            uiCam = Camera.main;
        }

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
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            areaTransform,
            eventData.position,
            uiCam,
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
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            areaTransform,
            eventData.position,
            uiCam,
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

        // 카메라 회전에 맞게 입력 벡터 조정
        Vector3 worldDirection = Camera.main.transform.TransformDirection(new Vector3(inputVector.x, 0, inputVector.y));
        Vector2 adjustedInput = new Vector2(worldDirection.x, worldDirection.z).normalized;

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
