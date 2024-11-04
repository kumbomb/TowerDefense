using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class DamageBallBase : MonoBehaviour
{
    // 데미지
    public float damage;

    // 속도
    public float speed;

    // 공격 할 수 있는가?
    [HideInInspector]
    public bool isAttack;

    // 생존 타임
    [HideInInspector]
    public float timer;
    [HideInInspector]
    public float lifeTime;
    // 사운드, 시각효과
    public GameObject visualEffect;
    public AudioClip soundEffect;

    [HideInInspector]
    public EnemyBase targetEnemy;
    [HideInInspector]
    public List<EnemyBase> targetEnemyList;
    protected virtual void Start()
    {
        // 시각 효과 및 사운드 재생
        PlayEffects();
    }
    protected virtual void Update()
    {
        if(gameObject.activeSelf)
        {
            Move();
        }
    }
    public virtual void Initialize(EnemyBase enemy, List<EnemyBase> enemyList)
    {
        timer = 0f;
        targetEnemy = enemy;
        targetEnemyList = enemyList;
        isAttack = true;
    }
    protected abstract void Move();
    protected abstract void ApplyDamage(EnemyBase enemy);
    protected virtual void endAttack()
    {
        isAttack = false;
        this.gameObject.SetActive(false);
    }
    protected virtual void PlayEffects()
    {
        if (visualEffect != null)
            Instantiate(visualEffect, transform.position, Quaternion.identity);

        if (soundEffect != null)
            AudioSource.PlayClipAtPoint(soundEffect, transform.position);
    }
}
