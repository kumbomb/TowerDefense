using BaseEnum;
using BaseStruct;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelManager : Singleton<LevelManager>
{
    //여기에서 테스트로 읽어온다
    [SerializeField] GameObject Level_1_Prefab;

    bool isInit = false;
    StageData curStageData;

    public void InitLevelManager(){
        if(isInit) return;
        isInit = true;
    }
    
    //읽어오는 데이터로 읽어올 씬을 결정한다.
    public void GetLevelData(){
        var stageDatas = CSVManager.Instance.GetDataList<StageData>(TABLE_TYPE.TABLE_STAGE);
        curStageData = stageDatas.Find(t => t.Id == 1);
        if (curStageData == null)
        {
            Debug.Log("No Found Correct Stage Data");
            return;
        }
        else
        {
            string stageSceneName = curStageData.SceneName;
            Debug.Log("SceneName : " + stageSceneName);
            // SceneLoader를 통해 씬 로드
            SceneLoader.Instance.LoadScene(stageSceneName, LoadSceneMode.Additive, OnStageSceneLoaded);
        }
    }

    void OnStageSceneLoaded()
    {
        Scene stageScene = SceneManager.GetSceneByName(curStageData.SceneName);

        if (stageScene.IsValid())
        {
            // 필요한 경우 Game 씬을 액티브 씬으로 설정
            //SceneManager.SetActiveScene(SceneManager.GetSceneByName("Game"));
            GameObject levelObj = Instantiate(Level_1_Prefab);           
            // 추가 초기화 작업
            Debug.Log($"Stage scene '{curStageData.SceneName}' loaded successfully.");
        }
        else
        {
            Debug.LogError($"Failed to load stage scene '{curStageData.SceneName}'.");
        }
    }
}
