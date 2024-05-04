using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "B_ScriptableObjects/Inventory/Consumable")]
public class SO_Consumable : SO_Item
{
    [Header("Consumable Data")]
    public int heal;
}
