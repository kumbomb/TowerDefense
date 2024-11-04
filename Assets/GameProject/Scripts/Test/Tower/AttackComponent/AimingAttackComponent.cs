using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System;
using UnityEngine;

public class AimingAttackComponent : MonoBehaviour, IAttackComponent
{
    [HideInInspector]
    public float attackSpeed;
    [HideInInspector]
    public float attackPower;
    public GameObject damageBallPrefab;
    public GameObject AimingPoint;
    public int attackAmount;
    public float damageBallTime;
    [HideInInspector]
    public bool isBulletFollow;
    public Transform shootPosition; // 데미지 볼 발사 위치
    public Transform bulletRoom; // 데미지 볼 발사 위치
    [HideInInspector]
    public float damageBallSpeed;
    [HideInInspector]
    public float colliderSize; // 데미지 탐색 범위 사이즈
    public List<EnemyBase> enemyList = new List<EnemyBase>();
    private CancellationTokenSource cancellationTokenSource;

    private Queue<DamageBallBase> damageBallPool; // 데미지 볼 풀
    private int poolSize = 20;

    void Start()
    {
        // 데미지 볼 풀 초기화
        InitializeDamageBallPool();
    }
    // 데미지 볼 풀을 초기화하고 5개를 미리 생성
    private void InitializeDamageBallPool()
    {
        damageBallPool = new Queue<DamageBallBase>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject damageBall = Instantiate(damageBallPrefab, bulletRoom);
            damageBall.SetActive(false); // 비활성화된 상태로 풀에 추가
            DamageBallBase ball = damageBall.GetComponent<DamageBallBase>();
            damageBallPool.Enqueue(ball);
        }
    }
    public void OnEnable()
    {
        cancellationTokenSource = new CancellationTokenSource();
        EnemySearching(cancellationTokenSource.Token).Forget();
    }
    public void OnDisable()
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
    }

    public void Update() // 완전 임시. 구조 나오면 바꿈
    {
        for (int i = enemyList.Count - 1; i >= 0; i--)
        {
            if (!enemyList[i].gameObject.activeInHierarchy)
            {
                enemyList.RemoveAt(i);
                Debug.Log("Enemy removed because it's inactive.");

            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) // 적 태그를 가진 오브젝트와의 충돌을 감지
        {
            if(EnemyBaseManager.Instance.enemyDic.TryGetValue(other.gameObject, out EnemyBase enemy))
            {
                enemyList.Add(enemy);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy")) // 적 태그를 가진 오브젝트가 범위를 벗어남을 감지
        {
            if (EnemyBaseManager.Instance.enemyDic.TryGetValue(other.gameObject, out EnemyBase enemy))
            {
                if (enemyList.Contains(enemy))
                {
                    enemyList.Remove(enemy);
                }
            }
            
        }
    }
    private async UniTaskVoid EnemySearching(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (EnemyBaseManager.Instance.enemyDic == null || enemyList == null || enemyList.Count <= 0)
                {
                    await UniTask.Yield();
                    continue;
                }
                if (EnemyBaseManager.Instance.enemyDic.TryGetValue(enemyList[0].gameObject, out EnemyBase enemy_1))
                {
                    for (int i = 0; i < attackAmount; i++)
                    {
                        if (enemyList.Count <= i)
                        {
                            if (AimingPoint != null)
                            {
                                Vector3 aim = enemy_1.transform.position;
                                aim.y = AimingPoint.transform.position.y;
                                AimingPoint.transform.LookAt(aim);
                            }
                            await UniTask.WhenAll(Shoot(enemy_1)); // 적을 찾아서 발사
                        }
                        else if (EnemyBaseManager.Instance.enemyDic.TryGetValue(enemyList[i].gameObject, out EnemyBase enemy))
                        {
                            if (AimingPoint != null)
                            {
                                Vector3 aim = enemy.transform.position;
                                aim.y = AimingPoint.transform.position.y;
                                AimingPoint.transform.LookAt(aim);
                            }
                            await UniTask.WhenAll(Shoot(enemy)); // 적을 찾아서 발사
                        }
                        await UniTask.Delay(100);
                    }
                    await UniTask.Delay((int)(attackSpeed * 1000)); // fireRate 초 만큼 대기
                }
                await UniTask.Yield(); // 다음 프레임까지 대기
            }
        }
        catch (OperationCanceledException)
        {

        }
    }
    private async UniTask Shoot(EnemyBase targetEnemy)
    {
        if (targetEnemy == null || !targetEnemy.gameObject.activeSelf) { return; }
        DamageBallBase damageBall = GetDamageBallFromPool();

        if (damageBall != null)
        {
            damageBall.transform.position = shootPosition.position;
            damageBall.damage = GetTotalAttackPower();
            damageBall.gameObject.SetActive(true); // 데미지 볼 활성화
            damageBall.speed = damageBallSpeed;
            if (colliderSize > 0) damageBall.lifeTime = (float)colliderSize / damageBallSpeed;
            else damageBall.lifeTime = damageBallTime;
            // 데미지 볼에 타겟 적을 설정
            damageBall.Initialize(targetEnemy, enemyList);
        }
        await UniTask.Yield();
    }
    private DamageBallBase GetDamageBallFromPool()
    {
        // 큐에서 총알을 순차적으로 꺼내온 후
        DamageBallBase damageBall = damageBallPool.Dequeue();

        // 활성화되지 않았으면 사용하고 다시 큐의 뒤로 넣음
        if (!damageBall.gameObject.activeInHierarchy)
        {
            damageBallPool.Enqueue(damageBall);
            return damageBall;
        }

        // 모든 오브젝트가 활성화된 상태라면 null 반환
        damageBallPool.Enqueue(damageBall);
        return null;
    }
    public void Attack(List<EnemyBase> enemyList)
    {

    }

    public float GetTotalAttackPower() // 나중에 데미지 계산식 대입 예정
    {
        return attackPower;
    }

    public void RemoveTarget(EnemyBase enemy)
    {
        throw new System.NotImplementedException();
    }

    public void SetTarget(EnemyBase enemy)
    {
        throw new System.NotImplementedException();
    }
}
