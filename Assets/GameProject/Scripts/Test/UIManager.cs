using BaseEnum;
using UnityEngine;
public class UIManager : Manager<UIManager>
{
    public SelectTowerUI selcetBtn;
    public void Start()
    {
        selcetBtn.gameObject.SetActive(true);
    }
    public void SetdragBtn(TowerType type)
    {

    }
}
