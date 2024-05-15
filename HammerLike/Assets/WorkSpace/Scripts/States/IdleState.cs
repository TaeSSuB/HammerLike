using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IAIState
{
    private B_UnitBase unitBase;

    public IdleState(B_UnitBase unitBase)
    {
        this.unitBase = unitBase;
    }

    public void OnEnter()
    {
        //Debug.Log("IdleState OnEnter");
        unitBase.Anim.SetTrigger("tIdle");

        if(unitBase as B_Skeleton_Prisoner)
        {
            var targetUnit = unitBase as B_Skeleton_Prisoner;
            if(targetUnit.WeaponColliderObj != null)
                targetUnit.WeaponColliderObj.SetActive(false);
        }
    }

    public void OnExit()
    {
        //Debug.Log("IdleState OnExit");
    }

    public void OnUpdate()
    {
        //Debug.Log("IdleState OnUpdate");
    }
}

