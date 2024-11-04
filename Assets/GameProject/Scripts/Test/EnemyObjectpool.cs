using BaseEnum;
using GameData;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObjectpool : Manager<EnemyObjectpool>
{
    [SerializeField] private EnemyData[] enemyPrefabs; // �� ������ ����Ʈ
    private Dictionary<EnemyType, List<EnemyBase>> enemyPools; // �� ������Ʈ Ǯ�� Ÿ�Ժ��� ����
    [SerializeField] private int poolSize = 5; // Ǯ ũ��

    void Start()
    {
        enemyPools = new Dictionary<EnemyType, List<EnemyBase>>();

        // �� �� Ÿ�Ժ��� Ǯ �ʱ�ȭ
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

    // �� Ÿ�Կ� �´� ������Ʈ�� Ǯ���� �������� �Լ�
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

            // Ǯ�� �� á���� ���ο� �� ����
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

        return null; // �� Ÿ���� ������ null ��ȯ
    }

    // ���� Ǯ�� ��ȯ�ϴ� �Լ�
    public void ReturnEnemyToPool(EnemyBase enemy)
    {
        if (enemy != null)
        {
            EnemyBaseManager.Instance.endEnemy(enemy);
            enemy.gameObject.SetActive(false);
        }
    }

    // Ÿ�Կ� �´� �������� ��ȯ�ϴ� �Լ�
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
