using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AreaAttackComponent : MonoBehaviour, IAttackComponent
{
    public void Attack(List<EnemyBase> enemyList)
    {
        throw new NotImplementedException();
    }

    public float GetTotalAttackPower()
    {
        throw new NotImplementedException();
    }

    public void RemoveTarget(EnemyBase enemy)
    {
        throw new NotImplementedException();
    }

    public void SetTarget(EnemyBase enemy)
    {
        throw new NotImplementedException();
    }
}
