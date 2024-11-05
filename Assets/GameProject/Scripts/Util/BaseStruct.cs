using BaseEnum;
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

    [System.Serializable]
    public struct RewardStruct
    {
        public int Idx;
        public int Amount;
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

    // 챕터 데이터
    [System.Serializable]
    public class ChapterData
    {
        public int Id;
        public string ChapterTitle;
        public int[] StageIdxs;                 // csv에서 '/'를 기준으로 split한뒤 int로 변환하여 배열로 만든다
        public string ChapterImgName;
    }

    // 스테이지 데이터
    [System.Serializable]
    public class StageData
    {
        public int Id;
        public string StageName;
        public int WaveIdx;
        public RewardStruct[] rewardStructs;    // csv 에서 '|' 를 기준으로 split한뒤 각 배열을 다시 '/' 로 split 해서 RewardStruct 생성 
        public string StageImgName;
        public int NeedStamina;
        public string SceneName;
        public string LevelPrefabName;
    }

    // 웨이브 데이터
    [System.Serializable]
    public class WaveData
    {
        public int Id;
        public int WaveIdx;
        public int WaveType;
        public int[] MonsterIdxs;           // csv에서 '/'를 기준으로 split 해서 int 변환 후 배열 생성
        public float WaveTime;
        public int SummonCnt;
        public int MaxCount;
        public float MultiplyHp;
        public float MultiplySpeed;
        public float MultiplyAtsp;
    }

    // 몬스터 데이터
    [System.Serializable]
    public class MonsterData
    {
        public int Id;                  // 몬스터 ID
        public string Name;             // 몬스터 이름
        public string PrefabName;       // 몬스터 프리팹 이름
        public int IsBoss;              // MONSTER_TYPE;
        public int HP;
        public float Speed;             // 기본값이 되는 스피드 -> wave에 따라 값이 증감됨
        public int Atk;
        public int DropExp;             // 획득 경험치량
        public float DropBeads;         // 경험치 구슬 드랍 확률 max = 100
    }

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

    #endregion
}