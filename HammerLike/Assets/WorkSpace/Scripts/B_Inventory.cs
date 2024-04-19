using System.Collections.Generic;
using UnityEngine;

public class B_Inventory : MonoBehaviour
{
    public List<ItemBase> items = new List<ItemBase>();

    public void AddItem(ItemBase item)
    {
        items.Add(item);
    }

    public void UseItem(ItemBase item)
    {
        item.Use();
    }
}
