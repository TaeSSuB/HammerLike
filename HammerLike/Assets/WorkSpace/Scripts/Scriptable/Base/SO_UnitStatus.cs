using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitStatus", menuName = "B_ScriptableObjects/UnitStatus", order = 1)]
public class SO_UnitStatus : ScriptableObject
{
    public int currentHP = 100;
    public int maxHP = 100;

    public float moveSpeed = 5f;

    public int atkDamage = 10;
    public int atkDamageOrigin = 10;
    public float atkRange = 1f;
    public float atkSpeed = 1f;
    public float maxAttackCooltime = 1f;
    public float currentAttackCooltime = 0f;

    public float mass = 1f;

    public SO_UnitStatus MakeCopyStatus()
    {
        SO_UnitStatus newStatus = CreateInstance<SO_UnitStatus>();

        newStatus.currentHP = currentHP;
        newStatus.maxHP = maxHP;
        newStatus.moveSpeed = moveSpeed;
        newStatus.atkDamage = atkDamage;
        newStatus.atkDamageOrigin = atkDamageOrigin;
        newStatus.atkRange = atkRange;
        newStatus.atkSpeed = atkSpeed;
        newStatus.maxAttackCooltime = maxAttackCooltime;
        newStatus.currentAttackCooltime = currentAttackCooltime;
        newStatus.mass = mass;

        return newStatus;
    }
}
