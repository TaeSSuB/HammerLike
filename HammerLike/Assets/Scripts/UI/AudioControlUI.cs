using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용하기 위해 추가

public class AudioControlUI : MonoBehaviour
{
    [Header("UI 요소")]
    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider masterVolumeSlider; // 추가된 마스터 볼륨 슬라이더
    public Toggle muteToggleButton;

    [Header("Text 요소")]
    public TextMeshProUGUI bgmVolumeText;
    public TextMeshProUGUI sfxVolumeText;
    public TextMeshProUGUI masterVolumeText;

    private bool isMuted = false;
    private float savedMasterVolume = 1.0f;
    private float savedBGMVolume = 1.0f;
    private float savedSFXVolume = 1.0f;

    private static AudioControlUI instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 현재 설정을 바탕으로 볼륨 슬라이더 초기화
        LoadSettings();

        // UI 요소 초기화
        bgmVolumeSlider.value = savedBGMVolume;
        sfxVolumeSlider.value = savedSFXVolume;
        masterVolumeSlider.value = savedMasterVolume; // 마스터 볼륨 슬라이더 초기화
        muteToggleButton.isOn = isMuted;

        // 텍스트 초기화
        UpdateVolumeTexts();

        // UI 요소에 대한 리스너 연결
        bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume); // 마스터 볼륨 슬라이더 리스너 연결
        muteToggleButton.onValueChanged.AddListener(ToggleMute);
    }

    private void LoadSettings()
    {
        savedMasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        savedBGMVolume = PlayerPrefs.GetFloat("BGMVolume", 1.0f);
        savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;

        if (isMuted)
        {
            B_AudioManager.Instance.SetVolume(AudioCategory.BGM, 0);
            B_AudioManager.Instance.SetVolume(AudioCategory.SFX, 0);
        }
        else
        {
            B_AudioManager.Instance.SetVolume(AudioCategory.BGM, savedBGMVolume * savedMasterVolume);
            B_AudioManager.Instance.SetVolume(AudioCategory.SFX, savedSFXVolume * savedMasterVolume);
        }
    }

    public void SetBGMVolume(float volume)
    {
        if (!isMuted)
        {
            savedBGMVolume = volume;
            PlayerPrefs.SetFloat("BGMVolume", volume);
            PlayerPrefs.Save();
            B_AudioManager.Instance.SetVolume(AudioCategory.BGM, volume * savedMasterVolume);
            UpdateVolumeTexts();
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (!isMuted)
        {
            savedSFXVolume = volume;
            PlayerPrefs.SetFloat("SFXVolume", volume);
            PlayerPrefs.Save();
            B_AudioManager.Instance.SetVolume(AudioCategory.SFX, volume * savedMasterVolume);
            UpdateVolumeTexts();
        }
    }

    public void SetMasterVolume(float volume)
    {
        savedMasterVolume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();

        // 마스터 볼륨을 조정할 때 BGM과 SFX 볼륨 슬라이더도 동일하게 조정
        bgmVolumeSlider.value = volume;
        sfxVolumeSlider.value = volume;
        SetBGMVolume(volume);
        SetSFXVolume(volume);
        UpdateVolumeTexts();
    }

    public void ToggleMute(bool isOn)
    {
        isMuted = isOn;
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();

        if (isMuted)
        {
            B_AudioManager.Instance.SetVolume(AudioCategory.BGM, 0);
            B_AudioManager.Instance.SetVolume(AudioCategory.SFX, 0);
        }
        else
        {
            B_AudioManager.Instance.SetVolume(AudioCategory.BGM, savedBGMVolume * savedMasterVolume);
            B_AudioManager.Instance.SetVolume(AudioCategory.SFX, savedSFXVolume * savedMasterVolume);
        }
        UpdateVolumeTexts();
    }

    private void UpdateVolumeTexts()
    {
        bgmVolumeText.text = (bgmVolumeSlider.value * 100).ToString("F0");
        sfxVolumeText.text = (sfxVolumeSlider.value * 100).ToString("F0");
        masterVolumeText.text = (masterVolumeSlider.value * 100).ToString("F0");
    }
}
