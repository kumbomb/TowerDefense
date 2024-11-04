using UnityEngine;

public class SingleFollowDamageBall : DamageBallBase
{
    protected override void Move()
    {
        if (targetEnemy != null && targetEnemy.gameObject.activeSelf)
        {
            Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            transform.LookAt(targetEnemy.transform.position);
            // ������ �Ÿ��� �ſ� ��������� �� (�浹�� ����)
            if (Vector3.Distance(transform.position, targetEnemy.transform.position) < 0.3f)
            {
                ApplyDamage(targetEnemy);
            }
        }
    }
    protected override void ApplyDamage(EnemyBase enemy)
    {
        if (enemy != null)
        {
            EnemyBaseManager.Instance.TakeDamage(enemy, damage);
        }
        enemy = null;
        // ������ ���� ��Ȱ��ȭ
        endAttack();
    }
}
