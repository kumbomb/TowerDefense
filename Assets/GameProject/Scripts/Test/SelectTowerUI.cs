using BaseEnum;
using UnityEngine;
using UnityEngine.UI;

public class SelectTowerUI : MonoBehaviour
{
    public SelectTowerBtn[] selectTowerBtn;

    public TowerType towerType; // 소환할 타워 타입을 설정

    public Button spawnButton;
    private DragAndDropManager dragAndDropManager;

    void Start()
    {
        for (int i = 0; i < selectTowerBtn.Length; i++)
        {
            selectTowerBtn[i].index = i;
        }
    }
}
