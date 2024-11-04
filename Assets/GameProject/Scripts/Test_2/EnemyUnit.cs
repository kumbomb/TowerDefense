using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : BaseUnit, IAttackable
{
    [Header("Enemy Settings")]
    public GridCell currentCell;
    public float attackRange = 3f;
    public float attackDamage = 10f;
    public float attackCooldown = 2f;
    private float lastAttackTime = -Mathf.Infinity;

    private List<GridCell> path;
    private int targetIndex;
    GridCell goalCell = null;
    GridManager gridManager;
    private Health healthComponent;

    public Transform visualChild; // Inspector에서 할당할 몬스터 시각적 자식 오브젝트
    public Animator anim;

    private Coroutine movementCoroutine;
    private bool isAttacking = false;
    private IDamageable currentTarget = null;

    //[Header("Movement Settings")]
    //public float moveSpeed = 2f; // 적절한 이동 속도로 설정

    protected void Awake()
    {
        healthComponent = GetComponent<Health>();
        if (healthComponent != null)
        {
            healthComponent.OnDeath += HandleDeath;
        }
    }

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
            movementCoroutine = StartCoroutine(FollowPath());
        }
    }

    IEnumerator FollowPath()
    {
        while (path != null && targetIndex < path.Count)
        {
            GridCell targetCell = path[targetIndex];
            Vector3 targetPosition = targetCell.position;

            Vector3 direction = (targetPosition - transform.position).normalized;

            //anim.SetBool("Run", true); // 애니메이션 상태 변경
            anim.SetTrigger("Run");

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                if (isAttacking)
                {
                    // 공격 상태일 때는 이동을 멈춤
                    break;
                }

                // 이동 속도를 고정하여 일관된 움직임
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

                if (direction != Vector3.zero)
                {
                    RotateVisual(direction);
                }

                yield return null;
            }

            //anim.SetBool("Run", false); // 애니메이션 상태 변경
            anim.SetTrigger("Run");

            if (!isAttacking)
            {
                currentCell = targetCell;
                targetIndex++;
            }
        }

        if (path != null && targetIndex >= path.Count)
        {
            // 몬스터가 목표 지점에 도달했을 때의 처리
            DestroyGameobject();
            yield break;
        }

        // 코루틴 종료 시 movementCoroutine을 null로 설정
        movementCoroutine = null;
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
        if (isAttacking) return; // 공격 중일 때는 경로 업데이트를 지연

        GridManager gridManager = GridManager.Instance;
        path = gridManager.FindPath(currentCell, goalCell);
        targetIndex = 1;

        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }

        if (path != null && path.Count > 1)
        {
            movementCoroutine = StartCoroutine(FollowPath());
        }
    }

    public void SetGoalCell(GridCell _goalCell)
    {
        goalCell = _goalCell;
    }

    void DestroyGameobject()
    {
        HandleDeath();
        Destroy(gameObject);
    }

    private bool DetectAndSetTarget()
    {
        // "Player" 레이어에 속하는 오브젝트만 감지
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Player"));

        Collider closestPlayer = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPlayer = collider;
                }
            }
        }

        if (closestPlayer != null)
        {
            IDamageable player = closestPlayer.GetComponent<IDamageable>();
            if (player != null)
            {
                currentTarget = player;
                return true;
            }
        }

        return false;
    }

    void Update()
    {
        if (isAttacking && currentTarget != null)
        {
            float distance = Vector3.Distance(transform.position, ((MonoBehaviour)currentTarget).transform.position);
            if (distance > attackRange)
            {
                // 타겟이 사정거리 밖으로 이동
                isAttacking = false;
                currentTarget = null;

                // 이동 코루틴 재개
                if (movementCoroutine != null)
                {
                    StopCoroutine(movementCoroutine);
                    movementCoroutine = null;
                }
                movementCoroutine = StartCoroutine(FollowPath());
            }
            else
            {
                // 공격 쿨타임이 지났으면 공격 수행
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    Attack(currentTarget);
                    lastAttackTime = Time.time;
                }
            }
        }
    }

    public void Attack(IDamageable target)
    {
        if (target != null)
        {
            target.TakeDamage(attackDamage);
            Debug.Log($"{gameObject.name} attacked {target} for {attackDamage} damage.");
            // 애니메이션을 트리거한 후 공격 완료
            anim.SetTrigger("Attack");
        }
    }

    private void HandleDeath()
    {
        isDead = true;
        // 추가 사망 처리 로직 (예: 점수 증가, 이펙트 재생 등)
        Debug.Log($"{gameObject.name} has been handled by MonsterManager.");
    }

    void RotateVisual(Vector3 direction)
    {
        if (visualChild == null)
            return;

        // Y축을 기준으로 회전 (수직 회전)
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        visualChild.rotation = Quaternion.Lerp(visualChild.rotation, targetRotation, Time.deltaTime * 10f);
    }

}
