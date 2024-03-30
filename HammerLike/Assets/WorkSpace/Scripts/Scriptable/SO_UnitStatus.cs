using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitStatus", menuName = "B_ScriptableObjects/UnitStatus", order = 1)]
public class SO_UnitStatus : ScriptableObject
{
    public float currentHP = 100;
    public float maxHP = 100;

    public float moveSpeed = 5f;

    public float atkDamage = 10f;
    public float atkRange = 1f;
    public float atkSpeed = 1f;

    public SO_UnitStatus MakeCopyStatus()
    {
        SO_UnitStatus newStatus = CreateInstance<SO_UnitStatus>();

        newStatus.currentHP = currentHP;
        newStatus.maxHP = maxHP;
        newStatus.moveSpeed = moveSpeed;
        newStatus.atkDamage = atkDamage;
        newStatus.atkRange = atkRange;
        newStatus.atkSpeed = atkSpeed;

        return newStatus;
    }
}
