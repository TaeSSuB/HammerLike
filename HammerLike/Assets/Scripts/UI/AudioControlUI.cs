using UnityEngine;
using UnityEngine.UI;

public class AudioControlUI : MonoBehaviour
{
    [Header("UI 요소")]
    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle muteToggleButton;

    private bool isMuted = false;
    private float savedBGMVolume = 1.0f;
    private float savedSFXVolume = 1.0f;

    private void Start()
    {
        // 현재 설정을 바탕으로 볼륨 슬라이더 초기화
        savedBGMVolume = PlayerPrefs.GetFloat("BGMVolume", 1.0f);
        savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);

        bgmVolumeSlider.value = savedBGMVolume;
        sfxVolumeSlider.value = savedSFXVolume;

        // UI 요소에 대한 리스너 연결
        bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        muteToggleButton.onValueChanged.AddListener(ToggleMute);
    }

    public void SetBGMVolume(float volume)
    {
        if (!isMuted)
        {
            B_AudioManager.Instance.SetVolume(AudioCategory.BGM, volume);
            savedBGMVolume = volume;
            PlayerPrefs.SetFloat("BGMVolume", volume);
            PlayerPrefs.Save();
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (!isMuted)
        {
            B_AudioManager.Instance.SetVolume(AudioCategory.SFX, volume);
            savedSFXVolume = volume;
            PlayerPrefs.SetFloat("SFXVolume", volume);
            PlayerPrefs.Save();
        }
    }

    public void ToggleMute(bool isOn)
    {
        isMuted = isOn;
        if (isMuted)
        {
            B_AudioManager.Instance.SetVolume(AudioCategory.BGM, 0);
            B_AudioManager.Instance.SetVolume(AudioCategory.SFX, 0);
        }
        else
        {
            B_AudioManager.Instance.SetVolume(AudioCategory.BGM, savedBGMVolume);
            B_AudioManager.Instance.SetVolume(AudioCategory.SFX, savedSFXVolume);
        }
    }
}
