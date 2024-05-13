using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingBossState : IBossAIState
{
    private B_Boss b_Boss;
    private int patternIdx = -1;

    public SwingBossState(B_Boss boss, int patternIdx)
    {
        this.b_Boss = boss;
        this.patternIdx = patternIdx;
    }


    public void OnEnter()
    {
        var xzPlayerPos = new Vector3(GameManager.Instance.Player.transform.position.x, b_Boss.transform.position.y, GameManager.Instance.Player.transform.position.z);
        b_Boss.transform.LookAt(xzPlayerPos);
        
        b_Boss.Anim.SetTrigger("tPatternPlay");
        b_Boss.Anim.SetInteger("PatternIdx", patternIdx);

    }

    public void OnUpdate()
    {
        //throw new System.NotImplementedException();
    }

    public void OnExit()
    {
        b_Boss.Anim.ResetTrigger("tPatternPlay");
    }
}
