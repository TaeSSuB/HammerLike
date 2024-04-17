using UnityEngine;
public enum ElementalType { None, Fire, Ice, Lightning }

[CreateAssetMenu(fileName = "New EquipItem", menuName = "Inventory/Equip")]
public class EquipItem : ItemBase
{
    //public int defense;
    //public int attack;

    public override void Use()
    {
        // 장비 아이템 사용 로직 구현
        Debug.Log(itemName + " equipped!");
    }
}