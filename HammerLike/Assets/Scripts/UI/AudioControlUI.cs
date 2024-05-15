using UnityEngine;
using UnityEngine.UI; // UI 요소(슬라이더 및 버튼 등) 사용을 위해 필요

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
        bgmVolumeSlider.value = B_AudioManager.Instance.GetVolume(AudioCategory.BGM);
        sfxVolumeSlider.value = B_AudioManager.Instance.GetVolume(AudioCategory.SFX);

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
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (!isMuted)
        {
            B_AudioManager.Instance.SetVolume(AudioCategory.SFX, volume);
            savedSFXVolume = volume;
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

public static class AudioManagerExtensions
{
    public static void SetVolume(this B_AudioManager manager, AudioCategory category, float volume)
    {
        if (category == AudioCategory.BGM)
        {
            manager.GetLoopAudioSource().volume = volume;
        }
        else if (category == AudioCategory.SFX)
        {
            foreach (var source in manager.GetOnceAudioSources())
            {
                source.volume = volume;
            }
        }
    }

    public static float GetVolume(this B_AudioManager manager, AudioCategory category)
    {
        if (category == AudioCategory.BGM)
        {
            return manager.GetLoopAudioSource().volume;
        }
        else if (category == AudioCategory.SFX)
        {
            return manager.GetOnceAudioSources().Length > 0 ? manager.GetOnceAudioSources()[0].volume : 0;
        }
        return 0;
    }
}

