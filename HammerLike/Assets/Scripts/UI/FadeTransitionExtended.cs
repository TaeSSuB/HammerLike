using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum FadeOption
{
    FadeIn,
    FadeOut,
    Both
}

[System.Serializable]
public class ImageFadeSettings
{
    public Image image;
    public float fadeDuration = 2f;
    public float displayDuration = 3f;
    public FadeOption fadeOption = FadeOption.FadeIn;
    public AudioClip soundClip; // 사운드 클립을 위한 필드 추가
}

public class FadeTransitionExtended : MonoBehaviour
{
    public ImageFadeSettings[] imagesSettings;
    public int selectedElement = 0;
    private AudioSource audioSource;
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        StartCoroutine(FadeImagesSequence());

    }

    IEnumerator FadeImagesSequence()
    {
        foreach (var setting in imagesSettings)
        {
            // Fade In
            if (setting.fadeOption == FadeOption.FadeIn || setting.fadeOption == FadeOption.Both)
            {
                PlaySound(setting.soundClip); // 사운드 재생
                setting.image.gameObject.SetActive(true);
                yield return StartCoroutine(FadeImage(setting.image, true, setting.fadeDuration));
            }

            // 이미지 표시 지속 시간
            yield return new WaitForSeconds(setting.displayDuration);

            // Fade Out
            if (setting.fadeOption == FadeOption.FadeOut || setting.fadeOption == FadeOption.Both)
            {
                PlaySound(setting.soundClip); // 사운드 재생
                yield return StartCoroutine(FadeImage(setting.image, false, setting.fadeDuration));
                setting.image.gameObject.SetActive(false);
            }
        }

        // 마지막 이미지 활성화 유지
        if (imagesSettings.Length > 0 && imagesSettings[imagesSettings.Length - 1].fadeOption != FadeOption.FadeOut)
        {
            imagesSettings[imagesSettings.Length - 1].image.gameObject.SetActive(true);
        }
    }

    IEnumerator FadeImage(Image image, bool fadeIn, float duration)
    {
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            image.color = new Color(1, 1, 1, newAlpha);
            yield return null;
        }

        image.color = new Color(1, 1, 1, endAlpha);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip); // 오디오 클립 재생
        }
    }
}
