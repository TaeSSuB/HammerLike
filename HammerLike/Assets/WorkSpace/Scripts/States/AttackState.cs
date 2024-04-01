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
        Debug.Log("AttackState OnEnter");
        //unitBase.Anim.SetTrigger("tAttack");
    }

    public void OnExit()
    {
        Debug.Log("AttackState OnExit");
    }

    public void OnUpdate()
    {
        //Debug.Log("AttackState OnUpdate");

        // Attack Cooltime
        if (unitBase.UnitStatus.currentAttackCooltime <= 0)
        {
            // Attack
            unitBase.UnitStatus.currentAttackCooltime = unitBase.UnitStatus.maxAttackCooltime;

            // Temp 20240402 - UnitBase에서 Attack 함수로 Anim 포함 일괄 호출하도록 변경 예정, a.HG
            //unitBase.Attack();
            // LookAt Player
            unitBase.transform.LookAt(GameManager.instance.Player.transform);
            unitBase.Anim.ResetTrigger("tAttack");
            unitBase.Anim.SetTrigger("tAttack");
        }
    }


}
