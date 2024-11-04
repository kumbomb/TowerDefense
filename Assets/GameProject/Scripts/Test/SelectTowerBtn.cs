using BaseEnum;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectTowerBtn : MonoBehaviour, IPointerDownHandler
{
    public int index;
    public TowerType type;
    public Button button;
    public void Awake()
    {
        button.onClick.AddListener(OnButtonClick);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        DragAndDropManager.Instance.StartDrag(type);
    }

    void OnButtonClick()
    {
        
    }
}
