using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatus", menuName = "B_ScriptableObjects/PlayerStatus", order = 2)]
public class SO_PlayerStatus : SO_UnitStatus
{
    // movement
    public float dashSpeed = 10f;
    public float dashDuration = 0.5f;
    public float dashCooldown = 1f;

    // charge attack based attack
    // it will be multiply with atkDamage
    public float chargeRate = 1f;
    public float minChargeRate = 1f;
    public float maxChargeRate = 2f;

    // stamina
    public float currentStamina = 100f;
    public float maxStamina = 100f;
    public float staminaRegenRate = 1f;

    public new SO_PlayerStatus MakeCopyStatus()
    {
        SO_PlayerStatus newStatus = CreateInstance<SO_PlayerStatus>();

        newStatus.currentHP = currentHP;
        newStatus.maxHP = maxHP;
        newStatus.moveSpeed = moveSpeed;
        newStatus.atkDamage = atkDamage;
        newStatus.atkRange = atkRange;
        newStatus.atkSpeed = atkSpeed;
        newStatus.mass = mass;

        newStatus.dashSpeed = dashSpeed;
        newStatus.dashDuration = dashDuration;
        newStatus.dashCooldown = dashCooldown;

        newStatus.chargeRate = chargeRate;
        newStatus.minChargeRate = minChargeRate;
        newStatus.maxChargeRate = maxChargeRate;

        newStatus.currentStamina = currentStamina;
        newStatus.maxStamina = maxStamina;
        newStatus.staminaRegenRate = staminaRegenRate;

        return newStatus;
    }

}
