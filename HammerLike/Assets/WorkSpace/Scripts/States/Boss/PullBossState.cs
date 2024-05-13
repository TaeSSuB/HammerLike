using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullBossState : IBossAIState
{
    private B_Boss b_Boss;
    private int patternIdx = -1;

    private GameObject weaponObj;

    private float pullDelay = 0.5f;
    private float startPullDelay = 0.5f;
    private float pullSpeed = 100f;

    public PullBossState(B_Boss boss, int patternIdx)
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
            weaponObj.GetComponent<Rigidbody>().isKinematic = true;
        }


        var xzPlayerPos = new Vector3(GameManager.Instance.Player.transform.position.x, b_Boss.transform.position.y, GameManager.Instance.Player.transform.position.z);
        b_Boss.transform.LookAt(xzPlayerPos);

        // b_Boss.Anim.SetTrigger("tPatternPlay");
        // b_Boss.Anim.SetInteger("PatternIdx", patternIdx);

        pullDelay = startPullDelay;
    }

    public void OnUpdate()
    {
        pullDelay -= Time.deltaTime;

        if(pullDelay <= 0)
        {
            b_Boss.Anim.SetBool("bGrabbed", true);

            PullWeapon();

            if(weaponObj != null)
            {
                if(Vector3.Distance(GameManager.Instance.Player.transform.position, b_Boss.transform.position) <= b_Boss.UnitStatus.atkRange * 1.5f)
                {
                    // b_Boss.Anim.ResetTrigger("tPatternPlay");
                    b_Boss.BossController.SetState(BossAIStateType.ATTACK);
                }
            }
        }
    }

    public void OnExit()
    {
        b_Boss.Anim.ResetTrigger("tPatternPlay");
        b_Boss.Anim.SetBool("bGrabbed", false);
        weaponObj.GetComponent<Rigidbody>().isKinematic = false;
        weaponObj.transform.position = (b_Boss as B_Boss_SkeletonTorturer).WeaponInitPos;
    }
    
    private void PullWeapon()
    {
        if(weaponObj != null)
        {
            weaponObj.transform.position = Vector3.MoveTowards(weaponObj.transform.position, b_Boss.transform.position, pullSpeed * Time.deltaTime);
            GameManager.Instance.Player.transform.position = Vector3.MoveTowards(GameManager.Instance.Player.transform.position, b_Boss.transform.position, pullSpeed * Time.deltaTime);
            // GameManager.Instance.Player.Rigid.AddForce((b_Boss.transform.position - GameManager.Instance.Player.transform.position).normalized * 100f);

        }
    }

}
