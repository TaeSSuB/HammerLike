using UnityEngine;
using System.Linq;
using NuelLib;
public enum AudioCategory { BGM, SFX }
public enum AudioTag 
{ 
    MainMenu, 
    Battle, 
    Ambient, 
    UI, 
    Death,
    PickUp
}

public class B_AudioManager : SingletonMonoBehaviour<B_AudioManager>
{
    public SO_AudioSet audioSet; // Assign in the Unity editor

    // BGM, SFX source
    [SerializeField] private int audioSourceOnceCount = 5;
    private AudioSource[] audioSourceOnces;
    private AudioSource audioSourceLoop;

    protected override void Awake()
    {
        base.Awake();

        audioSourceOnces = new AudioSource[audioSourceOnceCount];
        // Component Self
        for (int i = 0; i < audioSourceOnceCount; i++)
        {
            audioSourceOnces[i] = gameObject.AddComponent<AudioSource>();
        }

        audioSourceLoop = gameObject.AddComponent<AudioSource>();

        // Audio Source Object
        //GameObject sfxPlayerObj = new GameObject("AudioSource");

        //audioSourceOnce = sfxPlayerObj.AddComponent<AudioSource>();
        //audioSourceLoop = sfxPlayerObj.AddComponent<AudioSource>();
    }

    private void Start()
    {
        // Temp 240408 - DB 구현 전까지 임시 할당, a.HG
        PlaySound(AudioCategory.BGM, AudioTag.Battle);
    }

    public void PlaySound(AudioCategory category, AudioTag tag)
    {
        var audioInfo = audioSet.audioInfos.FirstOrDefault(info => info.category == category && info.tags.Contains(tag));
        if (audioInfo != null && audioInfo.clip != null)
        {
            // for Play Once (VFX)
            if(!audioInfo.loop)
            {
                AudioSource audioSourceOnce = audioSourceOnces.FirstOrDefault(source => source && !source.isPlaying);

                // When all audioSourceOnce is playing
                if(audioSourceOnce == null)
                {
                    audioSourceOnce = audioSourceOnces[0];
                }
                audioSourceOnce.clip = audioInfo.clip;
                audioSourceOnce.volume = audioInfo.volume;
                audioSourceOnce.loop = audioInfo.loop;
                audioSourceOnce.Play();
            }
            // for BGM like loop sound
            else
            {
                audioSourceLoop.clip = audioInfo.clip;
                audioSourceLoop.volume = audioInfo.volume;
                audioSourceLoop.loop = audioInfo.loop;
                audioSourceLoop.Play();
            }
        }
        else
        {
            Debug.LogWarning($"No audio found for category {category} with tag {tag}");
        }
    }
}
