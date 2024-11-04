using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class DamageBallBase : MonoBehaviour
{
    // ������
    public float damage;

    // �ӵ�
    public float speed;

    // ���� �� �� �ִ°�?
    [HideInInspector]
    public bool isAttack;

    // ���� Ÿ��
    [HideInInspector]
    public float timer;
    [HideInInspector]
    public float lifeTime;
    // ����, �ð�ȿ��
    public GameObject visualEffect;
    public AudioClip soundEffect;

    [HideInInspector]
    public EnemyBase targetEnemy;
    [HideInInspector]
    public List<EnemyBase> targetEnemyList;
    protected virtual void Start()
    {
        // �ð� ȿ�� �� ���� ���
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
