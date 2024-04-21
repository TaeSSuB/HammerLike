using System.Collections.Generic;
using UnityEngine;

public class B_Inventory : MonoBehaviour
{
    public List<SO_Item> items = new List<SO_Item>();

    public int capacity = 20;

    public bool AddItem(SO_Item item)
    {
        if (items.Count < capacity)
        {
            items.Add(item);
            return true;
        }
        else
        {
            Debug.Log("Not enough space in the container.");
            return false;
        }
    }

    public bool RemoveItem(SO_Item item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            return true;
        }
        else
        {
            Debug.Log("Item not found in the container.");
            return false;
        }
    }
}
