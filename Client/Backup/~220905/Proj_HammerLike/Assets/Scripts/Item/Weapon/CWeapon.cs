using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rank { Common, Rare, Unique, Legend}

public class CWeapon : CItem
{
    [Header("Weapon")]
    public int atkData;
    public int rangeData;
    public int knockbackData = 1;
    public bool is_Attacking;

    public void PickUpWeapon(CPlayer player)
    {
        player.weaponSwitcher.LootWeapon(this);
    }

}
