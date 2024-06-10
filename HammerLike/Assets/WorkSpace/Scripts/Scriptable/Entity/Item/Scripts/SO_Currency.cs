using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Currency", menuName = "B_ScriptableObjects/Inventory/Currency")]
public class SO_Currency : SO_Item
{
    [Header("Currency Data")]
    public int value;
    public bool isLess;

    public override void Use()
    {
        // 인벤토리에서 화폐 아이템을 사용하면 호출됩니다.
        int amountToAdd = value;
        if (isLess)
        {
            amountToAdd = UnityEngine.Random.Range(1, value + 1);
        }

        // 플레이어 인벤토리에 화폐 아이템 추가
        B_InventoryManager.Instance.AddCurrencyItem(this, amountToAdd);
        Debug.Log($"Used {name}, added {amountToAdd} of {this.name} to player inventory.");
    }
}
