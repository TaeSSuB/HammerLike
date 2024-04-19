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

