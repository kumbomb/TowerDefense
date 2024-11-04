using System;
using UnityEngine;
using BaseStruct;
using BaseEnum;

[Serializable]
public class GridCell : IComparable<GridCell>
{
    public Vector3 position;
    public Coord index;
    public bool isWalkable = true;
    [SerializeField] public PLACEMENTSTATE placeState = PLACEMENTSTATE.NONE;
    [SerializeField] public PLACEMENTTYPE placeType = PLACEMENTTYPE.NONE;

    [NonSerialized] public GameObject cellObject; // 셀을 나타내는 GameObject
    [NonSerialized] public GameObject placedObject;
    [NonSerialized] SpriteRenderer cellSprite;

    public PLACEMENTSTATE PlaceState
    {
        get
        {
            return placeState;
        }
        set
        {
            placeState = value;
            UpdateGridColor();
        }
    }
    public PLACEMENTTYPE PlaceType
    {
        get
        {
            return placeType;
        }
        set
        {
            placeType = value;
            UpdatePlaceMentType(placeType);
        }
    }

    #region A* 전용 필드
    [NonSerialized] public float G; // 시작점부터 이 노드까지의 비용
    [NonSerialized] public float H; // 이 노드부터 목표점까지의 휴리스틱 비용
    [NonSerialized] public GridCell Parent; // 경로 복원을 위한 부모 노드

    public float F => G + H; // 총 비용
    #endregion

    public GridCell(Vector3 pos, Coord idx)
    {
        position = pos;
        index = idx;
        placedObject = null;
    }

    public void SetCellObject(GameObject _obj)
    {
        cellObject = _obj;
        if (cellSprite == null && cellObject != null)
            cellSprite = _obj.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    #region 셀에 대한 판단 처리

    // 몬스터가 이동이 가능한 셀인지 
    public bool IsWalkable()
    {
        return isWalkable &&
               placeType != PLACEMENTTYPE.OBSTACLE &&
               placeType != PLACEMENTTYPE.WALL;
    }
    // 오브젝트를 배치할 수 있는 셀인지
    public bool IsCanPlace()
    {
        if (isWalkable == false)
            return false;

        if (placeState == PLACEMENTSTATE.IMPLACABLE ||
            placeState == PLACEMENTSTATE.HIDDEN ||
            placeState == PLACEMENTSTATE.MONSTERSPAWN ||
            placeState == PLACEMENTSTATE.ENDPOINT)
            return false;

        if (placeType == PLACEMENTTYPE.OBSTACLE ||
            placeType == PLACEMENTTYPE.FLOOR ||
            placeType == PLACEMENTTYPE.WALL)
            return false;

        return true;
    }

    // 셀의 이동 가능 여부를 설정하고 시각적 업데이트를 수행
    public void SetWalkable(bool walkable)
    {
        isWalkable = walkable;
        UpdateGridColor();
    }

    // 셀의 상태에 따른 색상 업데이트
    public void UpdateGridColor()
    {
        if (cellSprite == null && cellObject != null)
            cellSprite = cellObject.transform.GetChild(0).GetComponent<SpriteRenderer>();

        if (cellSprite == null)
            return;

        if (Application.isPlaying)
        {
            // 인게임에서는 특정 상태에 따라 처리
            switch (placeState)
            {
                case PLACEMENTSTATE.NONE:
                    cellSprite.color = new Color(1, 1, 1, 1);
                    break;
                case PLACEMENTSTATE.PLACABLE:
                    cellSprite.color = Color.green;
                    break;
                case PLACEMENTSTATE.IMPLACABLE:
                    cellSprite.color = Color.red;
                    break;
                case PLACEMENTSTATE.MONSTERSPAWN:
                case PLACEMENTSTATE.ENDPOINT:
                case PLACEMENTSTATE.STARTPOINT:
                case PLACEMENTSTATE.HIDDEN:
                    cellSprite.color = new Color(1, 1, 1, 0);
                    break;
                default:
                    cellSprite.color = Color.white;
                    break;
            }
        }
        else
        {
            // 에디터에서는 각 상태에 따라 색상 설정
            switch (placeState)
            {
                case PLACEMENTSTATE.NONE:
                    cellSprite.color = new Color(1, 1, 1, 1);
                    break;
                case PLACEMENTSTATE.PLACABLE:
                    cellSprite.color = Color.green;
                    break;
                case PLACEMENTSTATE.IMPLACABLE:
                    cellSprite.color = Color.red;
                    break;
                case PLACEMENTSTATE.MONSTERSPAWN:
                    cellSprite.color = Color.yellow; // 몬스터 스폰 상태일 때
                    break;
                case PLACEMENTSTATE.ENDPOINT:
                    cellSprite.color = Color.magenta; // 엔드포인트 상태일 때
                    break;
                case PLACEMENTSTATE.STARTPOINT:
                    cellSprite.color = Color.cyan; // 스타트포인트 상태일 때
                    break;
                case PLACEMENTSTATE.HIDDEN:
                    cellSprite.color = new Color(1, 1, 1, 0f); // 히든 상태일 때 투명하게
                    break;
                default:
                    cellSprite.color = Color.white;
                    break;
            }
        }
    }

    void UpdatePlaceMentType(PLACEMENTTYPE placeType)
    {
        switch (placeType)
        {
            case PLACEMENTTYPE.NONE:
                isWalkable = true;
                break;
            case PLACEMENTTYPE.FLOOR:
                isWalkable = true;
                break;
            case PLACEMENTTYPE.OBSTACLE:
                isWalkable = false;
                break;
            case PLACEMENTTYPE.WALL:
                isWalkable = false;
                break;
        }
    }

    #endregion

    // IComparable<GridCell> 인터페이스 구현
    public int CompareTo(GridCell other)
    {
        if (other == null) return 1;

        int compare = this.F.CompareTo(other.F);
        if (compare == 0)
        {
            // F 값이 같다면 H 값으로 추가 비교
            compare = this.H.CompareTo(other.H);
        }
        return compare;
    }
}
