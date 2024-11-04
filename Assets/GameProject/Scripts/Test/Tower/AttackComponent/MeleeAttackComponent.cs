using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MeleeAttackComponent : MonoBehaviour, IAttackComponent
{
    [HideInInspector]
    public float attackSpeed;
    [HideInInspector]
    public float attackPower;
    public GameObject attackTool;
    public List<EnemyBase> enemyList = new List<EnemyBase>();
    private CancellationTokenSource cancellationTokenSource;


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
            enemyList.Add(EnemyBaseManager.Instance.enemyDic[other.gameObject]);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy")) // 적 태그를 가진 오브젝트가 범위를 벗어남을 감지
        {
            var enemy = EnemyBaseManager.Instance.enemyDic[other.gameObject];
            if (enemyList.Contains(enemy))
            {
                enemyList.Remove(enemy);
            }
        }
    }
    private async UniTaskVoid EnemySearching(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (enemyList.Count > 0)
                {
                    Attack(enemyList);
                    await UniTask.Delay((int)(attackSpeed * 1000));
                }
                await UniTask.Yield();
            }
        }
        catch (OperationCanceledException)
        {

        }
    }
    public void Attack(List<EnemyBase> enemyList)
    {
        EnemyBaseManager.Instance.TakeDamage(enemyList, GetTotalAttackPower());
        attackTool.SetActive(true);
        Invoke("SetAnim", 0.3f);
    }

    public float GetTotalAttackPower()
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
    public void SetAnim()
    {
        attackTool.SetActive(false);
    }
}
