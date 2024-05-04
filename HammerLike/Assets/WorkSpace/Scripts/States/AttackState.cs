using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        //Debug.Log("AttackState OnEnter");
        //unitBase.Anim.SetTrigger("tAttack");
        //unitBase.StartAttack();
        var xzPlayerPos = new Vector3(GameManager.Instance.Player.transform.position.x, unitBase.transform.position.y, GameManager.Instance.Player.transform.position.z);
        unitBase.transform.LookAt(xzPlayerPos);
    }

    public void OnExit()
    {
        //Debug.Log("AttackState OnExit");
        //unitBase.EndAttack();
        unitBase.isAttacking = false;
    }

    public void OnUpdate()
    {
        //Debug.Log("AttackState OnUpdate");
        if (unitBase as B_Skeleton_Archer)
        {
            unitBase.transform.LookAt(GameManager.Instance.Player.transform);
        }

        // Attack Cooltime
        if (unitBase.UnitStatus.currentAttackCooltime <= 0)
        {
            // Attack
            unitBase.UnitStatus.currentAttackCooltime = unitBase.UnitStatus.maxAttackCooltime;

            unitBase.Anim.SetFloat("fRemainShot", unitBase.UnitStatus.maxAttackCooltime);
            unitBase.Anim.SetBool("IsAttacking", unitBase.isAttacking);

            // Temp 20240402 - UnitBase에서 Attack 함수로 Anim 포함 일괄 호출하도록 변경 예정, a.HG
            unitBase.Attack();

            unitBase.Anim.ResetTrigger("tAttack");
            unitBase.Anim.SetTrigger("tAttack");
        }
    }


}
