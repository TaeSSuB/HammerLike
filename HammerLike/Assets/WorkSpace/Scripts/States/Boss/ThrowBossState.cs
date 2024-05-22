using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBossState : IBossAIState
{
    private B_Boss b_Boss;
    private int patternIdx = -1;
    private GameObject weaponObj;
    private Vector3 lastPlayerPos;

    private float throwDelay = 2f;
    private float startThrowDelay = 2f;
    private float throwSpeed = 50f;

    public ThrowBossState(B_Boss boss, int patternIdx)
    {
        this.b_Boss = boss;
        this.patternIdx = patternIdx;
    }
    
    public void OnEnter()
    {
        b_Boss.SetEnableHitEvent(false);
        
        if(b_Boss as B_Boss_SkeletonTorturer)
        {
            (b_Boss as B_Boss_SkeletonTorturer).WeaponOrbitCommon.isTracking = false;
            weaponObj = (b_Boss as B_Boss_SkeletonTorturer).WeaponOrbitCommon.TargetObj;
            //weaponObj.transform.position = b_Boss.transform.position;
            weaponObj.GetComponent<Rigidbody>().isKinematic = true;
            
            (b_Boss as B_Boss_SkeletonTorturer).WeaponOrbitCommon.TargetCollider.enabled = false;
        }
        
        var xzPlayerPos = new Vector3(GameManager.Instance.Player.transform.position.x, b_Boss.transform.position.y, GameManager.Instance.Player.transform.position.z);
        b_Boss.transform.LookAt(xzPlayerPos);

        b_Boss.Anim.SetTrigger("tPatternPlay");
        b_Boss.Anim.SetInteger("PatternIdx", patternIdx);

        throwDelay = startThrowDelay;
    }

    public void OnUpdate()
    {
        throwDelay -= Time.deltaTime;

        if(throwDelay <= 0)
        {
            ThrowWeapon();

            if(weaponObj != null)
            {
                if(Vector3.Distance(weaponObj.transform.position, lastPlayerPos) <= 2f)
                {
                    b_Boss.BossController.SetState(BossAIStateType.PULL);
                }
            }
        }
        else
        {
            PrepareWeapon(throwDelay);
            lastPlayerPos = GameManager.Instance.Player.transform.position;
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

        (b_Boss as B_Boss_SkeletonTorturer).WeaponOrbitCommon.TargetCollider.enabled = true;
        // Approach Player
        weaponObj.transform.position = Vector3.MoveTowards(weaponObj.transform.position, lastPlayerPos, throwSpeed * Time.deltaTime);
    }

    // Return to Boss in delayTime
    public void PrepareWeapon(float delayTime)
    {
        if(weaponObj == null)
            return;

        float distance = Vector3.Distance(weaponObj.transform.position, (b_Boss as B_Boss_SkeletonTorturer).WeaponInitPos);
        float speed = distance / delayTime;
        weaponObj.transform.position = Vector3.MoveTowards(weaponObj.transform.position, (b_Boss as B_Boss_SkeletonTorturer).WeaponInitPos, speed * Time.deltaTime);
        
    }


}
