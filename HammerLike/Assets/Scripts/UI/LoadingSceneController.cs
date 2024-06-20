using System.Collections;
using System.Collections.Generic;
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

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("Loading");
    }
    // Start is called before the first frame update
    void Start()
    {
        if(!image.gameObject.activeSelf)
        {
            image.gameObject.SetActive(true);
        }
        FadeOut();
        StartCoroutine(LoadSceneProcess());
    }

    IEnumerator LoadSceneProcess()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0f;
        while (!op.isDone)
        {
            yield return null;

            if (op.progress < 0.9f)
            {
                progressBar.value = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                progressBar.value = Mathf.Lerp(0.9f, 1f, timer / 3f); // 3초 동안 부드럽게 채우기
                if (timer > 3f) // 최소 3초 대기
                {
                    op.allowSceneActivation = true;
                    FadeIn();
                    yield break;
                }
            }
        }
    }

    public void FadeIn()
    {
        // DOTween을 사용한 페이드 인: 투명도를 0에서 1로 변경
        image.DOFade(1, duration).SetEase(Ease.InOutQuad);
    }

    public void FadeOut()
    {
        // DOTween을 사용한 페이드 아웃: 투명도를 1에서 0으로 변경
        image.DOFade(0, duration).SetEase(Ease.InOutQuad);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
