using BaseEnum;
using GameData;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TowerObjectPool은 다양한 타워 프리팹을 오브젝트 풀링 방식으로 관리하는 매니저 클래스입니다.
/// 싱글톤 패턴을 사용하여 전역적으로 접근 가능합니다.
/// </summary>
public class TowerObjectPool : MonoBehaviour
{
    // 싱글톤 인스턴스
    private static TowerObjectPool _instance;
    public static TowerObjectPool Instance
    {
        get
        {
            if (_instance == null)
            {
                // 씬에 존재하지 않으면 새로 생성
                GameObject poolObject = new GameObject("TowerObjectPool");
                _instance = poolObject.AddComponent<TowerObjectPool>();
                DontDestroyOnLoad(poolObject);
            }
            return _instance;
        }
    }

    [Header("타워 프리팹 설정")]
    [Tooltip("풀링할 모든 타워 프리팹과 타입을 설정하세요.")]
    public List<TowerData> towerPrefabs = new List<TowerData>();

    [Header("초기 풀 크기")]
    [Tooltip("각 타워 타입별로 초기 풀 크기를 설정하세요.")]
    public int initialPoolSize = 10;

    // 각 타워 타입별 오브젝트 풀을 관리하는 딕셔너리
    private Dictionary<TowerType, Queue<GameObject>> towerPools = new Dictionary<TowerType, Queue<GameObject>>();

    // 초기화 여부
    private bool isInitialized = false;

    /// <summary>
    /// Awake 메서드에서 오브젝트 풀을 초기화합니다.
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
    /// 모든 타워 프리팹에 대한 오브젝트 풀을 초기화합니다.
    /// </summary>
    private void InitializePools()
    {
        foreach (var towerPrefab in towerPrefabs)
        {
            TowerType type = towerPrefab.type;
            GameObject prefab = towerPrefab.towerObj;

            if (prefab == null)
            {
                Debug.LogError($"타워 타입 '{type}'에 대한 프리팹이 할당되지 않았습니다.");
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
                Debug.LogWarning($"타워 풀에 '{type}' 타입이 이미 존재합니다.");
            }
        }
        isInitialized = true;
        Debug.Log("TowerObjectPool이 초기화되었습니다.");
    }

    /// <summary>
    /// 특정 타워 타입의 오브젝트를 풀에서 가져옵니다.
    /// </summary>
    /// <param name="type">타워 타입</param>
    /// <returns>활성화된 타워 오브젝트</returns>
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
                // 풀에 여유가 없으면 새로 생성
                GameObject prefab = towerPrefabs.Find(p => p.type == type)?.towerObj;
                if (prefab != null)
                {
                    GameObject obj = Instantiate(prefab);
                    return obj;
                }
                else
                {
                    Debug.LogError($"타워 타입 '{type}'에 대한 프리팹을 찾을 수 없습니다.");
                    return null;
                }
            }
        }
        else
        {
            Debug.LogError($"타워 풀에 '{type}' 타입이 존재하지 않습니다.");
            return null;
        }
    }

    /// <summary>
    /// 특정 타워 오브젝트를 풀에 반환합니다.
    /// </summary>
    /// <param name="type">타워 타입</param>
    /// <param name="obj">반환할 타워 오브젝트</param>
    public void ReturnTower(TowerType type, GameObject obj)
    {
        obj.SetActive(false);
        if (towerPools.ContainsKey(type))
        {
            towerPools[type].Enqueue(obj);
        }
        else
        {
            Debug.LogError($"타워 풀에 '{type}' 타입이 존재하지 않습니다.");
            Destroy(obj);
        }
    }

    /// <summary>
    /// 현재 풀에 등록된 타워의 종류 수를 반환합니다.
    /// </summary>
    /// <returns>타워 종류 수</returns>
    public int GetTowerTypeCount()
    {
        return towerPools.Count;
    }

    /// <summary>
    /// 특정 타워 타입의 풀에 있는 타워의 수를 반환합니다.
    /// </summary>
    /// <param name="type">타워 타입</param>
    /// <returns>풀에 있는 타워 수</returns>
    public int GetPoolCount(TowerType type)
    {
        if (towerPools.ContainsKey(type))
        {
            return towerPools[type].Count;
        }
        else
        {
            Debug.LogError($"타워 풀에 '{type}' 타입이 존재하지 않습니다.");
            return 0;
        }
    }
}
