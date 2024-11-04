using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DamageBall : MonoBehaviour
{
    public float speed = 5f; // 데미지 볼의 속도
    public float damage;
    public EnemyBase targetEnemy; // 타겟 적
    public List<EnemyBase> targetEnemyList; // 타겟 적
    public bool isFollow;
    public bool isMultipleAttacks;
    public float lifetime = 0.3f;
    private float timer;
    // 데미지 볼 초기화 (타겟 적을 설정)
    public void Initialize(EnemyBase enemy, List<EnemyBase> enemyList)
    {
        timer = 0f; 
        targetEnemy = enemy;
        targetEnemyList = enemyList;
    }

    void Update()
    {
        if (targetEnemy != null && targetEnemy.gameObject.activeSelf)
        {
            if (isFollow)
            {
                // 적을 향해 데미지 볼을 이동시킴
                Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;
                transform.LookAt(targetEnemy.transform.position);
                // 적과의 거리가 매우 가까워졌을 때 (충돌로 간주)
                if (Vector3.Distance(transform.position, targetEnemy.transform.position) < 0.3f)
                {
                    HitTarget(targetEnemy);
                }
            }
            else
            {
                if(!isMultipleAttacks)
                {
                    if (Vector3.Distance(transform.position, targetEnemy.transform.position) < 1f)
                    {
                        HitTarget(targetEnemy);
                    }
                }
                else
                {
                    MultipleAttack();
                }
                // 생존 시간 체크 후 파괴
                timer += Time.deltaTime;
                if (timer >= lifetime)
                {
                     gameObject.SetActive(false);
                }
            }
        }
        else
        {
            this.transform.position = Vector3.zero;
            // 타겟이 없으면 데미지 볼 비활성화
            gameObject.SetActive(false);
        }
    }
    private void MultipleAttack()
    {
        foreach(EnemyBase enemy in targetEnemyList)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < 1f)
            {
                MultipleHitTarget(enemy);
            }
        }
    }
    private void MultipleHitTarget(EnemyBase enemy)
    {
        if (targetEnemy != null)
        {
            EnemyBaseManager.Instance.TakeDamage(enemy, damage);
        }
    }
    // 타겟 적에 맞았을 때 호출
    private void HitTarget(EnemyBase enemy)
    {
        if (targetEnemy != null)
        {
            EnemyBaseManager.Instance.TakeDamage(enemy, damage);
        }
        targetEnemy = null;
        // 데미지 볼을 비활성화
        gameObject.SetActive(false);
    }
}
