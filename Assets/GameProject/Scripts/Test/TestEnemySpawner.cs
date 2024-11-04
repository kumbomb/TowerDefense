using BaseEnum;
using UnityEngine;

public class TestEnemySpawner : MonoBehaviour
{
    // ���� ��ġ�� ����� GameObject �迭 (ũ�� 3)
    public GameObject[] spawnPoints;

    void Update()
    {
        // ���� ��� �����̽��ٸ� ������ �� Orc Ÿ���� ���� ��ȯ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int testNum = Random.Range(0, 3);
            EnemyBase enemy = EnemyObjectpool.Instance.GetEnemyFromPool((EnemyType)testNum);
            if (enemy != null)
            {
                // ������ ��ġ���� ��ȯ, z�� -2�� ����
                Vector3 spawnPosition = GetRandomSpawnPosition();
                enemy.transform.position = spawnPosition;
                enemy.gameObject.SetActive(true); // �� Ȱ��ȭ -> OnEnable���� �̵� ����
            }
        }
    }

    // ������ GameObject�� ��ġ�� �����ͼ� z ���� -2�� ����
    private Vector3 GetRandomSpawnPosition()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("Spawn points are not set!");
            return Vector3.zero;
        }

        // �迭���� �����ϰ� �ϳ� ����
        GameObject chosenSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector3 spawnPosition = chosenSpawnPoint.transform.position;

        // z ���� -2�� ����
        spawnPosition.z += -2f;

        return spawnPosition;
    }
}
