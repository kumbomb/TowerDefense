using UnityEngine;

public class Utility
{
    /// <summary>
    /// 부모 오브젝트의 자식 중 이름이 일치하는 Transform을 찾습니다.
    /// </summary>
    /// <param name="parent">검색할 부모 Transform</param>
    /// <param name="childName">찾고자 하는 자식의 이름</param>
    /// <param name="recursive">재귀적으로 모든 하위 자식을 검색할지 여부</param>
    /// <returns>이름이 일치하는 Transform 또는 null</returns>
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
