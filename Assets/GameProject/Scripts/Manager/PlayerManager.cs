using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private GameObject playerPrefab; // Player 프리팹
    [SerializeField] private PlayerJoyStickMove joystick; // 조이스틱 참조

    bool isInit = false;

    /// <summary>
    /// Player 오브젝트를 동적으로 생성하고, 조이스틱 참조를 할당합니다.
    /// </summary>
    /// <param name="position">Player 생성 위치</param>
    /// <param name="rotation">Player 생성 회전</param>
    public void CreatePlayer(Vector3 position, Quaternion rotation)
    {
        if (playerPrefab == null)
        {
           // Debug.LogError("Player 프리팹이 할당되지 않았습니다.");
            return;
        }

        if (joystick == null)
        {
            //Debug.LogError("PlayerJoyStickMove 참조가 할당되지 않았습니다.");
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
           //Debug.LogWarning("Player 오브젝트에 PlayerController 스크립트가 없습니다.");
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
            //Debug.Log(" 플레이어 캐릭터가 이미 존재합니다 ");
        }
    }
}
