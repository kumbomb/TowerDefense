using BaseEnum;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] Button startBtn;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SettingBtn();
    }

    void SettingBtn()
    {
        startBtn.onClick.AddListener(() => { 
            SceneManager.LoadSceneAsync((int)SCENE_TYPE.GAME);
            LevelManager.Instance.GetLevelData();            
             });
       
    }
}
