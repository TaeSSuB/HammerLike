using UnityEngine;

[CreateAssetMenu(fileName = "New UsableItem", menuName = "Inventory/Usable")]
public class UsableItem : ItemBase
{
    public int healAmount;

    public override void Use()
    {
        // 사용 아이템 사용 로직 구현
        Debug.Log(itemName + " used!");
    }
}