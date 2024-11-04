using BaseEnum;
using UnityEngine;

public class TestEnemySpawner : MonoBehaviour
{
    // 시작 위치로 사용할 GameObject 배열 (크기 3)
    public GameObject[] spawnPoints;

    void Update()
    {
        // 예를 들어 스페이스바를 눌렀을 때 Orc 타입의 적을 소환
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int testNum = Random.Range(0, 3);
            EnemyBase enemy = EnemyObjectpool.Instance.GetEnemyFromPool((EnemyType)testNum);
            if (enemy != null)
            {
                // 랜덤한 위치에서 소환, z는 -2로 고정
                Vector3 spawnPosition = GetRandomSpawnPosition();
                enemy.transform.position = spawnPosition;
                enemy.gameObject.SetActive(true); // 적 활성화 -> OnEnable에서 이동 시작
            }
        }
    }

    // 랜덤한 GameObject의 위치를 가져와서 z 값을 -2로 설정
    private Vector3 GetRandomSpawnPosition()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("Spawn points are not set!");
            return Vector3.zero;
        }

        // 배열에서 랜덤하게 하나 선택
        GameObject chosenSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector3 spawnPosition = chosenSpawnPoint.transform.position;

        // z 값을 -2로 설정
        spawnPosition.z += -2f;

        return spawnPosition;
    }
}
