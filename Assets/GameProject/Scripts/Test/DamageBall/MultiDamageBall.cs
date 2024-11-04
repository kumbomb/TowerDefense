using UnityEngine;

public class MultiDamageBall : DamageBallBase
{
    protected override void Move()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            gameObject.SetActive(false);
        }
    }
    protected override void ApplyDamage(EnemyBase enemy)
    {
        EnemyBaseManager.Instance.TakeDamage(enemy, damage);
    }

    public  void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) // �� �±׸� ���� ������Ʈ���� �浹�� ����
        {
            if(EnemyBaseManager.Instance.enemyDic.TryGetValue(other.gameObject, out EnemyBase enemy))
            {
                ApplyDamage(enemy);
            }
        }
    }
}
