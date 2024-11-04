using BaseEnum;
using System.Threading;
using System;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public GridCell currentCell;
    public EnemyType enemyType;
    public float moveSpeed = 5f; // �̵� �ӵ�
    public float enemyMaxHP = 10f;
    private float EnemyHp;
    public float enemyHp 
    {
        get { return EnemyHp; }
        set 
        {
            EnemyHp = value;
            if(EnemyHp <= 0)
            {
                Die();
            }
        }
    }
   // private CancellationTokenSource cts; // ��� ��ū �ҽ�

    private void OnEnable()
    {
        // ���� Ȱ��ȭ�Ǹ� �̵� ����
        //StartMovementAsync(-12f, moveSpeed).Forget();
        enemyHp = enemyMaxHP;
    }

    //private void OnDisable()
    //{
    //    // ���� ��Ȱ��ȭ�Ǹ� �̵� �ߴ�
    //    if (cts != null)
    //    {
    //        cts.Cancel();
    //        cts.Dispose();
    //    }
    //}

    // z������ ��ǥ ��ġ���� �̵��ϴ� �񵿱� �۾�
    //private async UniTaskVoid StartMovementAsync(float targetZ, float speed)
    //{
    //    // ���ο� CancellationTokenSource ����
    //    cts = new CancellationTokenSource();
    //    try
    //    {
    //        while (transform.position.z > targetZ)
    //        {
    //            // ��Ұ� ��û�Ǹ� ���ܸ� ����
    //            cts.Token.ThrowIfCancellationRequested();

    //            // z ���� �̵�, x�� y�� �״�� ����
    //            Vector3 currentPosition = transform.position;
    //            currentPosition.z = Mathf.MoveTowards(currentPosition.z, targetZ, speed * Time.deltaTime);
    //            transform.position = currentPosition;

    //            // ���� �����ӱ��� ���
    //            await UniTask.Yield(PlayerLoopTiming.Update, cts.Token);
    //        }
    //        if (EnemyBaseManager.Instance.enemyDic.TryGetValue(this.gameObject, out EnemyBase enemy))
    //        {
    //            EnemyBaseManager.Instance.endEnemy(this);
    //        }
    //        EnemyObjectpool.Instance.ReturnEnemyToPool(this);
    //    }
    //    catch (OperationCanceledException)
    //    {
    //        // ���� ��Ȱ��ȭ�� �� �۾��� ��ҵ�
    //        Debug.Log($"{gameObject.name} movement cancelled.");
    //    }
    //}

    private void Die()
    {
        //if(EnemyBaseManager.Instance.enemyDic.TryGetValue(this.gameObject, out EnemyBase enemy))
        //{
        //    EnemyBaseManager.Instance.endEnemy(this);
        //}
        //EnemyObjectpool.Instance.ReturnEnemyToPool(this);
    }
}
