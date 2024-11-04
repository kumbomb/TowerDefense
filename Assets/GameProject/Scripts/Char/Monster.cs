using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Character
{
    public GridCell currentCell;
    public float moveSpeed = 1f;
    private List<GridCell> path;
    private int targetIndex;
    GridCell goalCell = null;
    GridManager gridManager;

    public Transform visualChild; // Inspector에서 할당할 몬스터 시각적 자식 오브젝트
    public Animator anim;
    void Start()
    {
        gridManager = GridManager.Instance;
        if (gridManager == null)
        {
            Debug.LogError("GridManager.Instance가 null입니다. GridManager가 씬에 존재하는지 확인하세요.");
        }
        if (visualChild == null)
        {
            Debug.LogError("VisualChild가 할당되지 않았습니다. Inspector에서 몬스터 시각적 자식 오브젝트를 할당하세요.");
        }

        currentCell = gridManager.GetCellFromWorldPosition(transform.position);
        gridManager.OnGridChanged += UpdatePath;

        path = gridManager.FindPath(currentCell, goalCell);

        if (path != null && path.Count > 1)
        {
            targetIndex = 1;
            StartCoroutine(FollowPath());
        }
    }

    IEnumerator FollowPath()
    {
        while (path != null && targetIndex < path.Count)
        {
            anim.SetTrigger("Run");
            GridCell targetCell = path[targetIndex];
            Vector3 targetPosition = targetCell.position;

            // 이동 방향을 계산하고 시각적 자식 오브젝트를 회전시킵니다.
            Vector3 direction = (targetPosition - transform.position).normalized;

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                if (direction != Vector3.zero)
                {
                    RotateVisual(direction);
                }
                yield return null;
            }

            currentCell = targetCell;
            targetIndex++;
        }

        if (path != null && targetIndex >= path.Count)
        {
            // 몬스터가 목표 지점에 도달했을 때의 처리
            //Debug.Log("몬스터가 목표 지점에 도달했습니다!");
            DestroyGameobject();
            yield break;
        }
    }

    void OnDestroy()
    {
        GridManager gridManager = GridManager.Instance;
        if (gridManager != null)
        {
            gridManager.OnGridChanged -= UpdatePath;
        }
    }

    public void UpdatePath()
    {
        GridManager gridManager = GridManager.Instance;
        path = gridManager.FindPath(currentCell, goalCell);
        targetIndex = 1;

        StopAllCoroutines();
        if (path != null && path.Count > 1)
        {
            StartCoroutine(FollowPath());
        }
    }

    public void SetGoalCell(GridCell _goalCell)
    {
        goalCell = _goalCell;
    }

    void DestroyGameobject()
    {
        OnDestroy();
        Destroy(gameObject);
    }

    /// <summary>
    /// 시각적 자식 오브젝트를 이동 방향으로 회전시킵니다.
    /// </summary>
    /// <param name="direction">이동 방향 벡터</param>
    void RotateVisual(Vector3 direction)
    {
        if (visualChild == null)
            return;

        // Y축을 기준으로 회전 (수직 회전)
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        visualChild.rotation = Quaternion.Lerp(visualChild.rotation, targetRotation, Time.deltaTime * 10f);
    }
}
