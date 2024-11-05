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
    // ��ġ�� �� �ִ� ���� ó�� 
    public enum PLACEMENTSTATE
    {
        NONE,              // �ƹ��͵� ��ġ�Ǿ� ���� ����
        PLACABLE,          // ��ġ�� ���� 
        IMPLACABLE,        // ���� ��ġ�Ǿ� ��ġ�� �Ұ���
        HIDDEN,            // ���� ó�� => ���Ұ����� ����
        MONSTERSPAWN,      // ���� ��ȯ�Ǵ� ���� 
        ENDPOINT,          // ������ ���� ��ǥ ���� 
        STARTPOINT,        // ���� ĳ������ ���� ������
    }

    public enum PLACEMENTTYPE
    {
        NONE,
        FLOOR,      // �ٴڼ�ġ 
        OBSTACLE,   // ���� ��ġ
        WALL,       // ���� ��ġ? => �ϴ� ������
    }

}

