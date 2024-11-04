using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Cysharp.Threading.Tasks.Triggers;

public class Catapult : TowerBase
{
    public AimingAttackComponent aimingAttack;
    public float damageBallSpeed;
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
        aimingAttack.isBulletFollow = true;
    }
}
