using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public int itemID;
    public enum ItemType { Equipment, Used, Ingredient, ETC };
    public ItemType itemType;
    public Sprite itemImage; // UI 표시용
    public GameObject itemPrefab; // 게임 내 표시용

    public Item(string name, int id, ItemType type, Sprite image, GameObject prefab)
    {
        itemName = name;
        itemID = id;
        itemType = type;
        itemImage = image;
        itemPrefab = prefab;
    }
}
