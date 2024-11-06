using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;
    public static bool HasInstance => _instance != null;
    public static T TryGetInstance() => HasInstance ? _instance : null;
    public static T Current => _instance;

    // DontDestroyOnLoad 적용 여부를 결정하는 플래그
    protected virtual bool IsPersistent => false;

    /// <summary>
    /// 싱글톤 디자인 패턴
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // Unity 2023.1 이상에서는 FindAnyObjectByType<T>() 사용 권장
                _instance = FindAnyObjectByType<T>();

                if (_instance == null)
                {
                    Debug.Log("Instance Check");
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name + "_AutoCreated";
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// Awake에서 인스턴스를 초기화합니다. 만약 Awake를 override해서 사용해야 한다면 base.Awake()를 호출해야 합니다.
    /// </summary>
    protected virtual void Awake()
    {
        InitializeSingleton();
    }
    
    /// <summary>
    /// 싱글톤을 초기화합니다.
    /// </summary>
    protected virtual void InitializeSingleton()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (_instance == null)
        {
            _instance = this as T;

            // IsPersistent가 true일 때만 DontDestroyOnLoad 적용
            if (IsPersistent)
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }
        else if (_instance != this)
        {
            // 이미 인스턴스가 존재하면 중복 생성 방지
            Destroy(this.gameObject);
        }
    }
}
