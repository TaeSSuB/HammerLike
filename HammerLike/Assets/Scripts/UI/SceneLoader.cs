using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneLoader : MonoBehaviour
{
    public Image targetImage; // 투명도를 조절할 Image 컴포넌트
    public float duration = 1.0f; // 페이드 인/아웃에 걸리는 시간
    // public string sceneToLoad;

    void Start()
    {
        // 시작 시 페이드 아웃 실행
        FadeOut();
    }
    /*public void ChangeScene()
    {
        // sceneToLoad에 설정된 씬으로 이동합니다.
        LoadingSceneController.LoadScene(sceneToLoad);
    }*/

    public void ChangeScene(string sceneName)
    {
        // sceneToLoad에 설정된 씬으로 이동합니다.
        LoadingSceneController.LoadScene(sceneName);
    }

    public void ChangeSceneOther(string sceneName)
    {
        StartCoroutine(ChangeSceneName(sceneName));
    }

    IEnumerator ChangeSceneName(string sceneName)
    {
        FadeIn();
        yield return new WaitForSeconds(1f);
        ChangeScene(sceneName);
    }

    IEnumerator ChangeTown()
    {
        FadeIn();
        yield return new WaitForSeconds(1f);
        ChangeScene("Town");
    }

    public void NextChangeTown()
    {
        StartCoroutine(ChangeTown());
    }
    
    public void FadeIn()
    {
        // DOTween을 사용한 페이드 인: 투명도를 0에서 1로 변경
        targetImage.DOFade(1, duration).SetEase(Ease.InOutQuad);
    }

    public void FadeOut()
    {
        // DOTween을 사용한 페이드 아웃: 투명도를 1에서 0으로 변경
        targetImage.DOFade(0, duration).SetEase(Ease.InOutQuad);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
