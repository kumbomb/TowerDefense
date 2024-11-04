using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private GameObject playerPrefab; // Player ������
    [SerializeField] private PlayerJoyStickMove joystick; // ���̽�ƽ ����

    bool isInit = false;

    /// <summary>
    /// Player ������Ʈ�� �������� �����ϰ�, ���̽�ƽ ������ �Ҵ��մϴ�.
    /// </summary>
    /// <param name="position">Player ���� ��ġ</param>
    /// <param name="rotation">Player ���� ȸ��</param>
    public void CreatePlayer(Vector3 position, Quaternion rotation)
    {
        if (playerPrefab == null)
        {
           // Debug.LogError("Player �������� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        if (joystick == null)
        {
            //Debug.LogError("PlayerJoyStickMove ������ �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        GameObject player = Instantiate(playerPrefab, position, rotation);
        Player controller = player.GetComponent<Player>();
        if (controller != null)
        {
            controller.SetJoystick(joystick);
        }
        else
        {
           //Debug.LogWarning("Player ������Ʈ�� PlayerController ��ũ��Ʈ�� �����ϴ�.");
        }
    }

    public void InitChar()
    {
        if (!isInit)
        {
            isInit = true;
            GridManager gridManager = GridManager.Instance;
            CreatePlayer(gridManager.GetPlayerSpawnCell().position, Quaternion.identity);
        }
        else
        {
            //Debug.Log(" �÷��̾� ĳ���Ͱ� �̹� �����մϴ� ");
        }
    }
}
