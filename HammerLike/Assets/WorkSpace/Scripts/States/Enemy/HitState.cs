using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HitState : IAIState
{
    private B_UnitBase unitBase;
    private float invincibleTime = 0.5f;
    private float maxInvincibleTime = 0.5f;

    public HitState(B_UnitBase unitBase)
    {
        this.unitBase = unitBase;
    }

    public void OnEnter()
    {
        unitBase.SetInvincible(true);
        unitBase.Anim?.SetTrigger("tHit");

        ApplyHitAmount(1f);

        invincibleTime = maxInvincibleTime;

        if(unitBase as B_Skeleton_Prisoner)
        {
            var targetUnit = unitBase as B_Skeleton_Prisoner;
            if(targetUnit.WeaponColliderObj != null)
                targetUnit.WeaponColliderObj.SetActive(false);
        }
    }

    public void OnExit()
    {
        ApplyHitAmount(0f);
        unitBase.SetInvincible(false);
    }

    public void OnUpdate()
    {
        invincibleTime -= Time.deltaTime;

        if (invincibleTime <= 0)
        {
            if(unitBase.IsAlive)
            {
                (unitBase as B_Enemy).AIStateManager.SetState(AIStateType.IDLE);
            }
            else
            {
                (unitBase as B_Enemy).AIStateManager.SetState(AIStateType.DEAD);
            }
        }
    }

    protected void ApplyHitAmount(float amount = 1f)
    {
        amount = Mathf.Clamp01(amount);

        var renderers  = unitBase.MeshObj?.GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
        {
            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.materials)
                {
                    material.SetFloat("_HitAmount", amount);
                }
            }
        }
    }
}
