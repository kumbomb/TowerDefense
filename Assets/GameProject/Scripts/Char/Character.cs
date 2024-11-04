using BaseStruct;
using UnityEngine;

public class Character : MonoBehaviour
{
    public StatData statData;

    public virtual void TakeDamage(float damage)
    {
        statData.currentHealth -= damage;
        if (statData.currentHealth <= 0)
        {
            statData.currentHealth = 0;
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name}이(가) 사망했습니다.");
        Destroy(gameObject);
    }
}
