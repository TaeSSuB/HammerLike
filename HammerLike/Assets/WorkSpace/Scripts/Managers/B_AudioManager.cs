using UnityEngine;
using System.Linq;
using NuelLib;
using UnityEngine.SceneManagement;
public enum AudioCategory { BGM, SFX }
public enum AudioTag
{
    MainMenu,
    Battle,
    Ambient,
    UI,
    Death,
    PickUp,
    Crash,
    Town,
    SlimeDeath,
    Click
}


public class B_AudioManager : SingletonMonoBehaviour<B_AudioManager>
{
    public SO_AudioSet audioSet; // Assign in the Unity editor

    // BGM, SFX source
    [SerializeField] private int audioSourceOnceCount = 5;
    private AudioSource[] audioSourceOnces; // For SFX
    private AudioSource audioSourceLoop; // For BGM

    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    public AudioSource GetLoopAudioSource()
    {
        return audioSourceLoop;
    }

    public AudioSource[] GetOnceAudioSources()
    {
        return audioSourceOnces;
    }

    protected override void Awake()
    {
        base.Awake();

        audioSourceOnces = new AudioSource[audioSourceOnceCount];
        for (int i = 0; i < audioSourceOnceCount; i++)
        {
            audioSourceOnces[i] = gameObject.AddComponent<AudioSource>();
        }

        audioSourceLoop = gameObject.AddComponent<AudioSource>();

        // Load saved volumes
        float bgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1.0f);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1.0f);

        SetVolume(AudioCategory.BGM, bgmVolume, false);
        SetVolume(AudioCategory.SFX, sfxVolume, false);
    }

    public void SetVolume(AudioCategory category, float volume, bool save = true)
    {
        if (category == AudioCategory.BGM)
        {
            GetLoopAudioSource().volume = volume;
            if (save)
            {
                PlayerPrefs.SetFloat(BGM_VOLUME_KEY, volume);
                PlayerPrefs.Save();
            }
        }
        else if (category == AudioCategory.SFX)
        {
            foreach (var source in GetOnceAudioSources())
            {
                source.volume = volume;
            }
            if (save)
            {
                PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
                PlayerPrefs.Save();
            }
        }
    }

    public float GetVolume(AudioCategory category)
    {
        if (category == AudioCategory.BGM)
        {
            return GetLoopAudioSource().volume;
        }
        else if (category == AudioCategory.SFX)
        {
            return GetOnceAudioSources().Length > 0 ? GetOnceAudioSources()[0].volume : 0;
        }
        return 0;
    }

    private void Start()
    {
        // Example: Play sound at start (optional)
        //PlaySound(AudioCategory.BGM, AudioTag.Town);
    }

    private void Update()
    {
        CheckForMainMenuClick();
    }

    private void CheckForMainMenuClick()
    {
        if (SceneManager.GetActiveScene().name == "Mainmenu" && Input.GetMouseButtonDown(0))
        {
            PlaySound(AudioCategory.SFX, AudioTag.Click);
        }
    }

    public void PlaySound(AudioCategory category, AudioTag tag)
    {
        var audioInfo = audioSet.audioInfos.FirstOrDefault(info => info.category == category && info.tags.Contains(tag));
        if (audioInfo != null && audioInfo.clip != null)
        {
            if (!audioInfo.loop)
            {
                AudioSource audioSourceOnce = audioSourceOnces.FirstOrDefault(source => source && !source.isPlaying);

                if (audioSourceOnce == null)
                {
                    audioSourceOnce = audioSourceOnces[0];
                }
                audioSourceOnce.clip = audioInfo.clip;
                audioSourceOnce.volume = GetVolume(AudioCategory.SFX); // Set to the current SFX volume
                audioSourceOnce.loop = audioInfo.loop;
                audioSourceOnce.Play();
            }
            else
            {
                audioSourceLoop.clip = audioInfo.clip;
                audioSourceLoop.volume = GetVolume(AudioCategory.BGM); // Set to the current BGM volume
                audioSourceLoop.loop = audioInfo.loop;
                audioSourceLoop.Play();
            }
        }
        else
        {
            Debug.LogWarning($"No audio found for category {category} with tag {tag}");
        }
    }

    public void PlaySound(string name)
    {
        var audioInfo = audioSet.audioInfos.FirstOrDefault(info => info.name == name);
        if (audioInfo != null && audioInfo.clip != null)
        {
            if (!audioInfo.loop)
            {
                AudioSource audioSourceOnce = audioSourceOnces.FirstOrDefault(source => source && !source.isPlaying);

                if (audioSourceOnce == null)
                {
                    audioSourceOnce = audioSourceOnces[0];
                }
                audioSourceOnce.clip = audioInfo.clip;
                audioSourceOnce.volume = GetVolume(AudioCategory.SFX); // Set to the current SFX volume
                audioSourceOnce.loop = audioInfo.loop;
                audioSourceOnce.Play();
            }
            else
            {
                audioSourceLoop.clip = audioInfo.clip;
                audioSourceLoop.volume = GetVolume(AudioCategory.BGM); // Set to the current BGM volume
                audioSourceLoop.loop = audioInfo.loop;
                audioSourceLoop.Play();
            }
        }
        else
        {
            Debug.LogWarning($"No audio found for name {name}");
        }
    }
}
