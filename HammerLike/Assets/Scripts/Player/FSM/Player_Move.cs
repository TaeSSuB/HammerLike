using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : cState
{
    public Player player;

    public Player_Move(Player _player)
    {
        player = _player;
    }

    public override void EnterState()
    {
        base.EnterState();
        player.animCtrl.SetLayerWeight(1, 1f);
        player.animCtrl.SetTrigger("tWalk");
    }

    public override void UpdateState()
    {
        base.UpdateState();
        player.aim.Aiming();

        // 키 입력에 따른 이동 처리
        bool isMoving = player.move.Move(player.stat.walkSpd,player.rewiredPlayer);
        if (!isMoving)
        {
            player.fsm.SetNextState("Player_Idle");
            ResetMovementAnimation();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.fsm.SetNextState("Player_Envasion");
        }

        if (Input.GetMouseButtonDown(0))
        {
            player.animCtrl.SetTrigger("tAtk");
            player.animCtrl.SetBool("bAttack", true);
            player.atk.Attack();
        }

        var temp = player.aim.rayResultPoint;
        temp.y = player.transform.position.y;
        player.transform.LookAt(temp);

        // 이동 방향에 따른 애니메이션 업데이트
        if (isMoving)
        {
            Vector3 relativeDir = Quaternion.Euler(-player.transform.rotation.eulerAngles) * player.move.lastMoveDir;
            player.animCtrl.SetFloat("MoveX", relativeDir.x);
            player.animCtrl.SetFloat("MoveZ", relativeDir.z);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }

    public override void LateUpdateState()
    {
        base.LateUpdateState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    private void ResetMovementAnimation()
    {
        player.animCtrl.SetFloat("MoveX", 0);
        player.animCtrl.SetFloat("MoveZ", 0);
    }
}
