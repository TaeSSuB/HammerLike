using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public int itemID;
    public int quantity;
    public enum ItemType { Equipment, Used, Ingredient, ETC };
    public ItemType itemType;
    public Sprite itemImage; // UI Ç¥½Ã¿ë
    public GameObject itemPrefab;
    public int limitNumber=1;

    public Item(string name, int id, ItemType type, Sprite image, int limitNumber, int quantity)
    {
        itemName = name;
        itemID = id;
        itemType = type;
        itemImage = image;
        this.limitNumber = limitNumber;
        this.quantity = quantity;
        
    }
}
