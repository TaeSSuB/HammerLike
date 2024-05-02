using Sirenix.OdinInspector;
using UnityEngine;

public class ItemAttribute : MonoBehaviour
{

    [Title("ItemAttribute", "ItemAttribute", TitleAlignments.Centered)]

    public SomeBitmaskEnum ItemType;

    [System.Flags]
    public enum SomeBitmaskEnum
    {
        Weapon = 1 << 1,
        Armor = 1 << 2,
        Accessory = 1 << 3,
        Cosumable = 1 << 4,
        Currency = 1 << 5,
        Container = 1 << 6,
        All = Weapon | Armor | Accessory | Cosumable | Currency | Container
    }

}