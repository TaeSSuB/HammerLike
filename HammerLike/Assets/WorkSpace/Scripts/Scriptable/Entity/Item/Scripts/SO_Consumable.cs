using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "B_ScriptableObjects/Inventory/Consumable")]
public class SO_Consumable : SO_Item
{
    // [Header("Consumable Data")]
    // public int heal; // buff Health

    public override void Use()
    {
        base.Use();

        // Use the item
        ItemBuff[] buffs = itemData.buffs;
        foreach (ItemBuff buff in buffs)
        {
            switch(buff.stat)
            {
                case Attributes.Gold:
                    Debug.Log("Adding " + buff.value + " gold");
                    B_InventoryManager.Instance.AddGold(buff.value);
                    break;
                case Attributes.Health:
                    Debug.Log("Healing " + buff.value + " health");
                    GameManager.Instance.Player.RestoreHP(buff.value);
                    break;
                default:
                    break;
            }
        }
    }
}
