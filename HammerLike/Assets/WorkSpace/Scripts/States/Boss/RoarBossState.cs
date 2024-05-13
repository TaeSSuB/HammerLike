using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoarBossState : IBossAIState
{
    private B_Boss b_Boss;

    private int patternIdx = -1;

    private GameObject weaponObj;

    public RoarBossState(B_Boss boss, int patternIdx)
    {
        this.b_Boss = boss;
        this.patternIdx = patternIdx;
    }
    
    public void OnEnter()
    {
        if(b_Boss as B_Boss_SkeletonTorturer)
        {
            (b_Boss as B_Boss_SkeletonTorturer).WeaponOrbitCommon.isTracking = false;
            weaponObj = (b_Boss as B_Boss_SkeletonTorturer).WeaponOrbitCommon.TargetObj;
            weaponObj.transform.position = (b_Boss as B_Boss_SkeletonTorturer).WeaponInitPos;
        }
            

        var xzPlayerPos = new Vector3(GameManager.Instance.Player.transform.position.x, b_Boss.transform.position.y, GameManager.Instance.Player.transform.position.z);
        b_Boss.transform.LookAt(xzPlayerPos);

        b_Boss.Anim.SetTrigger("tPatternPlay");
        b_Boss.Anim.SetInteger("PatternIdx", patternIdx);
    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {
        b_Boss.Anim.ResetTrigger("tPatternPlay");
    }
}
