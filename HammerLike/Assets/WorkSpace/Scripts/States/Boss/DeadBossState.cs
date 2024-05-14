using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBossState : IBossAIState
{
    private B_Boss b_Boss;

    public DeadBossState(B_Boss boss)
    {
        this.b_Boss = boss;
    }

    public void OnEnter()
    {
        b_Boss.DisableMovementAndRotation();
        b_Boss.Move(b_Boss.transform.position);
        b_Boss.Col.enabled = false;
        b_Boss.Agent.enabled = false;
        b_Boss.Rigid.velocity = Vector3.zero;
        b_Boss.Anim.SetTrigger("tDead");
    }

    public void OnExit()
    {
    }

    public void OnUpdate()
    {
    }
}
