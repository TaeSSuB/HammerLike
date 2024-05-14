using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingBossState : IBossAIState
{
    private B_Boss b_Boss;
    private int patternIdx = -1;
    private WeaponOrbitCommon weaponOrbitCommon;

    private float swingDelay = 2f;
    private float startSwingDelay = 2f;

    private bool isSwing = false;

    public SwingBossState(B_Boss boss, int patternIdx)
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

            weaponOrbitCommon.trackType = WeaponOrbitCommon.TrackType.None; // 선형 궤도를 사용
            weaponOrbitCommon.SetTracking(false);
        }
            
        // var xzPlayerPos = new Vector3(GameManager.Instance.Player.transform.position.x, b_Boss.transform.position.y, GameManager.Instance.Player.transform.position.z);
        // b_Boss.transform.LookAt(xzPlayerPos);

        swingDelay = startSwingDelay;
    }

    public void OnUpdate()
    {
        swingDelay -= Time.deltaTime;

        if(!isSwing)
        {
            if(swingDelay <= 0)
                StartSwing();
            else
            {
                // Temp. Swing 패턴 연출 미정으로 임시 할당.. 정면 방향 점차 이동
                weaponOrbitCommon.TargetObj.transform.position += b_Boss.transform.forward * Time.deltaTime * 4f;
            }
        }
    }

    public void OnExit()
    {
        if(b_Boss as B_Boss_SkeletonTorturer)
        {            
            if(weaponOrbitCommon == null)
            {
                var targetBoss = b_Boss as B_Boss_SkeletonTorturer;
                weaponOrbitCommon = targetBoss.WeaponOrbitCommon;
            }
                
            weaponOrbitCommon.trackType = WeaponOrbitCommon.TrackType.DirBasedPoint;
            weaponOrbitCommon.SetTracking(false);
            weaponOrbitCommon.DisableCollider();
        }

        swingDelay = startSwingDelay;
        isSwing = false;

        b_Boss.Anim.ResetTrigger("tPatternPlay");
    }

    void StartSwing()
    {
        isSwing = true;

        if(b_Boss as B_Boss_SkeletonTorturer)
        {
            if(weaponOrbitCommon == null)
            {
                var targetBoss = b_Boss as B_Boss_SkeletonTorturer;
                weaponOrbitCommon = targetBoss.WeaponOrbitCommon;
            }

            weaponOrbitCommon.SetTracking(true);
            weaponOrbitCommon.EnableCollider();
            weaponOrbitCommon.SetRigidKinematic(false);
        }

        
        b_Boss.Anim.SetTrigger("tPatternPlay");
        b_Boss.Anim.SetInteger("PatternIdx", patternIdx);

    }
}
