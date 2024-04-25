using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Armor", menuName = "Inventory/Equip/Armor")]
public class SO_Armor : SO_Equipment
{
    [Header("Armor Data")]
    public float movementSpeed = 0;
    public int armor = 0;
}
