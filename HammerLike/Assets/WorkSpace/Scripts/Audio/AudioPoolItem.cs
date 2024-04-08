using UnityEngine;

public class AudioPoolItem : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudio(AudioData audioData)
    {
        audioSource.clip = audioData.clip;
        audioSource.volume = audioData.volume;
        audioSource.loop = audioData.loop;
        audioSource.Play();

        if (!audioSource.loop)
        {
            Invoke(nameof(ReturnToPool), audioData.clip.length);
        }
    }

    private void ReturnToPool()
    {
        gameObject.SetActive(false);
    }
}
