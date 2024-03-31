using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HitState : IAIState
{
    private B_UnitBase unitBase;
    private float invincibleTime = 1.0f;
    private float maxInvincibleTime = 1.0f;

    public HitState(B_UnitBase unitBase)
    {
        this.unitBase = unitBase;
    }

    public void OnEnter()
    {
        Debug.Log("HitState OnEnter");
        unitBase.SetInvincible(true);
        unitBase.Anim.SetTrigger("tHit");
        invincibleTime = maxInvincibleTime;
    }

    public void OnExit()
    {
        Debug.Log("HitState OnExit");

        unitBase.SetInvincible(false);
    }

    public void OnUpdate()
    {
        Debug.Log("HitState OnUpdate");

        invincibleTime -= Time.deltaTime;

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
}
