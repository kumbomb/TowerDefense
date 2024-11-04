using BaseEnum;
using UnityEngine;

namespace GameData
{
    public static class GameData
    {
    }
    [System.Serializable]
    public class TowerData
    {
        public TowerType type;
        public GameObject towerObj;
    }
    [System.Serializable]
    public struct EnemyData
    {
        public EnemyType type;
        public GameObject enemyObj;
    }
}


