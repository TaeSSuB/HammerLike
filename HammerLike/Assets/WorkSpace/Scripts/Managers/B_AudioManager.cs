using UnityEngine;
using System.Linq;
public enum AudioCategory { BGM, SFX }
public enum AudioTag { MainMenu, Battle, Ambient, UI }

public class B_AudioManager : MonoBehaviour
{
    public static B_AudioManager Instance;

    public SO_AudioSet audioSet; // Assign in the Unity editor
    
    // BGM, SFX source
    private AudioSource audioSourceOnce;
    private AudioSource audioSourceLoop;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }

        audioSourceOnce = gameObject.AddComponent<AudioSource>();
        audioSourceLoop = gameObject.AddComponent<AudioSource>();
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
