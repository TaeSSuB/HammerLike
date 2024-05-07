using UnityEngine;
using System.Collections.Generic;
using static SO_AudioSet;

public class AudioPoolManager : MonoBehaviour
{
    public static AudioPoolManager Instance;
    public SO_AudioSet audioSet;
    [SerializeField] private AudioPoolItem audioPoolItemPrefab;
    private List<AudioPoolItem> pool = new List<AudioPoolItem>();

    private void Awake()
    {
        Instance = this;
        InitializePool();
    }

    private void InitializePool()
    {
        // 초기 풀 사이즈는 프로젝트의 필요에 따라 조정
        for (int i = 0; i < 10; i++)
        {
            AddItemToPool();
        }
    }

    private AudioPoolItem AddItemToPool()
    {
        var newItem = Instantiate(audioPoolItemPrefab, transform);
        newItem.gameObject.SetActive(false);
        pool.Add(newItem);
        return newItem;
    }

    public void PlaySound(string audioName)
    {
        // array to list
        //AudioInfo audioData = System.Array.Find(audioSet.audioInfos, item => item.name == audioName);
        AudioInfo audioData = audioSet.audioInfos.Find(item => item.name == audioName);
        if (audioData != null)
        {
            AudioPoolItem item = GetAvailableItem();
            if (item != null)
            {
                item.gameObject.SetActive(true);
                item.PlayAudio(audioData);
            }
        }
        else
        {
            Debug.LogWarning("Audio name not found: " + audioName);
        }
    }

    private AudioPoolItem GetAvailableItem()
    {
        foreach (var item in pool)
        {
            if (!item.gameObject.activeInHierarchy)
            {
                return item;
            }
        }
        return AddItemToPool();
    }
}
