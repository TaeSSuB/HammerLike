using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBossState : IBossAIState
{
    private B_Boss b_Boss;
    private int patternIdx = -1;
    private GameObject weaponObj;

    private float throwDelay = 1f;
    private float throwSpeed = 50f;

    public ThrowBossState(B_Boss boss, int patternIdx)
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
            weaponObj.transform.position = b_Boss.transform.position;
            weaponObj.GetComponent<Rigidbody>().isKinematic = true;
        }
            

        var xzPlayerPos = new Vector3(GameManager.Instance.Player.transform.position.x, b_Boss.transform.position.y, GameManager.Instance.Player.transform.position.z);
        b_Boss.transform.LookAt(xzPlayerPos);

        b_Boss.Anim.SetTrigger("tPatternPlay");
        b_Boss.Anim.SetInteger("PatternIdx", patternIdx);
    }

    public void OnUpdate()
    {
        throwDelay -= Time.deltaTime;

        if(throwDelay <= 0)
        {
            ThrowWeapon();

            if(weaponObj != null)
            {
                if(Vector3.Distance(weaponObj.transform.position, GameManager.Instance.Player.transform.position) <= 1f)
                {
                    throwDelay = 1f;
                    // b_Boss.Anim.ResetTrigger("tPatternPlay");
                    b_Boss.BossController.SetState(BossAIStateType.PULL);
                }
            }
        }
    }

    public void OnExit()
    {
        b_Boss.Anim.ResetTrigger("tPatternPlay");
        weaponObj.GetComponent<Rigidbody>().isKinematic = false;
    }

    public void ThrowWeapon()
    {
        if(weaponObj == null)
            return;

        // Approach Player
        weaponObj.transform.position = Vector3.MoveTowards(weaponObj.transform.position, GameManager.Instance.Player.transform.position, throwSpeed * Time.deltaTime);
    }


}
