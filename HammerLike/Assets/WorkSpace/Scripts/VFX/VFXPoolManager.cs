using UnityEngine;
using System.Collections.Generic;

public class VFXPoolManager : MonoBehaviour
{
    public static VFXPoolManager Instance;
    public VFXSet vfxSet;
    private Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        Instance = this;
        InitializePools();
    }

    private void InitializePools()
    {
        foreach (var vfxData in vfxSet.vfxDatas)
        {
            var queue = new Queue<GameObject>();
            for (int i = 0; i < 10; i++) // 예시 초기 큐 사이즈
            {
                var newObj = CreateNewVFXItem(vfxData.vfxPrefab);
                queue.Enqueue(newObj);
            }
            pools[vfxData.name] = queue;
        }
    }

    private GameObject CreateNewVFXItem(GameObject prefab)
    {
        var newObj = Instantiate(prefab, transform);
        newObj.SetActive(false);
        return newObj;
    }

    public void PlayVFX(string name, Vector3 position)
    {
        if (pools.ContainsKey(name) && pools[name].Count > 0)
        {
            var vfx = pools[name].Dequeue();
            vfx.transform.position = position;
            vfx.SetActive(true);
            var vfxData = System.Array.Find(vfxSet.vfxDatas, vfxData => vfxData.name == name);
            vfx.GetComponent<VFXPoolItem>().PlayVFX(vfxData.duration);
            pools[name].Enqueue(vfx); // 재사용을 위해 다시 큐에 추가
        }
    }
}
