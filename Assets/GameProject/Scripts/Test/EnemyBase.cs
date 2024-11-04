using BaseEnum;
using System.Threading;
using System;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public GridCell currentCell;
    public EnemyType enemyType;
    public float moveSpeed = 5f; // 이동 속도
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
   // private CancellationTokenSource cts; // 취소 토큰 소스

    private void OnEnable()
    {
        // 적이 활성화되면 이동 시작
        //StartMovementAsync(-12f, moveSpeed).Forget();
        enemyHp = enemyMaxHP;
    }

    //private void OnDisable()
    //{
    //    // 적이 비활성화되면 이동 중단
    //    if (cts != null)
    //    {
    //        cts.Cancel();
    //        cts.Dispose();
    //    }
    //}

    // z축으로 목표 위치까지 이동하는 비동기 작업
    //private async UniTaskVoid StartMovementAsync(float targetZ, float speed)
    //{
    //    // 새로운 CancellationTokenSource 생성
    //    cts = new CancellationTokenSource();
    //    try
    //    {
    //        while (transform.position.z > targetZ)
    //        {
    //            // 취소가 요청되면 예외를 던짐
    //            cts.Token.ThrowIfCancellationRequested();

    //            // z 값만 이동, x와 y는 그대로 유지
    //            Vector3 currentPosition = transform.position;
    //            currentPosition.z = Mathf.MoveTowards(currentPosition.z, targetZ, speed * Time.deltaTime);
    //            transform.position = currentPosition;

    //            // 다음 프레임까지 대기
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
    //        // 적이 비활성화될 때 작업이 취소됨
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
