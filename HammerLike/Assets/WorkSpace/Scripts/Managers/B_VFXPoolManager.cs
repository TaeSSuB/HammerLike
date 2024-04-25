using UnityEngine;
using System.Collections.Generic;

// Temp 240408 - DB 구현 전까지 임시 할당, a.HG
// Load VFX name by value. just use ex. hit = "Hit"
public struct VFXName
{
    public const string Hit = "Hit";
    public const string Explosion = "Explosion";
    public const string Heal = "Heal";
}

public class B_VFXPoolManager : MonoBehaviour
{
    public static B_VFXPoolManager Instance;

    public VFXSet vfxSet; // Assign in the Unity editor

    private Dictionary<string, Queue<GameObject>> vfxPoolDictionary = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, float> vfxDurationDictionary = new Dictionary<string, float>();

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

        foreach (var vfxData in vfxSet.vfxDatas)
        {
            Queue<GameObject> vfxPool = new Queue<GameObject>();
            vfxPoolDictionary.Add(vfxData.name, vfxPool);
            vfxDurationDictionary.Add(vfxData.name, vfxData.duration);
        }
    }

    public GameObject GetVFX(string vfxName)
    {
        if (vfxPoolDictionary.ContainsKey(vfxName))
        {
            Queue<GameObject> vfxPool = vfxPoolDictionary[vfxName];
            if (vfxPool.Count == 0)
            {
                GameObject newVFX = Instantiate(vfxSet.vfxDatas[0].vfxPrefab);
                newVFX.SetActive(false);
                vfxPool.Enqueue(newVFX);
            }

            GameObject vfx = vfxPool.Dequeue();
            vfx.SetActive(true);
            return vfx;
        }
        else
        {
            Debug.LogWarning($"No VFX found with name {vfxName}");
            return null;
        }
    }

    public void ReturnVFX(string vfxName, GameObject vfx)
    {
        if (vfxPoolDictionary.ContainsKey(vfxName))
        {
            vfx.SetActive(false);
            vfxPoolDictionary[vfxName].Enqueue(vfx);
        }
        else
        {
            Debug.LogWarning($"No VFX found with name {vfxName}");
        }
    }

    // VFX 호출 시, VFXSet에 세팅해두고 이 함수로 호출. a.HG
    public void PlayVFX(string vfxName, Vector3 position)
    {
        GameObject vfx = GetVFX(vfxName);
        vfx.transform.position = position;
        vfx.SetActive(true);
        StartCoroutine(DeactivateVFX(vfxName, vfx, vfxDurationDictionary[vfxName]));
    }

    private System.Collections.IEnumerator DeactivateVFX(string vfxName, GameObject vfx, float duration)
    {
        yield return new WaitForSeconds(duration);
        ReturnVFX(vfxName, vfx);
    }
}
