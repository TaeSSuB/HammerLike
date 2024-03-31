using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attack State
public class AttackState : IAIState
{
    private B_UnitBase unitBase;

    public AttackState(B_UnitBase unitBase)
    {
        this.unitBase = unitBase;
    }

    public void OnEnter()
    {
        Debug.Log("AttackState OnEnter");
        unitBase.Anim.SetTrigger("tAttack");
    }

    public void OnExit()
    {
        Debug.Log("AttackState OnExit");
    }

    public void OnUpdate()
    {
        //Debug.Log("AttackState OnUpdate");
    }
}
