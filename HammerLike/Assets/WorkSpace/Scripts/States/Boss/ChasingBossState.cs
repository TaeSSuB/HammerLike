using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ChasingBossState : IBossAIState
{    
    private B_Boss b_Boss;
    private Transform target;
    private int patternIdx = -1;

    public ChasingBossState(B_Boss boss, int patternIdx)
    {
        this.b_Boss = boss;
        this.patternIdx = patternIdx;
    }
    public void OnEnter()
    {
        //Debug.Log("ChaseState OnEnter");
        this.target = GameManager.Instance.Player.transform;

        b_Boss.Anim.SetTrigger("tPatternPlay");
        b_Boss.Anim.SetInteger("PatternIdx", patternIdx);

        if(b_Boss as B_Boss_SkeletonTorturer)
        {
            var targetBoss = b_Boss as B_Boss_SkeletonTorturer;
            var weaponOrbitCommon = targetBoss.WeaponOrbitCommon;

            weaponOrbitCommon.SetTracking(false);
            weaponOrbitCommon.SetWeaponPos(targetBoss.WeaponInitPos);
            weaponOrbitCommon.DisableCollider();
            weaponOrbitCommon.SetRigidKinematic(true);
        }

    }

    public void OnUpdate()
    {
        Vector3 nextMove = Vector3.zero;
        Vector3 moveDir = target.position - b_Boss.transform.position;

        float applyCoordScale = GameManager.Instance.CalcCoordScale(moveDir);
        float distance = moveDir.magnitude / applyCoordScale;
        float minDistance = Mathf.Infinity;

        minDistance = b_Boss.UnitStatus.atkRange;

        if (distance >= minDistance)
        {
            nextMove = b_Boss.Move(target.position);

            // Look at Player
            b_Boss.transform.LookAt(nextMove);
        }
        else
        {
            nextMove = b_Boss.Move(b_Boss.transform.position);
            b_Boss.Anim.SetFloat("MoveAmount", 0f);
            b_Boss.BossController.SetState(BossAIStateType.ATTACK);
        }
    }

    public void OnExit()
    {
        //Debug.Log("ChaseState OnExit");
        b_Boss.Anim.SetFloat("MoveAmount", 0f);
        b_Boss.Anim.ResetTrigger("tPatternPlay");
    }
}
