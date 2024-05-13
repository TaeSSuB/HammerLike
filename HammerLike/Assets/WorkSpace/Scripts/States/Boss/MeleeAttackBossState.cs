using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackBossState : IBossAIState
{
    private B_Boss b_Boss;

    private float startA;
    private float startB;

    public MeleeAttackBossState(B_Boss boss)
    {
        this.b_Boss = boss;
    }
    
    public void OnEnter()
    {
        if(b_Boss as B_Boss_SkeletonTorturer)
        {
            (b_Boss as B_Boss_SkeletonTorturer).WeaponOrbitCommon.trackType = WeaponOrbitCommon.TrackType.DirBasedPoint;
            (b_Boss as B_Boss_SkeletonTorturer).WeaponOrbitCommon.isTracking = true;
            
            startA = (b_Boss as B_Boss_SkeletonTorturer).WeaponOrbitCommon.a;
            startB = (b_Boss as B_Boss_SkeletonTorturer).WeaponOrbitCommon.b;

            (b_Boss as B_Boss_SkeletonTorturer).WeaponOrbitCommon.a *= 0.5f;
            (b_Boss as B_Boss_SkeletonTorturer).WeaponOrbitCommon.b *= 0.5f;
        }

        var xzPlayerPos = new Vector3(GameManager.Instance.Player.transform.position.x, b_Boss.transform.position.y, GameManager.Instance.Player.transform.position.z);
        b_Boss.transform.LookAt(xzPlayerPos);

        // b_Boss.Anim.SetTrigger("tPatternPlay");
        // b_Boss.Anim.SetInteger("PatternIdx", patternIdx);

        b_Boss.UnitStatus.currentAttackCooltime = b_Boss.UnitStatus.maxAttackCooltime;

        b_Boss.Attack();
        b_Boss.Anim.SetBool("bAttack", b_Boss.isAttacking);
        b_Boss.Anim.SetTrigger("tAttack");
    }

    public void OnUpdate()
    {
        b_Boss.UnitStatus.currentAttackCooltime -= Time.deltaTime;

        // Attack Cooltime
        if (b_Boss.UnitStatus.currentAttackCooltime <= 0)
        {
            // Attack
            b_Boss.UnitStatus.currentAttackCooltime = b_Boss.UnitStatus.maxAttackCooltime;

            b_Boss.Attack();
        }
    }

    public void OnExit()
    {
        b_Boss.UnitStatus.currentAttackCooltime = b_Boss.UnitStatus.maxAttackCooltime;

        (b_Boss as B_Boss_SkeletonTorturer).WeaponOrbitCommon.a = startA;
        (b_Boss as B_Boss_SkeletonTorturer).WeaponOrbitCommon.b = startB;

        b_Boss.isAttacking = false;
        b_Boss.Anim.ResetTrigger("tPatternPlay");
        b_Boss.Anim.SetInteger("PatternIdx", -1);
        b_Boss.Anim.SetBool("bAttack", b_Boss.isAttacking);
        b_Boss.Anim.ResetTrigger("tAttack");
    }
}
