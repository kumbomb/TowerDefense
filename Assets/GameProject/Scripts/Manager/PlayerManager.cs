using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private PlayerJoyStickMove joystick;

    bool isInit = false;

    public void CreatePlayer(Vector3 position, Quaternion rotation)
    {
        if (playerPrefab == null)
        {
            return;
        }

        if (joystick == null)
        {
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
        }
    }

    public void InitChar()
    {
        if (!isInit)
        {
            isInit = true;
            CreatePlayer(GridManager.Instance.GetPlayerSpawnCell().position, Quaternion.identity);
        }
        else
        {
        }
    }
}
