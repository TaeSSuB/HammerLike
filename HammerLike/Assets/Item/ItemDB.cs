using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDB : MonoBehaviour
{
    public static ItemDB Instance;
    public List<Item> itemDatabase = new List<Item>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Item GetItemByID(int id)
    {
        return itemDatabase.Find(item => item.itemID == id);
    }
}