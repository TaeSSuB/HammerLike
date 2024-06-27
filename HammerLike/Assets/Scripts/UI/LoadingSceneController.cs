using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LoadingSceneController : MonoBehaviour
{
    static string nextScene;
    public Image image;
    public float duration;
    [SerializeField]
    Slider progressBar;

    private bool isGenerationComplete = false;

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("Loading");
    }

    void Start()
    {
        StartCoroutine(LoadNextScene());
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
       /* if (scene.name == "Loading")
        {
            StartCoroutine(LoadNextScene());
        }
        else if (scene.name == nextScene)
        {
            BSPGenerator generator = FindObjectOfType<BSPGenerator>();
            if (generator != null)
            {
                generator.OnSuccessGenerate += HandleSuccessGenerate;
                Debug.Log("OnSuccessGenerate event handler registered.");
            }
            else
            {
                Debug.LogError("BSPGenerator not found in the scene.");
            }
        }*/
    }

    IEnumerator LoadNextScene()
    {
        yield return null; // 한 프레임 대기

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0f;
        float minLoadTime = 5f; // 최소 10초 대기

        while (!op.isDone)
        {
            yield return null;
            if (op.progress < 0.9f && !isGenerationComplete)
            {
                progressBar.value = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                progressBar.value = Mathf.Lerp(0.9f, 1f, timer / minLoadTime);
                if (progressBar.value >= 1f) // 최소 10초 대기 후 생성 완료 확인
                {
                    op.allowSceneActivation = true;

                    yield break;
                }
            }
        }
    }

    private void HandleSuccessGenerate()
    {
        Debug.Log("HandleSuccessGenerate called.");
        isGenerationComplete = true;
    }

    public void FadeIn()
    {
        image.DOFade(1, duration).SetEase(Ease.InOutQuad);
    }

    public void FadeOut()
    {
        image.DOFade(0, duration).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            //StartCoroutine(LoadNextScene());
        });
    }
}