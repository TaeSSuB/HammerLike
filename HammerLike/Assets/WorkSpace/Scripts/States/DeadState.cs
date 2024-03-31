using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : IAIState
{
    private B_UnitBase unitBase;

    public DeadState(B_UnitBase unitBase)
    {
        this.unitBase = unitBase;
    }

    public void OnEnter()
    {
        Debug.Log("DeadState OnEnter");

        // Dead logic
        unitBase.Anim.SetTrigger("tDead");
        //unitBase.Anim.SetTrigger("Dead");
    }

    public void OnExit()
    {
        Debug.Log("DeadState OnExit()");


    }

    public void OnUpdate()
    {

    }
}


