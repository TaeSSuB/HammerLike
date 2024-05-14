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
            var targetBoss = b_Boss as B_Boss_SkeletonTorturer;
            var weaponOrbitCommon = targetBoss.WeaponOrbitCommon;

            weaponOrbitCommon.trackType = WeaponOrbitCommon.TrackType.DirBasedPoint;
            weaponOrbitCommon.SetTracking(true);
            
            startA = weaponOrbitCommon.a;
            startB = weaponOrbitCommon.b;

            // 보스 공격 범위로 수정 예정.
            // CoordScalling 작업 필요하여 임시로 0.5f로 수정
            // 유닛 인식 범위에도 CoordScalling 적용 필요할 듯..
            weaponOrbitCommon.a *= 0.5f;
            weaponOrbitCommon.b *= 0.5f;
        }

        var xzPlayerPos = new Vector3(GameManager.Instance.Player.transform.position.x, b_Boss.transform.position.y, GameManager.Instance.Player.transform.position.z);
        b_Boss.transform.LookAt(xzPlayerPos);

        // b_Boss.Anim.SetTrigger("tPatternPlay");
        // b_Boss.Anim.SetInteger("PatternIdx", patternIdx);

        // b_Boss.UnitStatus.currentAttackCooltime = b_Boss.UnitStatus.maxAttackCooltime;

        b_Boss.Anim.SetBool("bAttack", b_Boss.isAttacking);
        b_Boss.Anim.SetTrigger("tAttack");

        b_Boss.Attack();
    }

    public void OnUpdate()
    {
        // b_Boss.UnitStatus.currentAttackCooltime -= Time.deltaTime;

        // // Attack Cooltime
        // if (b_Boss.UnitStatus.currentAttackCooltime <= 0)
        // {
        //     // Attack
        //     b_Boss.UnitStatus.currentAttackCooltime = b_Boss.UnitStatus.maxAttackCooltime;

        //     b_Boss.Attack();
        // }
    }

    public void OnExit()
    {
        if(b_Boss as B_Boss_SkeletonTorturer)
        {
            var targetBoss = b_Boss as B_Boss_SkeletonTorturer;
            var weaponOrbitCommon = targetBoss.WeaponOrbitCommon;
            weaponOrbitCommon.a = startA;
            weaponOrbitCommon.b = startB;
        }

        // b_Boss.UnitStatus.currentAttackCooltime = b_Boss.UnitStatus.maxAttackCooltime;

        b_Boss.isAttacking = false;
        b_Boss.Anim.ResetTrigger("tPatternPlay");
        b_Boss.Anim.SetInteger("PatternIdx", -1);
        b_Boss.Anim.SetBool("bAttack", b_Boss.isAttacking);
        b_Boss.Anim.ResetTrigger("tAttack");
    }
}
