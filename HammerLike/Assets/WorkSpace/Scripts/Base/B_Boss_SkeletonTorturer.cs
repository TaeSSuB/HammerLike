using HutongGames.PlayMaker.Actions;
using RootMotion.Demos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class B_Boss_SkeletonTorturer : B_Boss
{
    [SerializeField] private WeaponOrbitCommon weaponOrbitCommon;
    [SerializeField] private Transform weaponInitPos;

    [SerializeField] private float distance = 0f;
    //[SerializeField] private GameObject weaponObj;
    [SerializeField] private TMP_Text devText;

    public WeaponOrbitCommon WeaponOrbitCommon => weaponOrbitCommon;
    public Vector3 WeaponInitPos => weaponInitPos.position;

    public override Vector3 Move(Vector3 inPos)
    {            
        float moveAmount = Agent.velocity.normalized.magnitude;
            
        Anim.SetFloat("MoveAmount", moveAmount);

        return base.Move(inPos);
    }

    public override void StartAttack()
    {
        base.StartAttack();
        weaponOrbitCommon.TargetCollider.enabled = true;
    }

    public override void EndAttack()
    {
        base.EndAttack();
        //BossController.SetState(Thinking());
        weaponOrbitCommon.TargetCollider.enabled = false;
    }

    public override void Init()
    {
        base.Init();
    }
    
    protected override void Start()
    {
        base.Start();
        //weaponObj.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        B_UIManager.Instance.UI_InGame.UpdateBossHP(this);

        // if Dead return
        if (UnitStatus.currentHP <= 0)
        {
            //DisableMovementAndRotation();
            return;
        }

        if (BossController?.CurrentStateType != BossAIStateType.HIT && BossController?.CurrentStateType != BossAIStateType.DEAD)
        {
            BossController.SetState(Thinking());
            // else
            // {
            //     BossController.SetState(BossAIStateType.IDLE);
            // }
            //BossController.SetState(BossAIStateType.IDLE);
        }
    }

    protected BossAIStateType Thinking()
    {
        var moveDir = GameManager.Instance.Player.transform.position - transform.position;

        float applyCoordScale = GameManager.Instance.CalcCoordScale(moveDir);
        var targetDis = moveDir.magnitude / applyCoordScale;

        if (targetDis <= unitStatus.detectRange)
        {
            return BossController.CheckPattern();
        }
        else
        {
            return BossAIStateType.CHASE;        
        }
    }

}
