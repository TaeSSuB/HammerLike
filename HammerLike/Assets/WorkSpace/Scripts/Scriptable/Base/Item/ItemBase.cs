using UnityEngine;

public abstract class ItemBase : ScriptableObject
{
    public string itemName;
    public int itemID;
    public string description;

    // 아이템이 인벤토리에서 사용될 때 호출되는 메소드
    public abstract void Use();
}
