using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type { Use, Tresure, Weapon, Quest}

public class CItem : MonoBehaviour
{
    [Header("Item")]
    //public bool is_Dropped;
    public Type itemType;

}
