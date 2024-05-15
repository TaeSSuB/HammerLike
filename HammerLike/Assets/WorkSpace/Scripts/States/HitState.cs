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
        //Debug.Log("HitState OnEnter");
        unitBase.SetInvincible(true);
        unitBase.Anim.SetTrigger("tHit");

        HitEvent(1f);

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
        //Debug.Log("HitState OnExit");

        HitEvent(0f);
        unitBase.SetInvincible(false);
    }

    public void OnUpdate()
    {
        //Debug.Log("HitState OnUpdate");

        invincibleTime -= Time.deltaTime;

        //HitEvent(invincibleTime);

        if (invincibleTime <= 0)
        {
            if(unitBase.UnitStatus.currentHP <= 0)
            {
                (unitBase as B_Enemy).AIStateManager.SetState(AIStateType.DEAD);
            }
            else
            {
                (unitBase as B_Enemy).AIStateManager.SetState(AIStateType.IDLE);
            }
        }
    }

    // Temp 20240411 - Hit Event (Shader) a.HG
    protected void HitEvent(float amount = 1f)
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
