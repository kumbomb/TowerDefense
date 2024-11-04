using BaseEnum;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// DragAndDropManager는 UI 버튼을 통해 타워를 드래그 앤 드롭하는 기능을 관리하는 매니저 클래스입니다.
/// 싱글톤 패턴을 사용하여 전역적으로 접근 가능합니다.
/// </summary>
public class DragAndDropManager : Manager<DragAndDropManager>
{
    public Camera mainCamera; // 메인 카메라
    public LayerMask groundLayer; // 배치 가능한 바닥 레이어
    public Image dragPreviewImage; // 드래그 중에 표시할 프리뷰 이미지 (Optional)
    private GameObject previewObject; // 드래그 중인 프리뷰 오브젝트
    private bool isDragging = false;
    private TowerType currentTowerType; // 현재 드래그 중인 타워 타입
    public GameObject rangeIndicatorPrefab; // 범위 표시용 프리팹
    private GameObject rangeIndicatorInstance; // 범위 표시용 인스턴스
    private float towerRange = 1f; // 기본 배치 반경 (타워의 범위)

    void Update()
    {
        if (isDragging)
        {
            UpdateDragPosition();

            // 마우스 버튼을 놓으면 드래그 종료
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                EndDrag();
            }

            // Esc 키를 누르면 드래그 취소
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                CancelDrag();
            }
        }
    }

    public void StartDrag(TowerType type)
    {
        if (isDragging)
            return;

        currentTowerType = type;

        // 오브젝트 풀에서 타워 가져오기
        previewObject = TowerObjectPool.Instance.GetTower(currentTowerType);
        if (previewObject == null)
        {
            Debug.LogError($"타워 '{currentTowerType}'을(를) 풀에서 가져올 수 없습니다.");
            return;
        }

        // 프리뷰 오브젝트 설정
        previewObject.transform.position = GetMouseWorldPosition();
        previewObject.transform.rotation = Quaternion.identity;
        previewObject.SetActive(true);

        // 범위 표시 오브젝트 생성
        rangeIndicatorInstance = Instantiate(rangeIndicatorPrefab);
        rangeIndicatorInstance.transform.position = previewObject.transform.position;

        // 프리팹의 크기를 자동으로 설정
        SetRangeIndicatorSize(rangeIndicatorInstance, towerRange);
        rangeIndicatorInstance.SetActive(true);

        isDragging = true;
    }
    private void SetRangeIndicatorSize(GameObject rangeIndicator, float range)
    {
        // 원래 사이즈에서 타워의 범위에 맞춰 스케일 조정
        rangeIndicator.transform.localScale = new Vector3(range * 2, 1, range * 2);
    }
    private void UpdateDragPosition()
    {
        Vector3 mousePos = GetMouseWorldPosition();
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            previewObject.transform.position = hit.point;

            // 범위 표시 오브젝트의 위치도 업데이트
            if (rangeIndicatorInstance != null)
            {
                rangeIndicatorInstance.transform.position = hit.point;
            }

            // 배치 유효성 검사
            if (IsValidPlacement(hit.point))
            {
                SetObjectColor(previewObject, Color.green); // 배치 가능 - 녹색
                SetObjectColor(rangeIndicatorInstance, new Color(0, 1, 0, 0.5f)); // 반투명 녹색 범위
            }
            else
            {
                SetObjectColor(previewObject, Color.red); // 배치 불가능 - 빨간색
                SetObjectColor(rangeIndicatorInstance, new Color(1, 0, 0, 0.5f)); // 반투명 빨간색 범위
            }
        }
    }

    private void EndDrag()
    {
        if (previewObject == null)
            return;

        Vector3 placementPosition = previewObject.transform.position;

        if (IsValidPlacement(placementPosition))
        {
            previewObject.transform.position = placementPosition;
            previewObject.transform.rotation = Quaternion.identity;
            previewObject.SetActive(true);
            Debug.Log($"{currentTowerType} 타워를 소환했습니다.");
        }
        else
        {
            TowerObjectPool.Instance.ReturnTower(currentTowerType, previewObject);
            Debug.LogWarning("유효하지 않은 위치에 타워를 소환할 수 없습니다.");
        }

        // 범위 표시 오브젝트 삭제
        if (rangeIndicatorInstance != null)
        {
            Destroy(rangeIndicatorInstance);
            rangeIndicatorInstance = null;
        }

        isDragging = false;
    }

    private void CancelDrag()
    {
        if (previewObject != null)
        {
            TowerObjectPool.Instance.ReturnTower(currentTowerType, previewObject);
            previewObject = null;
        }

        if (dragPreviewImage != null)
        {
            dragPreviewImage.enabled = false;
        }

        isDragging = false;
    }
    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            return hit.point;
        }

        return Vector3.zero;
    }
    private void SetObjectTransparency(GameObject obj, float alpha)
    {
        // 투명도 설정 부분을 생략하고, 오브젝트는 기본 상태 그대로 표시
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material mat in renderer.materials)
            {
                // 투명도 설정을 생략하므로 여기서는 아무것도 하지 않습니다.
                // 단순히 오브젝트가 드래그 중에 보이게만 설정하면 됩니다.
                Debug.Log($"Material '{mat.name}' is set without transparency changes.");
            }
        }
    }
    private void SetObjectColor(GameObject obj, Color color)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material mat in renderer.materials)
            {
                mat.color = color; // 색상 변경
            }
        }
    }

    private bool IsValidPlacement(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 1f, groundLayer);
        return colliders.Length == 1;
    }
}
