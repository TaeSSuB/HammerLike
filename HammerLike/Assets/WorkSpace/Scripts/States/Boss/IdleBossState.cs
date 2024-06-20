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
        b_Boss.SetEnableHitEvent(true);
        
        b_Boss.Anim.SetTrigger("tIdle");

        if(b_Boss as B_Boss_SkeletonTorturer)
        {
            var targetBoss = b_Boss as B_Boss_SkeletonTorturer;
            var weaponOrbitCommon = targetBoss.WeaponOrbitCommon;

            weaponOrbitCommon.SetTracking(false);
            weaponOrbitCommon.DisableCollider();
            weaponOrbitCommon.SetRigidKinematic(false);
        }
        
    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {

    }
}
