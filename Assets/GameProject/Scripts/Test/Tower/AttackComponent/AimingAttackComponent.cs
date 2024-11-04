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
    public Transform shootPosition; // ������ �� �߻� ��ġ
    public Transform bulletRoom; // ������ �� �߻� ��ġ
    [HideInInspector]
    public float damageBallSpeed;
    [HideInInspector]
    public float colliderSize; // ������ Ž�� ���� ������
    public List<EnemyBase> enemyList = new List<EnemyBase>();
    private CancellationTokenSource cancellationTokenSource;

    private Queue<DamageBallBase> damageBallPool; // ������ �� Ǯ
    private int poolSize = 20;

    void Start()
    {
        // ������ �� Ǯ �ʱ�ȭ
        InitializeDamageBallPool();
    }
    // ������ �� Ǯ�� �ʱ�ȭ�ϰ� 5���� �̸� ����
    private void InitializeDamageBallPool()
    {
        damageBallPool = new Queue<DamageBallBase>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject damageBall = Instantiate(damageBallPrefab, bulletRoom);
            damageBall.SetActive(false); // ��Ȱ��ȭ�� ���·� Ǯ�� �߰�
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

    public void Update() // ���� �ӽ�. ���� ������ �ٲ�
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
        if (other.CompareTag("Enemy")) // �� �±׸� ���� ������Ʈ���� �浹�� ����
        {
            if(EnemyBaseManager.Instance.enemyDic.TryGetValue(other.gameObject, out EnemyBase enemy))
            {
                enemyList.Add(enemy);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy")) // �� �±׸� ���� ������Ʈ�� ������ ����� ����
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
                            await UniTask.WhenAll(Shoot(enemy_1)); // ���� ã�Ƽ� �߻�
                        }
                        else if (EnemyBaseManager.Instance.enemyDic.TryGetValue(enemyList[i].gameObject, out EnemyBase enemy))
                        {
                            if (AimingPoint != null)
                            {
                                Vector3 aim = enemy.transform.position;
                                aim.y = AimingPoint.transform.position.y;
                                AimingPoint.transform.LookAt(aim);
                            }
                            await UniTask.WhenAll(Shoot(enemy)); // ���� ã�Ƽ� �߻�
                        }
                        await UniTask.Delay(100);
                    }
                    await UniTask.Delay((int)(attackSpeed * 1000)); // fireRate �� ��ŭ ���
                }
                await UniTask.Yield(); // ���� �����ӱ��� ���
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
            damageBall.gameObject.SetActive(true); // ������ �� Ȱ��ȭ
            damageBall.speed = damageBallSpeed;
            if (colliderSize > 0) damageBall.lifeTime = (float)colliderSize / damageBallSpeed;
            else damageBall.lifeTime = damageBallTime;
            // ������ ���� Ÿ�� ���� ����
            damageBall.Initialize(targetEnemy, enemyList);
        }
        await UniTask.Yield();
    }
    private DamageBallBase GetDamageBallFromPool()
    {
        // ť���� �Ѿ��� ���������� ������ ��
        DamageBallBase damageBall = damageBallPool.Dequeue();

        // Ȱ��ȭ���� �ʾ����� ����ϰ� �ٽ� ť�� �ڷ� ����
        if (!damageBall.gameObject.activeInHierarchy)
        {
            damageBallPool.Enqueue(damageBall);
            return damageBall;
        }

        // ��� ������Ʈ�� Ȱ��ȭ�� ���¶�� null ��ȯ
        damageBallPool.Enqueue(damageBall);
        return null;
    }
    public void Attack(List<EnemyBase> enemyList)
    {

    }

    public float GetTotalAttackPower() // ���߿� ������ ���� ���� ����
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
