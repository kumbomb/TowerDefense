using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{

    // 단일 씬 로드 메서드 추가
    public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, System.Action onLoaded = null)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName, mode, onLoaded));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, LoadSceneMode mode, System.Action onLoaded)
    {
        // 로딩 화면 활성화 (필요 시)
        // LoadingScreen.Instance.Show();

        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, mode);

            while (!asyncLoad.isDone)
            {
                // 로딩 진행률 업데이트 (필요 시)
                yield return null;
            }

            // 액티브 씬 설정 (LoadSceneMode.Single인 경우 자동 설정됨)
            if (mode == LoadSceneMode.Additive)
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            }

            // 로딩 화면 비활성화 (필요 시)
            // LoadingScreen.Instance.Hide();

            // 로드 완료 후 콜백 실행
            onLoaded?.Invoke();
        }
        else
        {
            Debug.LogError($"Scene '{sceneName}' cannot be found. Please check your build settings.");
        }
    }

    // 기존의 여러 씬 로드 메서드 유지
    public void LoadScenes(string[] sceneNames)
    {
        StartCoroutine(LoadScenesCoroutine(sceneNames));
    }

    private IEnumerator LoadScenesCoroutine(string[] sceneNames)
    {
        // 로딩 화면 활성화 (필요 시)
        // LoadingScreen.Instance.Show();

        // 첫 번째 씬 로드
        if (sceneNames.Length > 0)
        {
            string firstScene = sceneNames[0];
            if (Application.CanStreamedLevelBeLoaded(firstScene))
            {
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(firstScene, LoadSceneMode.Single);
                while (!asyncLoad.isDone)
                {
                    // 로딩 진행률 업데이트 (필요 시)
                    yield return null;
                }
            }
            else
            {
                Debug.LogError($"Scene '{firstScene}' cannot be found. Please check your build settings.");
                yield break;
            }
        }

        // 나머지 씬 로드
        for (int i = 1; i < sceneNames.Length; i++)
        {
            string sceneName = sceneNames[i];
            if (Application.CanStreamedLevelBeLoaded(sceneName))
            {
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                while (!asyncLoad.isDone)
                {
                    // 로딩 진행률 업데이트 (필요 시)
                    yield return null;
                }
            }
            else
            {
                Debug.LogError($"Scene '{sceneName}' cannot be found. Please check your build settings.");
                continue;
            }
        }

        // 액티브 씬 설정
        if (sceneNames.Length > 0)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneNames[0]));
        }

        // 로딩 화면 비활성화 (필요 시)
        // LoadingScreen.Instance.Hide();

        // 추가 초기화 작업 (필요 시)
    }
}
