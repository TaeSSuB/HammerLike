using UnityEngine;

[CreateAssetMenu(fileName = "New MiscItem", menuName = "Inventory/Misc")]
public class MiscItem : ItemBase
{
    // ��Ÿ �����ۿ� Ưȭ�� �Ӽ� �߰� ����

    public override void Use()
    {
        // ��Ÿ ������ ��� ���� ����
        Debug.Log(itemName + " used in a special way!");
    }
}