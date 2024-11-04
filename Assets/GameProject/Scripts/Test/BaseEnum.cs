using UnityEngine;

namespace BaseEnum
{
    public enum SCENE_TYPE
    {
        PRELOADER,
        LOBBY,
        GAME,
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

