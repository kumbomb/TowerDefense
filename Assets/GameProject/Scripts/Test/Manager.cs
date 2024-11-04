using UnityEngine;

/// <summary>
/// ���׸� �̱��� ���̽� Ŭ����.
/// ��� �Ŵ��� Ŭ������ �� Ŭ������ ��ӹ޾� �̱������� ������ �� �ֽ��ϴ�.
/// </summary>
/// <typeparam name="T">�̱������� ����� Ŭ���� Ÿ��</typeparam>
public class Manager<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    /// <summary>
    /// �ܺο��� ���� ������ �ν��Ͻ� ������Ƽ
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // �� ������ �ش� Ÿ���� ������Ʈ�� ã��
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    // �� ���� ������ ���� ����
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();
                }

                // �ν��Ͻ��� �ٸ� ������ �Ѿ�� �ı����� �ʵ��� ����
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    /// <summary>
    /// Awake �޼��忡�� �ν��Ͻ� ���� �� �ߺ� ����
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
