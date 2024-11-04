using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using UnityEngine;

public class ThrowSpearWall : TowerBase
{
    public AimingAttackComponent aimingAttack;
    public float damageBallSpeed;
    public float attackSize;
    public BoxCollider attackCollider;
    private void Start()
    {
        if (aimingAttack == null)
        {
            aimingAttack = GetComponent<AimingAttackComponent>();
        }
        UpdateAbility();
    }
    public override void UpdateAbility()
    {
        aimingAttack.attackSpeed = attackSpeed;
        aimingAttack.attackPower = attackPower;
        aimingAttack.damageBallSpeed = damageBallSpeed;
        aimingAttack.colliderSize = GetColliderSize();
        aimingAttack.isBulletFollow = false;
    }
    public float GetColliderSize()
    {
        SetColliderSize();
        return attackCollider.size.z;
    }
    public void SetColliderSize()
    {
        Vector3 center = attackCollider.center;
        Vector3 size = attackCollider.size;

        size.z = attackSize;
        center.z = size.z / 2f;
        attackCollider.size = size;
        attackCollider.center = center;
    }
}
