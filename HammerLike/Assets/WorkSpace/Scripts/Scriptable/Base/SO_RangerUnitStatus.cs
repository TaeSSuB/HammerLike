using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ScriptableObject for Ranger Unit Status
[CreateAssetMenu(fileName = "RangerUnitStatus", menuName = "B_ScriptableObjects/Unit/RangerUnitStatus", order = 1)]
public class SO_RangerUnitStatus : SO_UnitStatus
{
    //[SerializeField] private GameObject projectilePrefab;
    // Start is called before the first frame update

    // projectile speed 
    public float projectileSpeed = 3f;

    public new SO_RangerUnitStatus MakeCopyStatus()
    {
        SO_RangerUnitStatus newStatus = CreateInstance<SO_RangerUnitStatus>();

        newStatus.currentHP = currentHP;
        newStatus.maxHP = maxHP;
        newStatus.moveSpeed = moveSpeed;
        newStatus.atkDamage = atkDamage;
        newStatus.atkRange = atkRange;
        newStatus.atkSpeed = atkSpeed;
        newStatus.maxAttackCooltime = maxAttackCooltime;
        newStatus.currentAttackCooltime = currentAttackCooltime;
        newStatus.mass = mass;

        newStatus.projectileSpeed = projectileSpeed;

        return newStatus;
    }
}
