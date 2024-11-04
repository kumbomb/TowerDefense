using BaseEnum;
using GameData;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObjectpool : Manager<EnemyObjectpool>
{
    [SerializeField] private EnemyData[] enemyPrefabs; // 적 프리팹 리스트
    private Dictionary<EnemyType, List<EnemyBase>> enemyPools; // 적 오브젝트 풀을 타입별로 관리
    [SerializeField] private int poolSize = 5; // 풀 크기

    void Start()
    {
        enemyPools = new Dictionary<EnemyType, List<EnemyBase>>();

        // 각 적 타입별로 풀 초기화
        foreach (var enemyPrefab in enemyPrefabs)
        {
            enemyPools[enemyPrefab.type] = new List<EnemyBase>();

            for (int i = 0; i < poolSize; i++)
            {
                GameObject enemy = Instantiate(enemyPrefab.enemyObj);
                enemy.name = enemy.name + enemy.GetInstanceID();
                EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
                enemyBase.enemyType = enemyPrefab.type;
                enemy.SetActive(false);
                enemyPools[enemyPrefab.type].Add(enemyBase);
            }
        }
    }

    // 적 타입에 맞는 오브젝트를 풀에서 가져오는 함수
    public EnemyBase GetEnemyFromPool(EnemyType enemyType)
    {
        if (enemyPools.ContainsKey(enemyType))
        {
            foreach (EnemyBase enemy in enemyPools[enemyType])
            {
                if (!enemy.gameObject.activeInHierarchy)
                {
                    enemy.gameObject.SetActive(true);
                    EnemyBaseManager.Instance.StartEnemy(enemy);
                    return enemy;
                }
            }

            // 풀이 꽉 찼으면 새로운 적 생성
            var prefab = GetPrefabByType(enemyType);
            if (prefab != null)
            {
                GameObject newEnemy = Instantiate(prefab);
                EnemyBase enemyBase = newEnemy.GetComponent<EnemyBase>();
                enemyBase.enemyType = enemyType;
                newEnemy.SetActive(false);
                enemyPools[enemyType].Add(enemyBase);
                return GetEnemyFromPool(enemyType);
            }
        }

        return null; // 적 타입이 없으면 null 반환
    }

    // 적을 풀로 반환하는 함수
    public void ReturnEnemyToPool(EnemyBase enemy)
    {
        if (enemy != null)
        {
            EnemyBaseManager.Instance.endEnemy(enemy);
            enemy.gameObject.SetActive(false);
        }
    }

    // 타입에 맞는 프리팹을 반환하는 함수
    private GameObject GetPrefabByType(EnemyType type)
    {
        foreach (var enemyPrefab in enemyPrefabs)
        {
            if (enemyPrefab.type == type)
            {
                return enemyPrefab.enemyObj;
            }
        }
        return null;
    }
}
