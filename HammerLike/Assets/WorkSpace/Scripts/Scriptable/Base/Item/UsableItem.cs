using UnityEngine;

[CreateAssetMenu(fileName = "New UsableItem", menuName = "Inventory/Usable")]
public class UsableItem : ItemBase
{
    public int healAmount;

    public override void Use()
    {
        // ��� ������ ��� ���� ����
        Debug.Log(itemName + " used!");
    }
}