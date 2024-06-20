using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Accessory", menuName = "B_ScriptableObjects/Inventory/Accessory")]
public class SO_Accessory : SO_Item
{
    [Header("Accessory Data")]
    public int stageNum;
}
