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

    [SerializeField] GameObject LoadingGroup; // �ε��� ���õȰ� ���� �������� ������ �ϰ�

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

    // ���⿡�� csv �а� ������ �����Ұ� �ϸ鼭 %�� ���� ��Ų��.
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
            // 1. CSV �ε�
            loadingUI.UpdateDescription("CSV ���� �ε� ��...");
            await LoadCSV();
            totalProgress += 33.3f;
            loadingUI.SetTargetProgress(totalProgress);

            // 2. ���� ������ �ε�
            loadingUI.UpdateDescription("���� ������ �ε� ��...");
            await LoadUserData();
            totalProgress += 33.3f;
            loadingUI.SetTargetProgress(totalProgress);

            // 3. ������Ʈ Ǯ�� �ʱ�ȭ
            loadingUI.UpdateDescription("������Ʈ Ǯ�� �ʱ�ȭ ��...");
            await InitializeObjectPooling();
            totalProgress += 33.4f;
            loadingUI.SetTargetProgress(totalProgress);

            // �ε� �Ϸ�   
            loadingUI.SetTargetProgress(100f);
            loadingUI.UpdateDescription("�ε� �Ϸ�!");

            // ��� ��� �� �κ�� ��ȯ
            await UniTask.Delay(1000);
            await SceneManager.LoadSceneAsync((int)SCENE_TYPE.LOBBY);
        }
        catch /*(System.Exception ex)*/
        {
            // Debug.LogError($"�ε� �� ���� �߻�: {ex.Message}");
            Debug.LogError("�ε� �� ���� �߻�");
            // ���� ó�� ���� �߰� (��: ���� �޽��� ǥ��)
        }
    }

    private async UniTask LoadCSV()
    {
        // ���� CSV �ε� ������ ���⿡ �����ϼ���
        // ���÷� �����̸� ����մϴ�.
        await UniTask.Delay(2000);
    }

    private async UniTask LoadUserData()
    {
        // ���� ���� ������ �ε� ������ ���⿡ �����ϼ���
        // ���÷� �����̸� ����մϴ�.
        await UniTask.Delay(2000);
    }

    private async UniTask InitializeObjectPooling()
    {
        // ���� ������Ʈ Ǯ�� �ʱ�ȭ ������ ���⿡ �����ϼ���
        // ���÷� �����̸� ����մϴ�.
        await UniTask.Delay(2000);
    }
}
