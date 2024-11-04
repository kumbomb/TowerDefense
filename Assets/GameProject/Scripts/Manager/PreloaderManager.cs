using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using BaseEnum;

public class PreloaderManager : MonoBehaviour
{
    [SerializeField] GameObject startBtn;
    [SerializeField] GameObject startText;

    [SerializeField] GameObject LoadingGroup; // 로딩바 관련된건 따로 그쪽으로 빼도록 하고

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

    // 여기에서 csv 읽고 데이터 세팅할거 하면서 %를 진행 시킨다.
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
            // 1. CSV 로딩
            loadingUI.UpdateDescription("CSV 파일 로딩 중...");
            await LoadCSV();
            totalProgress += 33.3f;
            loadingUI.SetTargetProgress(totalProgress);

            // 2. 유저 데이터 로딩
            loadingUI.UpdateDescription("유저 데이터 로딩 중...");
            await LoadUserData();
            totalProgress += 33.3f;
            loadingUI.SetTargetProgress(totalProgress);

            // 3. 오브젝트 풀링 초기화
            loadingUI.UpdateDescription("오브젝트 풀링 초기화 중...");
            await InitializeObjectPooling();
            totalProgress += 33.4f;
            loadingUI.SetTargetProgress(totalProgress);

            // 로딩 완료   
            loadingUI.SetTargetProgress(100f);
            loadingUI.UpdateDescription("로딩 완료!");

            // 잠시 대기 후 로비로 전환
            await UniTask.Delay(1000);
            await SceneManager.LoadSceneAsync((int)SCENE_TYPE.LOBBY);
        }
        catch /*(System.Exception ex)*/
        {
            // Debug.LogError($"로딩 중 오류 발생: {ex.Message}");
            Debug.LogError("로딩 중 오류 발생");
            // 에러 처리 로직 추가 (예: 에러 메시지 표시)
        }
    }

    private async UniTask LoadCSV()
    {
        // 실제 CSV 로딩 로직을 여기에 구현하세요
        // 예시로 딜레이를 사용합니다.
        await UniTask.Delay(2000);
    }

    private async UniTask LoadUserData()
    {
        // 실제 유저 데이터 로딩 로직을 여기에 구현하세요
        // 예시로 딜레이를 사용합니다.
        await UniTask.Delay(2000);
    }

    private async UniTask InitializeObjectPooling()
    {
        // 실제 오브젝트 풀링 초기화 로직을 여기에 구현하세요
        // 예시로 딜레이를 사용합니다.
        await UniTask.Delay(2000);
    }
}
