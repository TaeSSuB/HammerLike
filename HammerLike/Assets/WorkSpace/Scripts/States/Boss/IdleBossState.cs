using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBossState : IBossAIState
{
    private B_Boss b_Boss;

    public IdleBossState(B_Boss boss)
    {
        this.b_Boss = boss;
    }

    public void OnEnter()
    {
        b_Boss.Anim.SetTrigger("tIdle");
    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {

    }
}
