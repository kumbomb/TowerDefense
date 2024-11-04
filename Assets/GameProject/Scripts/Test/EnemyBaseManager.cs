using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseManager : Manager<EnemyBaseManager>
{
    public Dictionary<GameObject, EnemyBase> enemyDic = new Dictionary<GameObject, EnemyBase>();

    public void StartEnemy(EnemyBase enemy)
    {
        enemyDic.Add(enemy.gameObject, enemy);
    }
    public void endEnemy(EnemyBase enemy)
    {
          enemyDic.Remove(enemy.gameObject);
    }
    public void TakeDamage(List<EnemyBase> enemyList, float damage)
    {
        foreach (EnemyBase enemy in enemyList) 
        {
            if (enemyDic.TryGetValue(enemy.gameObject, out EnemyBase takeDamegeEnemy))
            {
                takeDamegeEnemy.enemyHp -= damage;
            }
        }
    }
    public void TakeDamage(EnemyBase enemy, float damage)
    {
        if (enemyDic.TryGetValue(enemy.gameObject, out EnemyBase takeDamegeEnemy))
        {
            takeDamegeEnemy.enemyHp -= damage;
        }
    }
}
