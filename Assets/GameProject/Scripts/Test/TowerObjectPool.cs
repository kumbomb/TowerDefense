using BaseEnum;
using GameData;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TowerObjectPool�� �پ��� Ÿ�� �������� ������Ʈ Ǯ�� ������� �����ϴ� �Ŵ��� Ŭ�����Դϴ�.
/// �̱��� ������ ����Ͽ� ���������� ���� �����մϴ�.
/// </summary>
public class TowerObjectPool : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    private static TowerObjectPool _instance;
    public static TowerObjectPool Instance
    {
        get
        {
            if (_instance == null)
            {
                // ���� �������� ������ ���� ����
                GameObject poolObject = new GameObject("TowerObjectPool");
                _instance = poolObject.AddComponent<TowerObjectPool>();
                DontDestroyOnLoad(poolObject);
            }
            return _instance;
        }
    }

    [Header("Ÿ�� ������ ����")]
    [Tooltip("Ǯ���� ��� Ÿ�� �����հ� Ÿ���� �����ϼ���.")]
    public List<TowerData> towerPrefabs = new List<TowerData>();

    [Header("�ʱ� Ǯ ũ��")]
    [Tooltip("�� Ÿ�� Ÿ�Ժ��� �ʱ� Ǯ ũ�⸦ �����ϼ���.")]
    public int initialPoolSize = 10;

    // �� Ÿ�� Ÿ�Ժ� ������Ʈ Ǯ�� �����ϴ� ��ųʸ�
    private Dictionary<TowerType, Queue<GameObject>> towerPools = new Dictionary<TowerType, Queue<GameObject>>();

    // �ʱ�ȭ ����
    private bool isInitialized = false;

    /// <summary>
    /// Awake �޼��忡�� ������Ʈ Ǯ�� �ʱ�ȭ�մϴ�.
    /// </summary>
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            InitializePools();
        }
        else if (_instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// ��� Ÿ�� �����տ� ���� ������Ʈ Ǯ�� �ʱ�ȭ�մϴ�.
    /// </summary>
    private void InitializePools()
    {
        foreach (var towerPrefab in towerPrefabs)
        {
            TowerType type = towerPrefab.type;
            GameObject prefab = towerPrefab.towerObj;

            if (prefab == null)
            {
                Debug.LogError($"Ÿ�� Ÿ�� '{type}'�� ���� �������� �Ҵ���� �ʾҽ��ϴ�.");
                continue;
            }

            if (!towerPools.ContainsKey(type))
            {
                Queue<GameObject> pool = new Queue<GameObject>();
                for (int i = 0; i < initialPoolSize; i++)
                {
                    GameObject obj = Instantiate(prefab);
                    obj.SetActive(false);
                    pool.Enqueue(obj);
                }
                towerPools.Add(type, pool);
            }
            else
            {
                Debug.LogWarning($"Ÿ�� Ǯ�� '{type}' Ÿ���� �̹� �����մϴ�.");
            }
        }
        isInitialized = true;
        Debug.Log("TowerObjectPool�� �ʱ�ȭ�Ǿ����ϴ�.");
    }

    /// <summary>
    /// Ư�� Ÿ�� Ÿ���� ������Ʈ�� Ǯ���� �����ɴϴ�.
    /// </summary>
    /// <param name="type">Ÿ�� Ÿ��</param>
    /// <returns>Ȱ��ȭ�� Ÿ�� ������Ʈ</returns>
    public GameObject GetTower(TowerType type)
    {
        if (!isInitialized)
        {
            InitializePools();
        }

        if (towerPools.ContainsKey(type))
        {
            Queue<GameObject> pool = towerPools[type];
            if (pool.Count > 0)
            {
                GameObject obj = pool.Dequeue();
                obj.SetActive(true);
                return obj;
            }
            else
            {
                // Ǯ�� ������ ������ ���� ����
                GameObject prefab = towerPrefabs.Find(p => p.type == type)?.towerObj;
                if (prefab != null)
                {
                    GameObject obj = Instantiate(prefab);
                    return obj;
                }
                else
                {
                    Debug.LogError($"Ÿ�� Ÿ�� '{type}'�� ���� �������� ã�� �� �����ϴ�.");
                    return null;
                }
            }
        }
        else
        {
            Debug.LogError($"Ÿ�� Ǯ�� '{type}' Ÿ���� �������� �ʽ��ϴ�.");
            return null;
        }
    }

    /// <summary>
    /// Ư�� Ÿ�� ������Ʈ�� Ǯ�� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="type">Ÿ�� Ÿ��</param>
    /// <param name="obj">��ȯ�� Ÿ�� ������Ʈ</param>
    public void ReturnTower(TowerType type, GameObject obj)
    {
        obj.SetActive(false);
        if (towerPools.ContainsKey(type))
        {
            towerPools[type].Enqueue(obj);
        }
        else
        {
            Debug.LogError($"Ÿ�� Ǯ�� '{type}' Ÿ���� �������� �ʽ��ϴ�.");
            Destroy(obj);
        }
    }

    /// <summary>
    /// ���� Ǯ�� ��ϵ� Ÿ���� ���� ���� ��ȯ�մϴ�.
    /// </summary>
    /// <returns>Ÿ�� ���� ��</returns>
    public int GetTowerTypeCount()
    {
        return towerPools.Count;
    }

    /// <summary>
    /// Ư�� Ÿ�� Ÿ���� Ǯ�� �ִ� Ÿ���� ���� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="type">Ÿ�� Ÿ��</param>
    /// <returns>Ǯ�� �ִ� Ÿ�� ��</returns>
    public int GetPoolCount(TowerType type)
    {
        if (towerPools.ContainsKey(type))
        {
            return towerPools[type].Count;
        }
        else
        {
            Debug.LogError($"Ÿ�� Ǯ�� '{type}' Ÿ���� �������� �ʽ��ϴ�.");
            return 0;
        }
    }
}
