using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Currency", menuName = "Inventory/Currency")]
public class SO_Currency : SO_Item
{
    [Header("Currency Data")]
    public int value;
}
