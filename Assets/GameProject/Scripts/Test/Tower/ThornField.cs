using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class ThornField : TowerBase
{
    public MeleeAttackComponent meleeAttack;
    private void Start()
    {
        if(meleeAttack == null)
        {
            meleeAttack = GetComponent<MeleeAttackComponent>();
        }
        UpdateAbility();
    }
    public override void UpdateAbility()
    {
        meleeAttack.attackSpeed = attackSpeed;
        meleeAttack.attackPower = attackPower;
    }
}
