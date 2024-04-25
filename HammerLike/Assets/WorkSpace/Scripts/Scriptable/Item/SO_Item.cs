using UnityEngine;

public enum ElementalType { None, Fire, Ice, Lightning }

public enum Rarity { Common, Uncommon, Rare, Epic, Legendary }

public abstract class SO_Item : ScriptableObject
{
    [Header("Item Data")]
    public int index = 0;
    public Sprite itemIcon = null;
    public string itemName = "New Item";
    public string itemDescription = "Item Description";

    [Header("Item Properties")]
    public int itemWeight = 0;
    public int itemPrice = 0;
    public int unlockDonateAmount = 0;
    public Rarity rarity = Rarity.Common;

    public int maxStacks = 1;

    // 아이템이 인벤토리에서 사용될 때 호출되는 메소드
    public virtual void Use()
    {
        Debug.Log(itemName + " equipped!");
    }
}
