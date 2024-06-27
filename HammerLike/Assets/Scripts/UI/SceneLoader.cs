using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneLoader : MonoBehaviour
{
    public Image targetImage; // 투명도를 조절할 Image 컴포넌트
    public float duration = 1.0f; // 페이드 인/아웃에 걸리는 시간

    public GameObject deadPanel;
    public GameObject clearPanel;

    void Start()
    {
        if (!targetImage.gameObject.activeSelf)
        {
            targetImage.gameObject.SetActive(true);
        }
        // 시작 시 페이드 아웃 실행
        FadeOut();

        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Town")
            B_AudioManager.Instance.PlaySound(AudioCategory.BGM, AudioTag.Town);
        else if (currentSceneName == "Mainmenu")
            B_AudioManager.Instance.PlaySound(AudioCategory.BGM, AudioTag.MainMenu);
        else
            B_AudioManager.Instance.PlaySound(AudioCategory.BGM, AudioTag.Battle);
    }

    public void ChangeScene(string sceneName)
    {
        if (Time.timeScale == 0)
            Time.timeScale = 1;
        LoadingSceneController.LoadScene(sceneName);
    }

    public void ChangeSceneOther(string sceneName)
    {
        if (Time.timeScale == 0)
            Time.timeScale = 1;
        StartCoroutine(ChangeSceneName(sceneName));
    }

    IEnumerator ChangeSceneName(string sceneName)
    {
        if (Time.timeScale == 0)
            Time.timeScale = 1;
        FadeInAndChangeScene(sceneName);
        yield return null;
    }

    IEnumerator ChangeTown()
    {
        FadeInAndChangeScene("Town");
        yield return null;
    }

    public void NextChangeTown()
    {
        StartCoroutine(ChangeTown());
    }

    public void FadeInAndChangeScene(string sceneName)
    {
        targetImage.DOFade(1, duration).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            LoadingSceneController.LoadScene(sceneName);
        }).SetUpdate(true);
    }

    public void FadeIn()
    {
        targetImage.DOFade(1, duration).SetEase(Ease.InOutQuad).SetUpdate(true);
    }

    public void FadeOut()
    {
        targetImage.DOFade(0, duration).SetEase(Ease.InOutQuad).SetUpdate(true);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void PlayerDead()
    {
        FadeIn();

        if (deadPanel != null && !deadPanel.gameObject.activeSelf)
        {
            deadPanel.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void BossDead()
    {
        FadeIn();

        if (clearPanel != null && !clearPanel.gameObject.activeSelf)
        {
            clearPanel.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public bool IsCurrentSceneTown()
    {
        return SceneManager.GetActiveScene().name == "Town";
    }
}
