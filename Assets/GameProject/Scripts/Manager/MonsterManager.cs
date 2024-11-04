using UnityEngine;
using System.Collections.Generic;

public class MonsterManager : Singleton<MonsterManager>
{
    [SerializeField] List<GameObject> monsterPrefab;
    [SerializeField] List<GridCell> spawnPoints;
    [SerializeField] GridCell goalPoint;

    GridManager gridManager;

    public void SumonMob()
    {
        if(gridManager == null)
        {
            gridManager = GridManager.Instance;
            goalPoint = gridManager.GetGoalCell();
        }
        StartSummon();
    }
    GridCell GetRandomSpawnCell()
    {
        spawnPoints = gridManager.GetMonsterSpawnCellList();

        if (spawnPoints.Count == 0)
            return null;

        int randomIndex = Random.Range(0, spawnPoints.Count);
        return spawnPoints[randomIndex];
    }

    void StartSummon()
    {
        // 이동 가능한 랜덤한 스폰 셀 선택
        GridCell spawnCell = GetRandomSpawnCell();
        int randNum = Random.Range(0, monsterPrefab.Count);
        if (spawnCell != null)
        {
            Vector3 spawnPosition = spawnCell.position;
            //풀링으로 변경
            GameObject monsterObj = Instantiate(monsterPrefab[randNum], spawnPosition, Quaternion.identity, transform);
            EnemyUnit monster = monsterObj.GetComponent<EnemyUnit>();
            monster.SetGoalCell(goalPoint);
            monster.currentCell = spawnCell;
        }
    }

    public void RegisterUnit(Health unitHealth)
    {
        if (unitHealth != null)
        {
            unitHealth.OnDeath += HandleUnitDeath;
        }
    }

    public void UnregisterUnit(Health unitHealth)
    {
        if (unitHealth != null)
        {
            unitHealth.OnDeath -= HandleUnitDeath;
        }
    }
    private void HandleUnitDeath(/*BaseUnit deadUnit*/)
    {
        // 사망한 유닛에 대한 처리
        Debug.Log($"MonsterManager detected that {/*deadUnit.*/gameObject.name} has died.");
        // 예: ScoreManager.Instance.AddScore(10);
        // 예: UIManager.Instance.UpdateScore();
    }
}
