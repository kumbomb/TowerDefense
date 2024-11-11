using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using BaseEnum;

public class PreloaderManager : MonoBehaviour
{
    [SerializeField] GameObject startBtn;
    [SerializeField] GameObject startText;

    [SerializeField] GameObject LoadingGroup; 
    Button btnStart;
    LoadingUI loadingUI;


    private void Start()
    {
        btnStart ??= startBtn.GetComponent<Button>();
        loadingUI ??= LoadingGroup.GetComponent<LoadingUI>();

        InitScreen();
    }

    void InitScreen()
    {
        LoadingGroup.SetActive(false);
        startBtn.SetActive(true);
        SetBtnActions();
    }

    void SetBtnActions()
    {
        btnStart.onClick.AddListener(StartLoadProcess);
    }

    void StartLoadProcess()
    {
        startText.SetActive(false);
        startBtn.SetActive(false);
        LoadingGroup.SetActive(true);

        LoadAsyncOperation().Forget();
    }

    private async UniTaskVoid LoadAsyncOperation()
    {
        float totalProgress = 0f;

        try
        {
            await LoadCSV();
            totalProgress += 25f;
            loadingUI.SetTargetProgress(totalProgress);
            loadingUI.UpdateDescription("CSV 로딩 중...");

            await LoadUserData();
            totalProgress += 25f;
            loadingUI.SetTargetProgress(totalProgress);
            loadingUI.UpdateDescription("유저 데이터 로딩 중...");

            await InitializeObjectPooling();
            totalProgress += 25f;
            loadingUI.SetTargetProgress(totalProgress);
            loadingUI.UpdateDescription("오브젝트 풀링 진행 중...");

            await LoadLevelData();
            totalProgress += 25f;
            loadingUI.SetTargetProgress(totalProgress);
            loadingUI.UpdateDescription("씬 데이터 로딩 중...");

            loadingUI.SetTargetProgress(100f);
            loadingUI.UpdateDescription("데이터 정리 중...");

            await UniTask.Delay(1000);
            loadingUI.UpdateDescription("로딩 완료!");

            await SceneManager.LoadSceneAsync((int)SCENE_TYPE.LOBBY);
        }
        catch /*(System.Exception ex)*/
        {
            Debug.LogError("로딩에 실패했습니다");
        }
    }

    private async UniTask LoadCSV()
    {
        await GoogleSheetManager.Instance.FetchGoogleSheet();
    }


    private async UniTask LoadUserData()
    {
        await UniTask.Delay(2000);
    }

    private async UniTask LoadLevelData(){
        if(LevelManager.Instance != null)
        {
            LevelManager.Instance.InitLevelManager();
        }
    }

    private async UniTask InitializeObjectPooling()
    {
        await UniTask.Delay(2000);
    }
}
