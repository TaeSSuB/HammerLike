using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeadState : IAIState
{
    private B_UnitBase unitBase;
    private float deadTime = 3f;

    public DeadState(B_UnitBase unitBase)
    {
        this.unitBase = unitBase;
    }

    public void OnEnter()
    {
        //Debug.Log("DeadState OnEnter");

        // 240502 a.HG : Dead logic Unit에 포함 시켜야..
        unitBase.DisableMovementAndRotation();
        unitBase.Col.enabled = false;
        unitBase.Agent.enabled = false;
        unitBase.Anim.SetTrigger("tDead");

        if(unitBase as B_Skeleton_Prisoner)
        {
            var targetUnit = unitBase as B_Skeleton_Prisoner;
            if(targetUnit.WeaponColliderObj != null)
                targetUnit.WeaponColliderObj.SetActive(false);
        }
    }

    public void OnExit()
    {
        //Debug.Log("DeadState OnExit()");


    }

    public void OnUpdate()
    {
        //Debug.Log("DeadState OnUpdate()");

        deadTime -= Time.deltaTime;
        if (deadTime <= 0)
        {
            //unitBase.Init();
            unitBase.gameObject.SetActive(false);
            unitBase.Col.enabled = true;
            unitBase.Agent.enabled = true;
            (unitBase as B_Enemy).AIStateManager.SetState(AIStateType.IDLE);
        }

    }
}


