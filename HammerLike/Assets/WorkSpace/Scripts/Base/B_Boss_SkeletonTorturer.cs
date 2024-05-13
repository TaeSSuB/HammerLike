using HutongGames.PlayMaker.Actions;
using RootMotion.Demos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class B_Boss_SkeletonTorturer : B_Boss
{
    [SerializeField] private float distance = 0f;
    [SerializeField] private GameObject weaponObj;
    [SerializeField] private TMP_Text devText;

    public override Vector3 Move(Vector3 inPos)
    {            
        float moveAmount = Agent.velocity.normalized.magnitude;
            
        Anim.SetFloat("MoveAmount", moveAmount);

        return base.Move(inPos);
    }

    public override void EndAttack()
    {
        BossController.SetState(Thinking());
    }

    public override void Init()
    {
        base.Init();
    }
    
    protected override void Start()
    {
        base.Start();
        
        weaponObj.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

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
