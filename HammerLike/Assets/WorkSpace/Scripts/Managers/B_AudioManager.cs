using UnityEngine;
using System.Collections.Generic;
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
    [SerializeField] private int audioSourceLoopCount = 5;
    private AudioSource[] audioSourceOnces;
    private AudioSource[] audioSourceLoops;

    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    public AudioSource[] GetLoopAudioSources()
    {
        return audioSourceLoops;
    }

    public AudioSource[] GetOnceAudioSources()
    {
        return audioSourceOnces;
    }

    public SO_AudioSet.AudioInfo[] GetAllAudioInfosWithCategory(AudioCategory category)
    {
        return audioSet.audioInfos.Where(info => info.category == category).ToArray();
    }

    protected override void Awake()
    {
        base.Awake();

        audioSourceOnces = new AudioSource[audioSourceOnceCount];
        for (int i = 0; i < audioSourceOnceCount; i++)
        {
            audioSourceOnces[i] = gameObject.AddComponent<AudioSource>();
        }

        audioSourceLoops = new AudioSource[audioSourceLoopCount];
        for (int i = 0; i < audioSourceLoopCount; i++)
        {
            audioSourceLoops[i] = gameObject.AddComponent<AudioSource>();
        }

        //audioSourceLoop = gameObject.AddComponent<AudioSource>();

        // Load saved volumes
        float bgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1.0f);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1.0f);

        SetVolume(AudioCategory.BGM, bgmVolume, false);
        SetVolume(AudioCategory.SFX, sfxVolume, false);
    }

    public void SetVolume(AudioCategory category, float volume, bool save = true)
    {
        var targetInfos = GetAllAudioInfosWithCategory(category);

        if (category == AudioCategory.BGM)
        {
            // GetLoopAudioSource().volume = volume;
            foreach (var info in targetInfos)
            {
                info.volume = volume;
            }
            if (save)
            {
                PlayerPrefs.SetFloat(BGM_VOLUME_KEY, volume);
                PlayerPrefs.Save();
            }
        }
        else if (category == AudioCategory.SFX)
        {
            foreach (var info in targetInfos)
            {
                info.volume = volume;
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
        // if (category == AudioCategory.BGM)
        // {
        //     return GetAllAudioSourcesWithCategory(category).Length > 0 ? GetAllAudioSourcesWithCategory(category)[0].volume : 0;
        // }
        // else if (category == AudioCategory.SFX)
        // {
        //     return GetAllAudioSourcesWithCategory(category).Length > 0 ? GetAllAudioSourcesWithCategory(category)[0].volume : 0;
            
        // }
        // return 0;
        return GetAllAudioInfosWithCategory(category).Length > 0 ? GetAllAudioInfosWithCategory(category)[0].volume : 0;
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
                audioSourceOnce.volume = GetVolume(category);
                audioSourceOnce.loop = audioInfo.loop;
                audioSourceOnce.Play();
            }
            else
            {
                AudioSource audioSourceLoop = audioSourceLoops.FirstOrDefault(source => source && !source.isPlaying);

                if (audioSourceLoop == null)
                {
                    audioSourceLoop = audioSourceLoops[0];
                }
                audioSourceLoop.clip = audioInfo.clip;
                audioSourceLoop.volume = GetVolume(category);
                audioSourceLoop.loop = audioInfo.loop;
                audioSourceLoop.Play();
            }
        }
        else
        {
            Debug.LogWarning($"No audio found for category {category} with tag {tag}");
        }
    }

    public void PlaySound(string name, AudioCategory category)
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
                audioSourceOnce.volume = GetVolume(category);
                audioSourceOnce.loop = audioInfo.loop;
                audioSourceOnce.Play();
                Debug.Log($"Playing sound {name}");

            }
            else
            {
                AudioSource audioSourceLoop = audioSourceLoops.FirstOrDefault(source => source && !source.isPlaying);

                if (audioSourceLoop == null)
                {
                    audioSourceLoop = audioSourceLoops[0];
                }
                audioSourceLoop.clip = audioInfo.clip;
                audioSourceLoop.volume = GetVolume(category);
                audioSourceLoop.loop = audioInfo.loop;
                audioSourceLoop.Play();

                Debug.Log($"Playing sound {name}");
            }
        }
        else
        {
            Debug.LogWarning($"No audio found for name {name}");
        }
    }

    public void StopSound(string name)
    {
        var audioInfo = audioSet.audioInfos.FirstOrDefault(info => info.name == name);
        if (audioInfo != null && audioInfo.clip != null)
        {
            if (!audioInfo.loop)
            {
                AudioSource audioSourceOnce = audioSourceOnces.FirstOrDefault(source => source && source.clip == audioInfo.clip);

                if (audioSourceOnce != null)
                {
                    audioSourceOnce.Stop();
                }
            }
            else
            {
                AudioSource audioSourceLoop = audioSourceLoops.FirstOrDefault(source => source && source.clip == audioInfo.clip);

                if (audioSourceLoop != null)
                {
                    audioSourceLoop.Stop();
                }
            }
        }
        else
        {
            Debug.LogWarning($"No audio found for name {name}");
        }

    }
    
}
