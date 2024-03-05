using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.SceneManagement;

[System.Serializable] 
public class SceneAudioSource
{
    public string sceneName;
    public AudioSource audioSource;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private List<SceneAudioSource> bgmSourcesList = new List<SceneAudioSource>();

    private Dictionary<string, AudioSource> bgmSources = new Dictionary<string, AudioSource>();
    [SerializeField] private List<AudioSource> sfxSources = new List<AudioSource>();
    public AudioClip[] audioClip;
    public TMP_InputField masterVolumeTextInput;
    public TMP_InputField bgmVolumeTextInput;
    public TMP_InputField sfxVolumeTextInput;

    public Slider masterVolumeSlider; 
    public Slider bgmVolumeSlider; 
    public Slider sfxVolumeSlider; 


    private float masterVolume = 1f;
    private float bgmVolume = 1f;
    private float sfxVolume = 1f;
    private bool isMuted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        foreach(var item in bgmSourcesList)
        {
            if(!bgmSources.ContainsKey(item.sceneName))
            {
                bgmSources.Add(item.sceneName, item.audioSource);
            }
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        LoadVolumeSettings();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!bgmSources.ContainsKey(scene.name))
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            // Configure your AudioSource here if needed (loop, playOnAwake, etc.)
            bgmSources.Add(scene.name, newSource);
        }

        PlayBGMForCurrentScene();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void PlayBGMForCurrentScene()
    {
        // 새 씬이 로드될 때 모든 현재 재생 중인 BGM을 정지합니다.
        StopAllBGM();

        // 현재 씬 이름을 가져옵니다.
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 새 씬에 해당하는 BGM을 찾아 재생합니다.
        if (bgmSources.TryGetValue(currentSceneName, out AudioSource currentBGM))
        {
            if (currentBGM.clip != null)
            {
                currentBGM.Play();
            }
        }
    }

    // 모든 BGM을 정지하는 메서드입니다.
    private void StopAllBGM()
    {
        foreach (var bgmSource in bgmSources.Values)
        {
            if (bgmSource.isPlaying)
            {
                bgmSource.Stop();
            }
        }
    }

    private void Start()
    {
        // 현재 볼륨 값에 100을 곱하여 문자열로 변환 후, 각 TMP_InputField에 설정
        if (SceneManager.GetActiveScene().name == "UI")
        {
            masterVolumeTextInput.text = (masterVolume * 100).ToString("0");
            bgmVolumeTextInput.text = (bgmVolume * 100).ToString("0");
            sfxVolumeTextInput.text = (sfxVolume * 100).ToString("0");

            // 슬라이더의 값도 현재 볼륨에 맞춰 설정
            masterVolumeSlider.value = masterVolume;
            bgmVolumeSlider.value = bgmVolume;
            sfxVolumeSlider.value = sfxVolume;
        }
    }



    // 문자열 입력을 받아 마스터 볼륨 설정
    public void SetMasterValue(string value)
    {
        if (float.TryParse(value, out float volume))
        {
            SetMasterVolume(volume / 100f); // 0에서 100 사이의 값으로 변환
            masterVolumeSlider.value = volume / 100f; // 슬라이더 값 업데이트
        }
    }

    // 문자열 입력을 받아 배경음악 볼륨 설정
    public void SetBGMValue(string value)
    {
        if (float.TryParse(value, out float volume))
        {
            SetBGMVolume(volume / 100f); // 0에서 100 사이의 값으로 변환
            bgmVolumeSlider.value = volume / 100f; // 슬라이더 값 업데이트
        }
    }

    // 문자열 입력을 받아 효과음 볼륨 설정
    public void SetSFXValue(string value)
    {
        if (float.TryParse(value, out float volume))
        {
            SetSFXVolume(volume / 100f); // 0에서 100 사이의 값으로 변환
            sfxVolumeSlider.value = volume / 100f; // 슬라이더 값 업데이트
        }
    }


    public void PlaySFX(AudioClip clip)
    {
        AudioSource freeSource = sfxSources.Find(source => !source.isPlaying);
        if (freeSource == null)
        {
            freeSource = gameObject.AddComponent<AudioSource>();
            sfxSources.Add(freeSource);
        }
        freeSource.PlayOneShot(clip, sfxVolume * masterVolume);
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        string textValue = (volume * 100f).ToString("0");
        masterVolumeTextInput.text = textValue;
        UpdateAllVolumes();
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        string textValue = (volume * 100f).ToString("0");
        bgmVolumeTextInput.text = textValue;
        UpdateAllVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        string textValue = (volume * 100f).ToString("0");
        sfxVolumeTextInput.text = textValue;
        
        UpdateAllVolumes();
    }

    private void UpdateAllVolumes()
    {
        foreach (var bgmSource in bgmSources.Values)
        {
            bgmSource.volume = isMuted ? 0 : bgmVolume * masterVolume;
        }
        sfxSources.ForEach(source => source.volume = isMuted ? 0 : sfxVolume * masterVolume);
    }

    public void ToggleMute(bool mute)
    {
        isMuted = mute;
        UpdateAllVolumes();
    }

    public void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void OnSFXVolumeChangeComplete()
    {
        // 볼륨 설정을 저장하지 않고, 사용자가 볼륨 조정을 완료했을 때 테스트 사운드를 재생
        // 예시로, sfxSources 리스트의 첫 번째 AudioSource를 사용하여 테스트 사운드 재생
        // 실제 사용 시에는 적절한 테스트 사운드 클립을 선택하거나, 특정 조건에 맞는 사운드를 재생할 수 있습니다.
        if (sfxSources.Count > 0 && audioClip.Length > 0)
        {
            sfxSources[0].PlayOneShot(audioClip[0], sfxVolume * masterVolume); // 첫 번째 오디오 클립을 예시로 사용
        }
    }

    private void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;
        UpdateAllVolumes();
    }

    public void ResetToDefaultSettings()
    {
        SetMasterVolume(1f);
        SetBGMVolume(1f);
        SetSFXVolume(1f);
        ToggleMute(false);
        SaveVolumeSettings();
    }

    
}
