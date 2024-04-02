using UnityEngine;
using System.Linq;
public enum AudioCategory { BGM, SFX }
public enum AudioTag { MainMenu, Battle, Ambient, UI }

public class B_AudioManager : MonoBehaviour
{
    public SO_AudioSet audioSet; // Assign in the Unity editor
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlaySound(AudioCategory category, AudioTag tag)
    {
        var audioInfo = audioSet.audioInfos.FirstOrDefault(info => info.category == category && info.tags.Contains(tag));
        if (audioInfo != null && audioInfo.clip != null)
        {
            audioSource.clip = audioInfo.clip;
            audioSource.volume = audioInfo.volume;
            audioSource.loop = audioInfo.loop;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning($"No audio found for category {category} with tag {tag}");
        }
    }
}
