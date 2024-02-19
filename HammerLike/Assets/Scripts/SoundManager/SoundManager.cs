using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private List<AudioSource> sfxSources = new List<AudioSource>();

    private float bgmVolume = 1f;
    private float sfxVolume = 1f;

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
    }

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource.clip != clip)
        {
            bgmSource.clip = clip;
            bgmSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        AudioSource freeSource = sfxSources.Find(source => !source.isPlaying);
        if (freeSource == null)
        {
            freeSource = gameObject.AddComponent<AudioSource>();
            freeSource.volume = sfxVolume;
            sfxSources.Add(freeSource);
        }
        freeSource.PlayOneShot(clip, sfxVolume);
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        bgmSource.volume = bgmVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        sfxSources.ForEach(source => source.volume = sfxVolume);
    }

    public void ToggleMute()
    {
        if (bgmSource.volume > 0f || sfxVolume > 0f)
        {
            SetBGMVolume(0f);
            SetSFXVolume(0f);
        }
        else
        {
            SetBGMVolume(bgmVolume);
            SetSFXVolume(sfxVolume);
        }
    }
}
