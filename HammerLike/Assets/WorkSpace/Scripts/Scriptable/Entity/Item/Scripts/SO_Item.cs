using System;
using UnityEditor.Rendering;
using UnityEngine;

public enum ElementalType { None, Fire, Ice, Lightning }

public enum Rarity { Common, Uncommon, Rare, Epic, Legendary }

public enum Attributes
{
    Health,
    Stamina,
    Damage,
    Armor
}

public enum ItemType
{
    Food,
    Helmet,
    Chest,
    Pants,
    Boots,
    Weapon,
    Default
}

[Serializable]
public class ItemBuff
{
    public Attributes stat;
    public int value;
    public int min;   //buff min value roll
    public int max;   //buff max value roll
    public ItemBuff(int _min, int _max)
    {
        min = _min;
        max = _max;
        GenerateField();
    }

    public void AddValue(ref int v)
    {
        v += value;
    }

    public void GenerateField()
    {
        value = UnityEngine.Random.Range(min, max);
    }
}

public abstract class SO_Item : ScriptableObject
{
    [Header("Item Data")]
    public Sprite itemIcon = null;
    public string itemDescription = "Item Description";
    public int unlockDonateAmount = 0;
    public bool stackable = false;
    public int maxStacks = 1;

    [Header("Item Properties")]
    public B_Item itemData = new B_Item();
    public Rarity rarity = Rarity.Common;
    public ItemType itemType = ItemType.Default;

    public B_Item CreateItem()
    {
        B_Item newItem = new B_Item(this);

        return newItem;
    }

    // 아이템이 인벤토리에서 사용될 때 호출되는 메소드
    public virtual void Use()
    {
        Debug.Log("Used!");
    }

}

[System.Serializable]
public class B_Item
{
    public string itemName = "New Item";
    public int index = -1;

    public ItemBuff[] buffs;
    public int itemWeight = 0;
    public int itemPrice = 0;


    public B_Item()
    {
        itemName = "";
        index = -1;
    }
    public B_Item(SO_Item item)
    {
        itemName = item.name;
        index = item.itemData.index;
        buffs = new ItemBuff[item.itemData.buffs.Length];

        for (int i = 0; i < buffs.Length; i++)
        {
            buffs[i] = new ItemBuff(item.itemData.buffs[i].min, item.itemData.buffs[i].max);
        }
    }
}