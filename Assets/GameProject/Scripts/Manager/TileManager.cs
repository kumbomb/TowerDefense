//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//public class TileManager : Singleton<TileManager>
//{
//    public int gridWidth;
//    public int gridHeight;
//    public float tileSize;
//    public GameObject tilePrefab;
//    private Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();

//    private void Start()
//    {
//        GenerateGrid();
//    }

//    void GenerateGrid()
//    {
//        for (int x = 0; x < gridWidth; x++)
//        {
//            for (int z = 0; z < gridHeight; z++)
//            {
//                Vector3 position = new Vector3(x * tileSize, 0, z * tileSize);
//                GameObject tileObject = Instantiate(tilePrefab, position, Quaternion.identity, transform);
//                Tile tile = tileObject.GetComponent<Tile>();
//                tile.gridPosition = new Vector2Int(x, z);
//                tiles.Add(tile.gridPosition, tile);
//            }
//        }
//    }

//    public Tile GetTileAtPosition(Vector2Int position)
//    {
//        tiles.TryGetValue(position, out Tile tile);
//        return tile;
//    }

//    // 타일의 배치 가능 여부를 확인하는 함수
//    public bool CanPlaceObject(Vector2Int[] occupiedPositions)
//    {
//        foreach (var pos in occupiedPositions)
//        {
//            Tile tile = GetTileAtPosition(pos);
//            if (tile == null || tile.isOccupied)
//                return false;
//        }
//        return true;
//    }

//    // 타일의 상태를 업데이트하는 함수
//    public void SetTilesOccupied(Vector2Int[] positions, bool occupied)
//    {
//        foreach (var pos in positions)
//        {
//            Tile tile = GetTileAtPosition(pos);
//            if (tile != null)
//                tile.isOccupied = occupied;
//        }
//    }

//    //타일의 색상 변경 처리
//    public void HighlightTiles(Vector2Int[] positions, bool canPlace)
//    {
//        Color color = canPlace ? Color.green : Color.red;
//        foreach (var pos in positions)
//        {
//            Tile tile = GetTileAtPosition(pos);
//            if (tile != null)
//                tile.SetColor(color);
//        }
//    }

//    #region 에디터 전용 스크립트

//#if UNITY_EDITOR
//    public void GenerateGridInEditor()
//    {
//        ClearGridInEditor();

//        for (int x = 0; x < gridWidth; x++)
//        {
//            for (int z = 0; z < gridHeight; z++)
//            {
//                Vector3 position = new Vector3(x * tileSize, 0, z * tileSize);
//                GameObject tileObject = PrefabUtility.InstantiatePrefab(tilePrefab, transform) as GameObject;
//                tileObject.transform.position = position;
//                Tile tile = tileObject.GetComponent<Tile>();
//                tile.gridPosition = new Vector2Int(x, z);
//                tiles.Add(tile.gridPosition, tile);
//            }
//        }
//    }

//    public void ClearGridInEditor()
//    {
//        // 모든 자식 오브젝트 삭제
//        for (int i = transform.childCount - 1; i >= 0; i--)
//        {
//            DestroyImmediate(transform.GetChild(i).gameObject);
//        }
//        tiles.Clear();
//    }
//#endif

//    #endregion
//}
