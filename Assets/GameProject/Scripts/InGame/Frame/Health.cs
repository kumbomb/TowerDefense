using UnityEngine;
using System;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    public bool isDead { get; private set; } = false;

    // 사망 시 호출되는 이벤트
    public event Action OnDeath;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
            return;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (isDead)
            return;

        isDead = true;
        Debug.Log($"{gameObject.name} died.");
        OnDeath?.Invoke();

        // 사망 애니메이션, 이펙트 등 추가 가능
        Destroy(gameObject, 0.5f); // 예: 0.5초 후 오브젝트 파괴
    }

    // 체력 회복 기능 등 추가 가능
    public void Heal(float amount)
    {
        if (isDead)
            return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"{gameObject.name} healed {amount} health. Current Health: {currentHealth}");
    }
}
