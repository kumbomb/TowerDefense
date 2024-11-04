using System.Collections.Generic;
using UnityEngine;

public interface IAttackComponent
{
    void SetTarget(EnemyBase enemy);
    void RemoveTarget(EnemyBase enemy);
    void Attack(List<EnemyBase> enemyList);
    float GetTotalAttackPower();
}
