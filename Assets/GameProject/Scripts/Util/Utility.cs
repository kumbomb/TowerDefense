using UnityEngine;

public class Utility
{
    /// <summary>
    /// �θ� ������Ʈ�� �ڽ� �� �̸��� ��ġ�ϴ� Transform�� ã���ϴ�.
    /// </summary>
    /// <param name="parent">�˻��� �θ� Transform</param>
    /// <param name="childName">ã���� �ϴ� �ڽ��� �̸�</param>
    /// <param name="recursive">��������� ��� ���� �ڽ��� �˻����� ����</param>
    /// <returns>�̸��� ��ġ�ϴ� Transform �Ǵ� null</returns>
    public static Transform FindDeepChildByName(Transform parent, string childName, bool recursive = true)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }

            if (recursive)
            {
                Transform result = FindDeepChildByName(child, childName, recursive);
                if (result != null)
                {
                    return result;
                }
            }
        }

        return null;
    }
}
