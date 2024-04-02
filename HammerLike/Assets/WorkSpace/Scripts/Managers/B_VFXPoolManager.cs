using UnityEngine;
using System.Collections.Generic;

public class B_VFXPoolManager : MonoBehaviour
{
    public static B_VFXPoolManager Instance;

    [SerializeField]
    private GameObject vfxPrefab;
    [SerializeField]
    private int initialPoolSize = 10;

    private List<B_VFXPoolItem> pool = new List<B_VFXPoolItem>();

    private void Awake()
    {
        Instance = this;
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewPoolItem();
        }
    }

    private B_VFXPoolItem CreateNewPoolItem()
    {
        var newItem = Instantiate(vfxPrefab, transform).GetComponent<B_VFXPoolItem>();
        newItem.gameObject.SetActive(false);
        pool.Add(newItem);
        return newItem;
    }

    public B_VFXPoolItem GetVFX()
    {
        foreach (var item in pool)
        {
            if (!item.gameObject.activeInHierarchy)
            {
                return item;
            }
        }

        return CreateNewPoolItem();
    }
    //public void TriggerVFX(Vector3 position)
    //{
    //    var vfx = VFXPoolManager.Instance.GetVFX();
    //    vfx.transform.position = position;
    //    vfx.PlayVFX();
    //}
}
