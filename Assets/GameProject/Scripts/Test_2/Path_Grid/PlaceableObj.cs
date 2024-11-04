using BaseEnum;
using BaseStruct;
using UnityEngine;

public class PlaceableObj : MonoBehaviour
{
    public Coord index;
    public Vector2Int size;
    public bool isDestructible;
    public PLACEMENTTYPE placeType;
    public GameObject previewCell;
}
