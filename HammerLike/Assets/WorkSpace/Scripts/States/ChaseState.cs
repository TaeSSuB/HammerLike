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
        this.target = GameManager.instance.Player.transform;
    }

    public void OnExit()
    {
        Debug.Log("ChaseState OnExit");
    }

    public void OnUpdate()
    {
        //Debug.Log("ChaseState OnUpdate");
        Vector3 moveDir = target.position - unitBase.transform.position;

        var resultMoveDir = unitBase.Move(moveDir);
        unitBase.transform.LookAt(resultMoveDir + unitBase.transform.position);
    }
}

