using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatus", menuName = "B_ScriptableObjects/Unit/PlayerStatus", order = 2)]
public class SO_PlayerStatus : SO_UnitStatus
{
    [Header("Player Status")]
    [Header("Dash")]
    public float dashSpeed = 10f;
    public float dashDuration = 0.5f;
    public float dashCooldown = 1f;
    protected float dashCooldownOrigin = 1f;

    // charge attack based attack
    // it will be multiply with atkDamage
    [Header("Charge")]
    public float chargeRate = 1f;
    public float minChargeRate = 1f;
    public float maxChargeRate = 2f;
    public float chargeRateIncrease = 0.1f;
    //protected float chargeRateOrigin = 1f;

    [Header("Stamina")]
    public float currentStamina = 100f;
    public float maxStamina = 100f;
    public float staminaRegenRate = 1f;

    public float DashCooldownOrigin => dashCooldownOrigin;

    public override void Init()
    {
        base.Init();

        dashCooldownOrigin = dashCooldown;
    }
}
