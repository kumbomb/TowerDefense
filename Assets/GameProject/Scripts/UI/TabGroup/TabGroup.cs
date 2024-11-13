using UnityEngine;
using System.Collections.Generic;

public class TabGroup : MonoBehaviour
{
    public List<TabBtn> tabButtons;
    [SerializeField] Sprite tabIdle;     // 탭 버튼 기본 상태 이미지
    [SerializeField] Sprite tabActive;   // 탭 버튼 선택 상태 이미지
    [SerializeField] TabBtn seletedTab;  // 현재 선택된 탭
    [SerializeField] List<GameObject> tabObjects; // 탭에 따라 표현되야할 오브젝트
    public void Subscribe(TabBtn button)
    {
        if(tabButtons == null)
        {
            tabButtons = new List<TabBtn>();
        }
        tabButtons.Add(button);
    }

    public void OnTabSelected(TabBtn button)
    {
        seletedTab = button;
        ResetTabs();
        button.SetBtnImg(tabActive);
        int index = button.transform.GetSiblingIndex();
        for(int i=0;i<tabObjects.Count;i++)
        {
            tabObjects[i].SetActive(i == index);
        }
    }
    
    public void ResetTabs()
    {
        foreach(TabBtn btn in tabButtons)
        {
            if(seletedTab != null && btn == seletedTab) continue;
            btn.SetBtnImg(tabIdle);
        }
    }
}
