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
        if (!image.gameObject.activeSelf)
        {
            //image.gameObject.SetActive(true);
        }
        StartCoroutine(LoadSceneProcess());
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
        /*if (scene.name == nextScene)
        {*/
            if (scene.name == "Stage01")
            {
                BSPGenerator generator = FindObjectOfType<BSPGenerator>();
                if (generator != null)
                {
                    generator.OnSuccessGenerate += HandleSuccessGenerate;
                }
            }
        //}
    }

    IEnumerator LoadSceneProcess()
    {
        //yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync("Stage01");
        op.allowSceneActivation = false;

        float timer = 0f;
        while (!op.isDone)
        {
            yield return null;
            
            if (op.progress < 0.9f )
            {
                progressBar.value = op.progress;
                
            }
            
            else
            {
                timer += Time.unscaledDeltaTime;
                progressBar.value = Mathf.Lerp(0.9f, 1f, timer / 10f); //끝나면 바로 시작 
                if (timer > 10f) // 최소 3초 대기 후 생성 완료 확인
                {
                    op.allowSceneActivation = true;

                    break;
                }
            }
        }

        // 이벤트 구독 해제
        BSPGenerator generator = FindObjectOfType<BSPGenerator>();
        if (generator != null)
        {
            generator.OnSuccessGenerate -= HandleSuccessGenerate;
        }
       
    }

    private void HandleSuccessGenerate()
    {
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
            StartCoroutine(LoadSceneProcess());
        });
    }
}
