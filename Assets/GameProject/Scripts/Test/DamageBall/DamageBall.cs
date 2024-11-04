using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DamageBall : MonoBehaviour
{
    public float speed = 5f; // ������ ���� �ӵ�
    public float damage;
    public EnemyBase targetEnemy; // Ÿ�� ��
    public List<EnemyBase> targetEnemyList; // Ÿ�� ��
    public bool isFollow;
    public bool isMultipleAttacks;
    public float lifetime = 0.3f;
    private float timer;
    // ������ �� �ʱ�ȭ (Ÿ�� ���� ����)
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
                // ���� ���� ������ ���� �̵���Ŵ
                Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;
                transform.LookAt(targetEnemy.transform.position);
                // ������ �Ÿ��� �ſ� ��������� �� (�浹�� ����)
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
                // ���� �ð� üũ �� �ı�
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
            // Ÿ���� ������ ������ �� ��Ȱ��ȭ
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
    // Ÿ�� ���� �¾��� �� ȣ��
    private void HitTarget(EnemyBase enemy)
    {
        if (targetEnemy != null)
        {
            EnemyBaseManager.Instance.TakeDamage(enemy, damage);
        }
        targetEnemy = null;
        // ������ ���� ��Ȱ��ȭ
        gameObject.SetActive(false);
    }
}
