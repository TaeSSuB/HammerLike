using UnityEngine;
public enum ElementalType { None, Fire, Ice, Lightning }

[CreateAssetMenu(fileName = "New EquipItem", menuName = "Inventory/Equip")]
public class EquipItem : ItemBase
{
    //public int defense;
    //public int attack;

    public override void Use()
    {
        // ��� ������ ��� ���� ����
        Debug.Log(itemName + " equipped!");
    }
}