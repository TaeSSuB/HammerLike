using UnityEngine;
using System.Collections.Generic;

public class AudioPoolManager : MonoBehaviour
{
    public static AudioPoolManager Instance;
    public AudioSet audioSet;
    [SerializeField] private AudioPoolItem audioPoolItemPrefab;
    private List<AudioPoolItem> pool = new List<AudioPoolItem>();

    private void Awake()
    {
        Instance = this;
        InitializePool();
    }

    private void InitializePool()
    {
        // �ʱ� Ǯ ������� ������Ʈ�� �ʿ信 ���� ����
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
        AudioData audioData = System.Array.Find(audioSet.audioDatas, item => item.name == audioName);
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
