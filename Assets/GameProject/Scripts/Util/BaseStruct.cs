using BaseEnum;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace BaseStruct
{
    //캐릭터 스테이터스
    [System.Serializable]
    public struct StatData
    {
        public float maxHealth;
        public float currentHealth;
        public float attackPower;
        public float attackSpeed;
        public float attackRange;

        public StatData(float maxHealth, float attackPower, float attackSpeed, float attackRange)
        {
            this.maxHealth = maxHealth;
            this.currentHealth = maxHealth; // 초기 현재 체력은 최대 체력으로 설정
            this.attackPower = attackPower;
            this.attackSpeed = attackSpeed;
            this.attackRange = attackRange;
        }
    }

    //타일의 좌표값 
    [System.Serializable]
    public class Coord
    {
        public int x;
        public int y;
        public float xPos;
        public float yPos;

        public Coord(int _x, int _y, float _xPos, float _yPos)
        {
            x = _x;
            y = _y;
            xPos = _xPos;
            yPos = _yPos;
        }
    }

    [System.Serializable]
    public class PlaceableData
    {
        public string objectName;
        public Coord index;             // 좌표값 처리  
        public Vector2Int sizeInCells;  // 그리드에서 차지하는 영역
        public PLACEMENTSTATE placementState;
        public PLACEMENTTYPE placementType;

        // 추가적인 공통 속성을 여기에 정의할 수 있습니다.
    }

    #region 저장용 데이터 형태 

    #region 인벤토리에 보유하는 타워들 
    [System.Serializable]
    public class TowerInvenData
    {
        List<TowerInvenSlot> towerSlotList = new();
    }
    [System.Serializable]
    public class TowerInvenSlot
    {
        public int Id;
        public int lv;
        public int pieceCnt;
    }
    #endregion

    #endregion

    #region CSV 데이터 구조 형태 

    // 테이블에서 읽어올 타워의 기본 데이터 
    [System.Serializable]
    public class TowerCSVData
    {
        public int Id;                  // 타워 ID
        public string Name;             // 타워 이름
        public string Desc;             // 타워 설명

        public int IsAvailable;         // 타워 사용가능 여부 -> 실제로 획득할 수 있는 타워 ( 디버프 타워 제외 )

        public string PrefabName;       // 타워 프리팹 이름
        public string IconName;         // 타워 아이콘 이름
        public int TileCnt;             // 타워가 차지하는 타일 갯수
        public string TilePrefab;       // 타워가 차지되는 타일의 프리팹 이름 

        public float AttackDmg;         // 타워 공격력 
        public float AttackSpeed;       // 타워 공격속도
        public float Range;             // 타워 공격 범위 

        public float TowerValue_1;      // 타워 보유 능력 수치 1
        public float TowerValue_2;      // 타워 보유 능력 수치 2
        public float TowerValue_3;      // 타워 보유 능력 수치 3

        public int Grade;               // 타워 등급 

        public float LvUpAtt;           // 레벨이 오를때 상승할 공격력 증가 % 
        public int MaxLv;               // 인벤토리에서 강화할 수 있는 타워의 최대레벨
        public int MaxMergeLv;          // 인게임에서 타워의 머지할 수 있는 최대레벨  
    }

    // 테이블에서 읽어올 몬스터의 기본 데이터 
    // 몬스터의 공격력과 이동속도 값은 Stage Wave의 값에 따라 설정된다.
    [System.Serializable]
    public class MonsterCSVData
    {   
        public int Id;                  // 몬스터 ID
        public string Name;             // 몬스터 이름
        public string Desc;             // 몬스터 설명 

        public int BossType;            // 보스 타입 ( 0 : 일반 / 1 : 엘리트 / 2 : 보스 )

        public string PrefabName;       // 몬스터 프리팹 이름 
        public string IconName;         // 아이콘 이름 

        public float MoveSpeed;         // 이동속도
        public int GetCoin;             // 드랍하는 골드량 
        public int GetExp;              // 드랍하는 경험치량

        public float Range;             // 시야 
        public float AttackSpeed;       // 몬스터의 공격 속도 => 보단 공격 후 딜레이?
        public float AttackDmg;         // 몬스터의 공격력 
    }



    #endregion
}