using UnityEngine;
public enum EquipmentCategory
{
    Helmet,
    Chest,
    Gloves,
    Boots,
    Weapon,
    Shield,
    Accessory
}

[CreateAssetMenu(fileName = "New EquipItem", menuName = "B_ScriptableObjects/Inventory/Equip")]
public class SO_Equipment : SO_Item
{
    [Header("Equipment")]
    public GameObject prefab;
    public int maxUpgrade = 0;
    public EquipmentCategory equipmentCategory;
}