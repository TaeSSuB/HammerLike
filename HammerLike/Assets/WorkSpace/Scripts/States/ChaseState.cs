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
        //Debug.Log("ChaseState OnEnter");
        this.target = GameManager.Instance.Player.transform;
        unitBase.Anim.SetBool("IsChasing", true);

        if(unitBase as B_Skeleton_Prisoner)
        {
            var targetUnit = unitBase as B_Skeleton_Prisoner;
            if(targetUnit.WeaponColliderObj != null)
                targetUnit.WeaponColliderObj.SetActive(false);
        }
    }

    public void OnExit()
    {
        //Debug.Log("ChaseState OnExit");
        unitBase.Anim.SetBool("IsChasing", false);
    }

    public void OnUpdate()
    {
        //Debug.Log("ChaseState OnUpdate");
        //Vector3 moveDir = target.position - unitBase.transform.position;

        Vector3 nextMove = Vector3.zero;
        Vector3 moveDir = target.position - unitBase.transform.position;

        float applyCoordScale = GameManager.Instance.CalcCoordScale(moveDir);
        float distance = moveDir.magnitude / applyCoordScale;
        float minDistance = Mathf.Infinity;

        //if (unitBase as B_Skeleton_Archer)
        //{
        //    minDistance = (unitBase.UnitStatus as SO_RangerUnitStatus).chasingRange;
        //}
        //else
        //{
        //    minDistance = unitBase.UnitStatus.atkRange;
        //}

        minDistance = unitBase.UnitStatus.atkRange;

        if (distance >= minDistance)
        {
            nextMove = unitBase.Move(target.position);

            // Look at Player
            unitBase.transform.LookAt(nextMove);
        }
        else
        {
            nextMove = unitBase.Move(unitBase.transform.position);
        }
    }
}

