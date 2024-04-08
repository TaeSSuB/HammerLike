using UnityEngine;

public abstract class ItemBase : ScriptableObject
{
    public string itemName;
    public int itemID;
    public string description;

    // �������� �κ��丮���� ���� �� ȣ��Ǵ� �޼ҵ�
    public abstract void Use();
}
