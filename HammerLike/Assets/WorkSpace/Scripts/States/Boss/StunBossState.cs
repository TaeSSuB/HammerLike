using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunBossState : IBossAIState
{
    private B_Boss b_Boss;

    private float stunDuration = 5f;
    private float startStunDuration = 5f;
    public StunBossState(B_Boss boss)
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

        stunDuration = startStunDuration;

        B_AudioManager.Instance.PlaySound("Boss_Stun", AudioCategory.SFX);
    }

    public void OnUpdate()
    {
        stunDuration -= Time.deltaTime;

        if(stunDuration <= 0)
        {
            b_Boss.BossController.SetState(BossAIStateType.IDLE);
        }
    }

    public void OnExit()
    {

    }
}
