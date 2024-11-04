using BaseEnum;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// DragAndDropManager�� UI ��ư�� ���� Ÿ���� �巡�� �� ����ϴ� ����� �����ϴ� �Ŵ��� Ŭ�����Դϴ�.
/// �̱��� ������ ����Ͽ� ���������� ���� �����մϴ�.
/// </summary>
public class DragAndDropManager : Manager<DragAndDropManager>
{
    public Camera mainCamera; // ���� ī�޶�
    public LayerMask groundLayer; // ��ġ ������ �ٴ� ���̾�
    public Image dragPreviewImage; // �巡�� �߿� ǥ���� ������ �̹��� (Optional)
    private GameObject previewObject; // �巡�� ���� ������ ������Ʈ
    private bool isDragging = false;
    private TowerType currentTowerType; // ���� �巡�� ���� Ÿ�� Ÿ��
    public GameObject rangeIndicatorPrefab; // ���� ǥ�ÿ� ������
    private GameObject rangeIndicatorInstance; // ���� ǥ�ÿ� �ν��Ͻ�
    private float towerRange = 1f; // �⺻ ��ġ �ݰ� (Ÿ���� ����)

    void Update()
    {
        if (isDragging)
        {
            UpdateDragPosition();

            // ���콺 ��ư�� ������ �巡�� ����
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                EndDrag();
            }

            // Esc Ű�� ������ �巡�� ���
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

        // ������Ʈ Ǯ���� Ÿ�� ��������
        previewObject = TowerObjectPool.Instance.GetTower(currentTowerType);
        if (previewObject == null)
        {
            Debug.LogError($"Ÿ�� '{currentTowerType}'��(��) Ǯ���� ������ �� �����ϴ�.");
            return;
        }

        // ������ ������Ʈ ����
        previewObject.transform.position = GetMouseWorldPosition();
        previewObject.transform.rotation = Quaternion.identity;
        previewObject.SetActive(true);

        // ���� ǥ�� ������Ʈ ����
        rangeIndicatorInstance = Instantiate(rangeIndicatorPrefab);
        rangeIndicatorInstance.transform.position = previewObject.transform.position;

        // �������� ũ�⸦ �ڵ����� ����
        SetRangeIndicatorSize(rangeIndicatorInstance, towerRange);
        rangeIndicatorInstance.SetActive(true);

        isDragging = true;
    }
    private void SetRangeIndicatorSize(GameObject rangeIndicator, float range)
    {
        // ���� ������� Ÿ���� ������ ���� ������ ����
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

            // ���� ǥ�� ������Ʈ�� ��ġ�� ������Ʈ
            if (rangeIndicatorInstance != null)
            {
                rangeIndicatorInstance.transform.position = hit.point;
            }

            // ��ġ ��ȿ�� �˻�
            if (IsValidPlacement(hit.point))
            {
                SetObjectColor(previewObject, Color.green); // ��ġ ���� - ���
                SetObjectColor(rangeIndicatorInstance, new Color(0, 1, 0, 0.5f)); // ������ ��� ����
            }
            else
            {
                SetObjectColor(previewObject, Color.red); // ��ġ �Ұ��� - ������
                SetObjectColor(rangeIndicatorInstance, new Color(1, 0, 0, 0.5f)); // ������ ������ ����
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
            Debug.Log($"{currentTowerType} Ÿ���� ��ȯ�߽��ϴ�.");
        }
        else
        {
            TowerObjectPool.Instance.ReturnTower(currentTowerType, previewObject);
            Debug.LogWarning("��ȿ���� ���� ��ġ�� Ÿ���� ��ȯ�� �� �����ϴ�.");
        }

        // ���� ǥ�� ������Ʈ ����
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
        // ���� ���� �κ��� �����ϰ�, ������Ʈ�� �⺻ ���� �״�� ǥ��
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material mat in renderer.materials)
            {
                // ���� ������ �����ϹǷ� ���⼭�� �ƹ��͵� ���� �ʽ��ϴ�.
                // �ܼ��� ������Ʈ�� �巡�� �߿� ���̰Ը� �����ϸ� �˴ϴ�.
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
                mat.color = color; // ���� ����
            }
        }
    }

    private bool IsValidPlacement(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 1f, groundLayer);
        return colliders.Length == 1;
    }
}
