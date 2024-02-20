using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveMoneyObject : MonoBehaviour
{
    private ItemManager item;
    private MeshDestroy mesh;
    private bool isGiven = false;
    void Start()
    {
        GameObject itemDB = GameObject.Find("ItemDB");
        if (itemDB != null)
        {
            item = itemDB.GetComponent<ItemManager>();
            if (item == null)
            {
                Debug.LogWarning("ItemManager component not found on 'itemDB' GameObject.");
            }
        }
        else
        {
            Debug.LogWarning("GameObject 'itemDB' not found in the scene.");
        }

        mesh = GetComponent<MeshDestroy>();
        if (mesh == null)
        {
            Debug.LogWarning("MeshDestroy component not found on the GameObject.");
        }
    }

    void Update()
    {
        if (!isGiven && mesh != null && mesh.curHp <= 0)
        {
            if (item != null)
            {
                item.DropItem(0, transform.position);
                isGiven = true;
            }
            else
            {
                Debug.LogWarning("Cannot drop item because ItemManager is null.");
            }
        }
    }

}
