using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IAIState
{
    private B_UnitBase unitBase;
    private Transform target;

    public ChaseState(B_UnitBase unitBase)
    {
        this.unitBase = unitBase;
    }

    public void OnEnter()
    {
        Debug.Log("ChaseState OnEnter");
    }

    public void OnExit()
    {
        Debug.Log("ChaseState OnExit");
    }

    public void OnUpdate()
    {
        Debug.Log("ChaseState OnUpdate");
    }
}

