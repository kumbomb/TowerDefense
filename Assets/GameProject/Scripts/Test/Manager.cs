using UnityEngine;

/// <summary>
/// 제네릭 싱글톤 베이스 클래스.
/// 모든 매니저 클래스는 이 클래스를 상속받아 싱글톤으로 구현할 수 있습니다.
/// </summary>
/// <typeparam name="T">싱글톤으로 사용할 클래스 타입</typeparam>
public class Manager<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    /// <summary>
    /// 외부에서 접근 가능한 인스턴스 프로퍼티
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // 씬 내에서 해당 타입의 오브젝트를 찾음
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    // 씬 내에 없으면 새로 생성
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();
                }

                // 인스턴스가 다른 씬으로 넘어가도 파괴되지 않도록 설정
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    /// <summary>
    /// Awake 메서드에서 인스턴스 설정 및 중복 방지
    /// </summary>
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if (this != _instance)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
