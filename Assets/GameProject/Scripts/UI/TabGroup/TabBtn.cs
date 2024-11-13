using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class TabBtn : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] TabGroup tabGroup;
    [SerializeField] Image backGround;

    private void Start() 
    {
        tabGroup.Subscribe(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    public void SetBtnImg(Sprite sprite)
    {
        backGround.sprite = sprite;
    }

    public void Select()
    {

    }
    public void DeSelect()
    {

    }
}
