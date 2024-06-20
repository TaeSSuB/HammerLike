using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullBossState : IBossAIState
{
    private B_Boss b_Boss;
    private int patternIdx = -1;

    private WeaponOrbitCommon weaponOrbitCommon;

    private float pullDelay = 0.5f;
    private float startPullDelay = 0.5f;
    private float pullSpeed = 50f;

    public PullBossState(B_Boss boss, int patternIdx)
    {
        this.b_Boss = boss;
        this.patternIdx = patternIdx;
    }

    public void OnEnter()
    {
        b_Boss.SetEnableHitEvent(false);

        if(b_Boss as B_Boss_SkeletonTorturer)
        {            
            var targetBoss = b_Boss as B_Boss_SkeletonTorturer;
            weaponOrbitCommon = targetBoss.WeaponOrbitCommon;

            weaponOrbitCommon.SetTracking(false);
            weaponOrbitCommon.SetRigidKinematic(true);
        }

        // b_Boss.Anim.SetTrigger("tPatternPlay");
        // b_Boss.Anim.SetInteger("PatternIdx", patternIdx);

        pullDelay = startPullDelay;

        // 땅에 닿았을 때를 기준으로 효과음 재생
        B_AudioManager.Instance.PlaySound("Boss_MaceGroundBlast", AudioCategory.SFX);

    }

    public void OnUpdate()
    {
        pullDelay -= Time.deltaTime;

        if(pullDelay <= 0)
        {
            b_Boss.Anim.SetBool("bGrabbed", true);

            PullWeapon();

            if(Vector3.Distance(GameManager.Instance.Player.transform.position, b_Boss.transform.position) <= b_Boss.UnitStatus.atkRange)
            {
                // b_Boss.Anim.ResetTrigger("tPatternPlay");
                b_Boss.BossController.SetState(BossAIStateType.ATTACK);
            }
        }
    }

    public void OnExit()
    {
        b_Boss.Anim.ResetTrigger("tPatternPlay");
        b_Boss.Anim.SetBool("bGrabbed", false);
    }
    
    private void PullWeapon()
    {
        var weaponObj = weaponOrbitCommon.TargetObj;
        var weaponCollider = weaponOrbitCommon.TargetCollider;
        
        var distance = Vector3.Distance(weaponObj.transform.position, b_Boss.transform.position);

        if(distance <= 0.1f)
            return;
            
        var towardMovePos = Vector3.MoveTowards(weaponObj.transform.position, b_Boss.transform.position, pullSpeed * Time.deltaTime);

        weaponObj.transform.position = towardMovePos;

        // if Player is Overlapped with Weapon, then Player will be moved to the Weapon's position
        if(Vector3.Distance(GameManager.Instance.Player.transform.position, weaponObj.transform.position) < weaponCollider.bounds.size.magnitude / 2f)
        {
            GameManager.Instance.Player.transform.position = towardMovePos;
        }

        // GameManager.Instance.Player.Rigid.AddForce((b_Boss.transform.position - GameManager.Instance.Player.transform.position).normalized * 100f);

    }

}
