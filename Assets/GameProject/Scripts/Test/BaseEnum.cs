using UnityEngine;

namespace BaseEnum
{
    public enum SCENE_TYPE
    {
        PRELOADER,
        LOBBY,
        GAME,
    }

    public enum TABLE_TYPE
    {
        TABLE_CHAPTER,
        TABLE_STAGE,
        TABLE_MONSTER,
        TABLE_WAVE,
    }
    public enum STAGE_TYPE
    {
        STAGE_TUTORIAL,
        STAGE_SCENARIO,
        STAGE_MODE
    }

    public enum WAVE_TYPE
    {
        WAVE_DEFAULT,
        WAVE_INTERVAL_BOSS,
        WAVE_FINAL_BOSS
    }
    
    public enum MONSTER_TYPE
    {
        MONSTER_NORMAL,
        MONSTER_ELITE,
        MONSTER_BOSS
    }
    
    public enum TowerType
    {
        normal,
        fire,
        ice,
        lighting,
        portal,
        tree,
        copper
    }
    public enum LocationType
    {
        Wall,
        Ground
    }
    public enum EnemyType
    {
        Orc,
        Goblin,
        Dragon
    }
    // 설치할 수 있는 상태 처리 
    public enum PLACEMENTSTATE
    {
        NONE,              // 아무것도 설치되어 있지 않음
        PLACABLE,          // 설치가 가능 
        IMPLACABLE,        // 무언가 설치되어 설치가 불가능
        HIDDEN,            // 가림 처리 => 사용불가능한 영역
        MONSTERSPAWN,      // 몬스터 소환되는 지점 
        ENDPOINT,          // 몬스터의 도달 목표 지점 
        STARTPOINT,        // 유저 캐릭터의 최초 시작점
    }

    public enum PLACEMENTTYPE
    {
        NONE,
        FLOOR,      // 바닥설치 
        OBSTACLE,   // 위에 설치
        WALL,       // 벽에 설치? => 일단 만들어둠
    }

}

